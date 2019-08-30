using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using System;

public class Polygon
{
    public List<int> m_Vertices;
    public List<Polygon> m_Neighbors;
    public Color32 m_Color;
    public bool m_SmoothNormals;


    public Polygon(int a, int b, int c)
    {
        m_Vertices = new List<int>() { a, b, c };
        m_Neighbors = new List<Polygon>();

        m_SmoothNormals = true;

        m_Color = new Color32(255, 0, 255, 255);
    }

    public bool IsNeighborOf(Polygon other_poly)
    {
        int shared_vertices = 0;
        foreach (int vertex in m_Vertices)
        {
            if (other_poly.m_Vertices.Contains(vertex))
                shared_vertices++;
        }
        return shared_vertices == 2;
    }

    public void ReplaceNeighbor(Polygon oldNeighbor, Polygon newNeighbor)
    {
        for (int i = 0; i < m_Neighbors.Count; i++)
        {
            if (oldNeighbor == m_Neighbors[i])
            {
                m_Neighbors[i] = newNeighbor;
                return;
            }
        }
    }
}

public class Planet : MonoBehaviour
{
    // These public parameters can be tweaked to give different styles to your planet.

    public Material m_Material;
    public GameObject _nodePrefab;

    public int m_NumberOfContinents = 5;
    public float m_ContinentSizeMax = 1.0f;
    public float m_ContinentSizeMin = 0.1f;

    public int m_NumberOfHills = 5;


    public float m_HillSizeMax = 1.0f;
    public float m_HillSizeMin = 0.1f;

    public int m_NumberOfShops = 5;
    public float m_shopRangeMin = 3.0f;


    // Internally, the Planet object stores its mesh as a child GameObject:
    GameObject m_PlanetMesh;

    // The subdivided icosahedron that we use to generate our planet is represented as a list
    // of Polygons, and a list of Vertices for those Polygons:
    List<Polygon> m_Polygons;
    List<Vector3> m_Vertices;

    Color32 colorOcean = new Color32(0, 80, 220, 0);
    Color32 colorGrass = new Color32(0, 220, 0, 0);
    Color32 colorDirt = new Color32(180, 140, 20, 0);

    PolySet landPolys = new PolySet();
    PolySet hillPolys = new PolySet();

    PolySet water = new PolySet();

    NodeController m_nodeController;

    private SkyNet _skyNet;
    private Ship _playerShip;

    private Battle _currBattle;

