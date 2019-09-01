using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NodeController : ScriptableObject{

    public int _shopCount = 4;
    public int _eventCount = 2;

    public int _battleCount = 9;

    private GameObject _gameObject;
    public List<Node> _nodes;
    private List<GameObject> _objects;

    private GameObject _planet;

    public NodeController(){
        _objects = new List<GameObject>();
    }

    public Node checkNodes(){
        if (Input.GetMouseButtonDown(0)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit)){
                
                int index = int.Parse(hit.transform.name);
                Debug.Log("NODECOUNT: " + _nodes.Count.ToString());
                Debug.Log("index: " + index.ToString());

                return _nodes[index];
            }
        }
        return null;
    }

    public  List<Node>  RandomizeNodes(){
        List<Node> nodeList = CreateNodeListToGen();
        this._nodes = new List<Node>();
        int count = nodeList.Count;
        for(int i = 0; i < count; i++){
            int index = Random.Range(0,nodeList.Count);
            Node newNode =  new Node(nodeList[index]);
            _nodes.Add(newNode);
            nodeList.RemoveAt(index);
        }
        return _nodes;
    }

    public List<Node> CreateNodeListToGen(){
        List<Node> nodes = new List<Node>();
        for(int i = 0; i < _shopCount; i++){
            nodes.Add(new Node(NODE_TYPES.SHOP));
        }
         for(int i = 0; i < _eventCount; i++){
            DecisionTree _decision = new DecisionTree("THIS IS AN EVENT BOET! BUT WHAT WILL YOU CHOOSE?", new Decision("option1", EVENT_OUTCOME.BATTLE), new Decision("Option2", EVENT_OUTCOME.NONE), null);
            nodes.Add(new Node(NODE_TYPES.EVENT,_decision));
        }
         for(int i = 0; i < _battleCount; i++){
            nodes.Add(new Node(NODE_TYPES.BATTLE));
        }
        return nodes;
    }

    public void GenerateNodes(GameObject _nodePrefab,List<Vector3> spawnPoints, Transform parent){
        //randomise the list of _nodes
        this._nodes = RandomizeNodes();
        //spawn the nodes _objects
        for(int i = 0; i < spawnPoints.Count; i++){
            _nodePrefab.name = i.ToString();
            GameObject node = (GameObject)Object.Instantiate(_nodePrefab, spawnPoints[i], new Quaternion(0, 0, 0, 0), parent);
            node.name = i.ToString();
            _objects.Add(node);
            _nodes[i]._gameObject = node;
        }
    }

}
