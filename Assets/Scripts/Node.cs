using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum NODE_TYPES
{
    SHOP,
    EVENT,
    BATTLE
}

public class Node
{
    public GameObject _gameObject;
    public NODE_TYPES _nodeType;
    public bool _visited;
    public GameObject _planet;

    public DecisionTree _tree;

    public List<Node> _neighbours;

    public Node(NODE_TYPES type)
    {
        _nodeType = type;
    }

    public Node(NODE_TYPES type, DecisionTree tree)
    {
        _nodeType = type;
        _tree = tree;
    }

    public Node(Node newNode)
    {
        _nodeType = newNode._nodeType;
        _gameObject = newNode._gameObject;
        _visited = newNode._visited;
        _tree = newNode._tree;
    }

    public void ExecuteNode(ref BattleManagerScript _battleScript, ref GameObject _battlePanel, ref GameObject _battleManager, ref GameObject _eventPanel, ref GameObject PanelDesc, ref GameObject PanelOption1, ref GameObject PanelOption2)
    {
        _visited = true;
        if (_nodeType == NODE_TYPES.BATTLE)
        {
            _battleManager.SetActive(true);
            _battlePanel.SetActive(true);

            _battleScript.StartBattle();
            //freezeplanet movement;
            _gameObject.GetComponent<MeshRenderer>().material.color = Color.magenta;

        }
        else if (_nodeType == NODE_TYPES.EVENT)
        {
            _gameObject.GetComponent<MeshRenderer>().material.color = Color.magenta;


            //  transform.Find("waterStock").GetComponent<Text>().text = player.GetComponent<PlayerScript>().water.ToString();
            _eventPanel.SetActive(true);
            PanelDesc.transform.GetComponent<Text>().text = _tree._eventDesciption;
            PanelOption1.transform.GetComponent<Text>().text = _tree._option1._optionText;
            PanelOption2.transform.GetComponent<Text>().text = _tree._option2._optionText;
            // _eventPanel.transform.GetComponent<Description>().GetComponent<Text>().text = _tree._eventDesciption;
            // _eventPanel.GetComponent<Option1>().text = _tree._option1._optionText;
            // _eventPanel.GetComponent<Option2>().text = _tree._option2._optionText;

            ;
        }
    }

}