    public void Start()
    {

        m_nodeController = ScriptableObject.CreateInstance<NodeController>();
        // Create an icosahedron, subdivide it three times so that we have plenty of polys
        // to work with.
        CreateIcoSphere();
        Subdivide(4);
        // When we begin extruding polygons, we'll need each one to know who its immediate
        //neighbors are.
        CalculateNeighbors();
        // By default, everything is colored blue.

        foreach (Polygon p in m_Polygons)
            p.m_Color = colorOcean;

        // randomly sized spheres on the surface of the planet, and adding any Polygon that falls
        // inside that sphere.
        for (int i = 0; i < m_NumberOfContinents; i++)
        {
            float continentSize = Random.Range(m_ContinentSizeMin, m_ContinentSizeMax);

            PolySet newLand = GetPolysInSphere(Random.onUnitSphere, continentSize, m_Polygons);

            landPolys.UnionWith(newLand);
        }

        // This is our land now, so color it green. =)

        foreach (Polygon landPoly in landPolys)
        {
            landPoly.m_Color = colorGrass;
        }

        // The Extrude function will raise the land Polygons up out of the water.
        // It also generates a strip of new Polygons to connect the newly raised land
        // back down to the water level. We can color this vertical strip of land brown like dirt.

        PolySet sides = Extrude(landPolys, 0.05f);

        foreach (Polygon side in sides)
        {
            side.m_Color = colorDirt;
        }

        // Grab additional polygons to generate hills, but only from the set of polygons that are land.



        for (int i = 0; i < m_NumberOfHills; i++)
        {
            float hillSize = Random.Range(m_HillSizeMin, m_HillSizeMax);

            PolySet newHill = GetPolysInSphere(Random.onUnitSphere, hillSize, landPolys);

            hillPolys.UnionWith(newHill);
        }

        sides = Extrude(hillPolys, 0.05f);

        foreach (Polygon side in sides)
        {
            side.m_Color = colorDirt;
        }

        // Okay, we're done! Let's generate an actual game mesh for this planet.

        GenerateMesh();
        GetWaterPolygons();
        SpawnNodes(water);
    }
    public void Update()
    {

        if (_currBattle!= null){
            if (_currBattle.Update())
            {
                //update own synet for testing
                _skyNet.update(_currBattle);
            }else{
                Object.Destroy(_currBattle);
            }
        }
        else
        {
            Node clicked = m_nodeController.checkNodes();
            if (clicked != null)
            {
                //freezeplanet movement;
                if (clicked._nodeType == NODE_TYPES.BATTLE)
                {
                    _playerShip = new Ship("PLAYER", 20.0f, 5.0f, 1.0f);
                    _playerShip.AddWeapon(new Weapon(WEAPONS_TYPE.CANNON, EFFECT_TYPE.NONE));
                    _playerShip.AddWeapon(new Weapon(WEAPONS_TYPE.CANNON, EFFECT_TYPE.NONE));
                    _playerShip._level = 1;

                    _skyNet = new SkyNet(ref _playerShip, _playerShip._level);
                    _currBattle =  ScriptableObject.CreateInstance<Battle>();
                    _currBattle.StartBattle(_playerShip);
                    clicked._gameObject.GetComponent<MeshRenderer>().material.color = Color.magenta;
                }
            }
            
        }

    }


    public void GetWaterPolygons()
    {
        foreach (Polygon poly in m_Polygons)
        {
            if (!hillPolys.Contains(poly) && !landPolys.Contains(poly))
            {
                water.Add(poly);
            }
        }
    }

    public void CreateIcoSphere()
    {
        m_Polygons = new List<Polygon>();
        m_Vertices = new List<Vector3>();
        //12 vertices
        float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

        m_Vertices.Add(new Vector3(-1, t, 0).normalized);
        m_Vertices.Add(new Vector3(1, t, 0).normalized);
        m_Vertices.Add(new Vector3(-1, -t, 0).normalized);
        m_Vertices.Add(new Vector3(1, -t, 0).normalized);
        m_Vertices.Add(new Vector3(0, -1, t).normalized);
        m_Vertices.Add(new Vector3(0, 1, t).normalized);
        m_Vertices.Add(new Vector3(0, -1, -t).normalized);
        m_Vertices.Add(new Vector3(0, 1, -t).normalized);
        m_Vertices.Add(new Vector3(t, 0, -1).normalized);
        m_Vertices.Add(new Vector3(t, 0, 1).normalized);
        m_Vertices.Add(new Vector3(-t, 0, -1).normalized);
        m_Vertices.Add(new Vector3(-t, 0, 1).normalized);

        // And here's the formula for the 20 sides,
        // referencing the 12 vertices we just created.
        m_Polygons.Add(new Polygon(0, 11, 5));
        m_Polygons.Add(new Polygon(0, 5, 1));
        m_Polygons.Add(new Polygon(0, 1, 7));
        m_Polygons.Add(new Polygon(0, 7, 10));
        m_Polygons.Add(new Polygon(0, 10, 11));
        m_Polygons.Add(new Polygon(1, 5, 9));
        m_Polygons.Add(new Polygon(5, 11, 4));
        m_Polygons.Add(new Polygon(11, 10, 2));
        m_Polygons.Add(new Polygon(10, 7, 6));
        m_Polygons.Add(new Polygon(7, 1, 8));
        m_Polygons.Add(new Polygon(3, 9, 4));
        m_Polygons.Add(new Polygon(3, 4, 2));
        m_Polygons.Add(new Polygon(3, 2, 6));
        m_Polygons.Add(new Polygon(3, 6, 8));
        m_Polygons.Add(new Polygon(3, 8, 9));
        m_Polygons.Add(new Polygon(4, 9, 5));
        m_Polygons.Add(new Polygon(2, 4, 11));
        m_Polygons.Add(new Polygon(6, 2, 10));
        m_Polygons.Add(new Polygon(8, 6, 7));
        m_Polygons.Add(new Polygon(9, 8, 1));
    }

