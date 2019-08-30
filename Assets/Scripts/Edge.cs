using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    // The Poly that's inside the Edge. This is the one 
    // we'll be extruding or insetting.
    public Polygon m_InnerPoly;
    // The Poly that's outside the Edge. We'll be leaving 
    // this one alone.
    public Polygon m_OuterPoly;
    //The vertices along this edge, according to the Outer poly.
    public List<int> m_OuterVerts;
    //The vertices along this edge, according to the Inner poly.
    public List<int> m_InnerVerts;
    public Edge(Polygon inner_poly, Polygon outer_poly)
    {
        m_InnerPoly = inner_poly;
        m_OuterPoly = outer_poly;
        m_OuterVerts = new List<int>(2);
        m_InnerVerts = new List<int>(2);
        //Find which vertices these polys share.
        foreach (int vertex in inner_poly.m_Vertices)
        {
            if (outer_poly.m_Vertices.Contains(vertex))
                m_InnerVerts.Add(vertex);
        }
        // For consistency, we want the 'winding order' of the 
        // edge to be the same as that of the inner polygon.
        // So the vertices in the edge are stored in the same order
        // that you would encounter them if you were walking clockwise
        // around the polygon. That means the pair of edge vertices 
        // will be:
        // [1st inner poly vertex, 2nd inner poly vertex] or
        // [2nd inner poly vertex, 3rd inner poly vertex] or
        // [3rd inner poly vertex, 1st inner poly vertex]
        //
        // The formula above will give us [1st inner poly vertex, 
        // 3rd inner poly vertex] though, so we check for that 
        // situation and reverse the vertices.
        if (m_InnerVerts[0] == inner_poly.m_Vertices[0] &&
           m_InnerVerts[1] == inner_poly.m_Vertices[2])
        {
            int temp = m_InnerVerts[0];
            m_InnerVerts[0] = m_InnerVerts[1];
            m_InnerVerts[1] = temp;
        }
        // No manipulations have happened yet, so the outer and 
        // inner Polygons still share the same vertices.
        // We can instantiate m_OuterVerts as a copy of m_InnerVerts.
        m_OuterVerts = new List<int>(m_InnerVerts);
    }

}

public class EdgeSet : HashSet<Edge>
{
    // Split - Given a list of original vertex indices and a list of
    // replacements, update m_InnerVerts to use the new replacement
    // vertices.
    public void Split(List<int> oldVertices, List<int> newVertices)
    {
        foreach (Edge edge in this)
        {
            for (int i = 0; i < 2; i++)
            {
                edge.m_InnerVerts[i] = newVertices[oldVertices.IndexOf(
                                       edge.m_OuterVerts[i])];
            }
        }
    }
    // GetUniqueVertices - Get a list of all the vertices referenced
    // in this edge loop, with no duplicates.
    public List<int> GetUniqueVertices()
    {
        List<int> vertices = new List<int>();
        foreach (Edge edge in this)
        {
            foreach (int vert in edge.m_OuterVerts)
            {
                if (!vertices.Contains(vert))
                    vertices.Add(vert);
            }
        }
        return vertices;
    }
}

public class PolySet : HashSet<Polygon>
{
    //Given a set of Polys, calculate the set of Edges
    //that surround them.
    public EdgeSet CreateEdgeSet()
    {
        EdgeSet edgeSet = new EdgeSet();
        foreach (Polygon poly in this)
        {
            foreach (Polygon neighbor in poly.m_Neighbors)
            {
                if (this.Contains(neighbor))
                    continue;

                // If our neighbor isn't in our PolySet, then
                // the edge between us and our neighbor is one
                // of the edges of this PolySet.
                Edge edge = new Edge(poly, neighbor);
                edgeSet.Add(edge);
            }
        }
        return edgeSet;
    }
    // GetUniqueVertices calculates a list of the vertex indices 
    // used by these Polygons with no duplicates.
    public List<int> GetUniqueVertices()
    {
        List<int> verts = new List<int>();
        foreach (Polygon poly in this)
        {
            foreach (int vert in poly.m_Vertices)
            {
                if (!verts.Contains(vert))
                    verts.Add(vert);
            }
        }
        return verts;
    }
}