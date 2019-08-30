using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InformationScript : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Find("waterStock").GetComponent<Text>().text = player.GetComponent<PlayerScript>().water.ToString();
        transform.Find("foodStock").GetComponent<Text>().text = player.GetComponent<PlayerScript>().food.ToString();
        transform.Find("cannonballStock").GetComponent<Text>().text = player.GetComponent<PlayerScript>().cannonballs.ToString();     
        transform.Find("crewMembers").GetComponent<Text>().text = player.GetComponent<PlayerScript>().crewmembers.ToString();
    }
}