    public void Subdivide(int recursions)
    {
        var midPointList = new Dictionary<int, int>();
        //subdivide triangle into 4 smaller triangles repeat " recursion" times
        for (int i = 0; i < recursions; i++)
        {

            var newPolys = new List<Polygon>();
            foreach (var poly in m_Polygons)
            {
                int a = poly.m_Vertices[0];
                int b = poly.m_Vertices[1];
                int c = poly.m_Vertices[2];

                int ab = GetMidPointIndex(midPointList, a, b);
                int bc = GetMidPointIndex(midPointList, b, c);
                int ca = GetMidPointIndex(midPointList, c, a);

                newPolys.Add(new Polygon(a, ab, ca));
                newPolys.Add(new Polygon(b, bc, ab));
                newPolys.Add(new Polygon(c, ca, bc));
                newPolys.Add(new Polygon(ab, bc, ca));
            }
            // Replace all our old polygons with the new set created
            m_Polygons = newPolys;
        }
    }
    public int GetMidPointIndex(Dictionary<int, int> midPointList, int indexA, int indexB)
    {
        //key is ordered set of co ord (a, b) stored in one int
        int smallerIndex = Mathf.Min(indexA, indexB);
        int greaterIndex = Mathf.Max(indexA, indexB);
        int key = (smallerIndex << 16) + greaterIndex;
        // If a midpoint is already defined, just return it.
        int ret;
        if (midPointList.TryGetValue(key, out ret))
            return ret;

        Vector3 p1 = m_Vertices[indexA];
        Vector3 p2 = m_Vertices[indexB];
        Vector3 middle = Vector3.Lerp(p1, p2, 0.5f).normalized;

        ret = m_Vertices.Count;
        m_Vertices.Add(middle);

        midPointList.Add(key, ret);
        return ret;
    }

    public void ColourMesh()
    {
        foreach (Polygon p in m_Polygons)
            p.m_Color = colorOcean;
    }

    public void GenerateMesh()
    {
        if (m_PlanetMesh)
            Destroy(m_PlanetMesh);

        m_PlanetMesh = new GameObject("PlanetMesh");

        MeshRenderer surfaceRenderer = m_PlanetMesh.AddComponent<MeshRenderer>();
        surfaceRenderer.material = m_Material;

        Mesh terrainMesh = new Mesh();

        int vertexCount = m_Polygons.Count * 3;

        int[] indices = new int[vertexCount];

        Vector3[] vertices = new Vector3[vertexCount];
        Vector3[] normals = new Vector3[vertexCount];
        Color32[] colors = new Color32[vertexCount];

        for (int i = 0; i < m_Polygons.Count; i++)
        {
            var poly = m_Polygons[i];

            indices[i * 3 + 0] = i * 3 + 0;
            indices[i * 3 + 1] = i * 3 + 1;
            indices[i * 3 + 2] = i * 3 + 2;

            vertices[i * 3 + 0] = m_Vertices[poly.m_Vertices[0]];
            vertices[i * 3 + 1] = m_Vertices[poly.m_Vertices[1]];
            vertices[i * 3 + 2] = m_Vertices[poly.m_Vertices[2]];

            colors[i * 3 + 0] = poly.m_Color;
            colors[i * 3 + 1] = poly.m_Color;
            colors[i * 3 + 2] = poly.m_Color;

            if (poly.m_SmoothNormals)
            {
                normals[i * 3 + 0] = m_Vertices[poly.m_Vertices[0]].normalized;
                normals[i * 3 + 1] = m_Vertices[poly.m_Vertices[1]].normalized;
                normals[i * 3 + 2] = m_Vertices[poly.m_Vertices[2]].normalized;
            }
            else
            {
                Vector3 ab = m_Vertices[poly.m_Vertices[1]] - m_Vertices[poly.m_Vertices[0]];
                Vector3 ac = m_Vertices[poly.m_Vertices[2]] - m_Vertices[poly.m_Vertices[0]];

                Vector3 normal = Vector3.Cross(ab, ac).normalized;

                normals[i * 3 + 0] = normal;
                normals[i * 3 + 1] = normal;
                normals[i * 3 + 2] = normal;
            }
        }

        terrainMesh.vertices = vertices;
        terrainMesh.normals = normals;
        terrainMesh.colors32 = colors;

        terrainMesh.SetTriangles(indices, 0);

        MeshFilter terrainFilter = m_PlanetMesh.AddComponent<MeshFilter>();
        terrainFilter.mesh = terrainMesh;
    }

