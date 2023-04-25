using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    private Transform destination;
    //is it the first vent to travel thru
    public bool isStart;
    public float distance = 0.2f;
    public Collider2D vent;


    // Start is called before the first frame update
    void Start()
    {
        if (isStart == false)
        {
            destination = GameObject.FindGameObjectWithTag("start portal").GetComponent<Transform>();
        }
        else
        {
            destination = GameObject.FindGameObjectWithTag("end portal").GetComponent<Transform>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D vent)
    {
        if (Vector2.Distance(transform.position, vent.transform.position) > distance)
        {
            vent.transform.position = new Vector2(destination.position.x, destination.position.y);
        }
    }
}
