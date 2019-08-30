using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum NODE_TYPES{
    SHOP,
    EVENT,
    BATTLE
}

public class Node
{
    public GameObject _gameObject;
    public NODE_TYPES _nodeType;
    public bool _visited;

    public List<Node> _neighbours;

    public Node(NODE_TYPES type){
        _nodeType = type;
    }

    public Node(Node newNode){
         _nodeType = newNode._nodeType;
         _gameObject = newNode._gameObject;
         _visited = newNode._visited;
    }

}