    bool checkShopDistance(Vector3 shopToPlace, List<Vector3> _shops)
    {

        foreach (Vector3 shop in _shops)
        {
            if (Vector3.Distance(shop, shopToPlace) < m_shopRangeMin)
            {
                return false;
            }
        }
        return true;
    }

    public void SpawnNodes(PolySet water)
    {
        List<Vector3> spawnPoints = new List<Vector3>();
        List<Polygon> _water = new List<Polygon>(water);
        for (int i = 0; i < m_NumberOfShops; i++)
        {

            Polygon poly = _water[Random.Range(0, water.Count)];
            Vector3 ab = m_Vertices[poly.m_Vertices[1]] - m_Vertices[poly.m_Vertices[0]];
            Vector3 ac = m_Vertices[poly.m_Vertices[2]] - m_Vertices[poly.m_Vertices[0]];

            Vector3 normal = Vector3.Cross(ab, ac).normalized;

            Vector3 shopToPlace = m_PlanetMesh.transform.TransformPoint(normal);
            if (checkShopDistance(shopToPlace, spawnPoints))
            {
                spawnPoints.Add(shopToPlace);
            }
            else
            {
                i--;
                m_shopRangeMin -= 0.02f;
            }

        }

        m_nodeController.GenerateNodes(_nodePrefab, spawnPoints, m_PlanetMesh.transform);
    }

    public List<int> GetUniqueVertices()
    {
        List<int> verts = new List<int>();
        foreach (Polygon poly in m_Polygons)
        {
            foreach (int v in poly.m_Vertices)
            {
                if (!verts.Contains(v))
                    verts.Add(v);
            }
        }
        return verts;
    }

    public void extrudeNodes()
    {
        List<int> verts = GetUniqueVertices();
        Vector3 center = Vector3.zero;
        foreach (int vert in verts)
            center += m_Vertices[vert];
        center /= verts.Count;

        foreach (int vert in verts)
        {
            Vector3 v = m_Vertices[vert];
            v = v.normalized * (v.magnitude + Random.Range(-0.05f, 0.05f));
            m_Vertices[vert] = v;
        }

    }

    public PolySet GetPolysInSphere(Vector3 center, float radius, IEnumerable<Polygon> source)
    {
        PolySet newSet = new PolySet();
        foreach (Polygon p in source)
        {
            foreach (int vertexIndex in p.m_Vertices)
            {
                float distanceToSphere = Vector3.Distance(center,
                                         m_Vertices[vertexIndex]);

                if (distanceToSphere <= radius)
                {
                    newSet.Add(p);
                    break;
                }
            }
        }
        return newSet;
    }

