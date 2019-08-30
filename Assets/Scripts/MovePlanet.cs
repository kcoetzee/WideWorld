using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlanet : MonoBehaviour
{

    public Transform _planet;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
 
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
 
    public float distanceMin = 3.0f;
    public float distanceMax = 11.0f;
 
    public Rigidbody rigidbody;
 
    float x = 0.0f;
    float y = 0.0f;


    private bool rotate = false;
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
 
        rigidbody = GetComponent<Rigidbody>();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            rotate = true;
        if (Input.GetMouseButtonUp(0))
            rotate = false;
    }

    void LateUpdate () 
    {
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        if (_planet && rotate) 
        {
            x += Input.GetAxis("Mouse X") * xSpeed * distance * Time.deltaTime;
            y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            RaycastHit hit;
            if (Physics.Linecast (_planet.position, transform.position, out hit)) 
            {
                distance -=  hit.distance;
            }

            Vector3 position = rotation * negDistance + _planet.position;
 
            transform.rotation = rotation;
            transform.position = position;
        }
        
        // distance = distance - Input.GetAxis("Mouse ScrollWheel")*3;
      
        distance = distanceMin;
     
    }

}