    // public PolySet Inset(PolySet polys, float interpolation)
    // {
    //     PolySet stitchedPolys = StitchPolys(polys);
    //     List<int> verts = polys.GetUniqueVertices();
    //     //Calculate the average center of all the vertices
    //     //in these Polygons.
    //     Vector3 center = Vector3.zero;
    //     foreach (int vert in verts)
    //         center += m_Vertices[vert];
    //     center /= verts.Count;
    //     // Pull each vertex towards the center, then correct
    //     // it's height so that it's as far from the center of
    //     // the planet as it was before.
    //     foreach (int vert in verts)
    //     {
    //         Vector3 v = m_Vertices[vert];
    //         float height = v.magnitude;
    //         v = Vector3.Lerp(v, center, interpolation);
    //         v = v.normalized * height;
    //         m_Vertices[vert] = v;
    //     }
    //     return stitchedPolys;
    // }

    public PolySet Extrude(PolySet polys, float height)
    {
        PolySet stitchedPolys = StitchPolys(polys);
        List<int> verts = polys.GetUniqueVertices();
        // Take each vertex in this list of polys, and push it
        // away from the center of the Planet by the height
        // parameter.
        foreach (int vert in verts)
        {
            Vector3 v = m_Vertices[vert];
            v = v.normalized * (v.magnitude + height);
            m_Vertices[vert] = v;
        }
        return stitchedPolys;
    }


    public List<int> CloneVertices(List<int> old_verts)
    {
        List<int> new_verts = new List<int>();
        foreach (int old_vert in old_verts)
        {
            Vector3 cloned_vert = m_Vertices[old_vert];
            new_verts.Add(m_Vertices.Count);
            m_Vertices.Add(cloned_vert);
        }
        return new_verts;
    }

    public PolySet StitchPolys(PolySet polys)
    {
        PolySet stichedPolys = new PolySet();
        var edgeSet = polys.CreateEdgeSet();
        var originalVerts = edgeSet.GetUniqueVertices();
        var newVerts = CloneVertices(originalVerts);
        edgeSet.Split(originalVerts, newVerts);
        foreach (Edge edge in edgeSet)
        {
            // Create new polys along the stitched edge. These
            // will connect the original poly to its former
            // neighbor.
            var stitch_poly1 = new Polygon(edge.m_OuterVerts[0],
                                           edge.m_OuterVerts[1],
                                           edge.m_InnerVerts[0]);
            var stitch_poly2 = new Polygon(edge.m_OuterVerts[1],
                                           edge.m_InnerVerts[1],
                                           edge.m_InnerVerts[0]);
            // Add the new stitched faces as neighbors to
            // the original Polys.
            edge.m_InnerPoly.ReplaceNeighbor(edge.m_OuterPoly,
                                             stitch_poly2);
            edge.m_OuterPoly.ReplaceNeighbor(edge.m_InnerPoly,
                                             stitch_poly1);
            m_Polygons.Add(stitch_poly1);
            m_Polygons.Add(stitch_poly2);
            stichedPolys.Add(stitch_poly1);
            stichedPolys.Add(stitch_poly2);
        }
        //Swap to the new vertices on the inner polys.
        foreach (Polygon poly in polys)
        {
            for (int i = 0; i < 3; i++)
            {
                int vert_id = poly.m_Vertices[i];
                if (!originalVerts.Contains(vert_id))
                    continue;

                int vert_index = originalVerts.IndexOf(vert_id);
                poly.m_Vertices[i] = newVerts[vert_index];
            }
        }
        return stichedPolys;
    }

    public void CalculateNeighbors()
    {
        foreach (Polygon poly in m_Polygons)
        {
            foreach (Polygon other_poly in m_Polygons)
            {
                if (poly == other_poly)
                    continue;

                if (poly.IsNeighborOf(other_poly))
                    poly.m_Neighbors.Add(other_poly);
            }
        }
    }
}

