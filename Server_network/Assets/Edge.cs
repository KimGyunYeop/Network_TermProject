using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour {
    GameObject car;
	// Use this for initialization
	void Start () {
        car = GameObject.Find("ServerCar0");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerEnter2D(Collider2D others)
    {
        if (others.gameObject.tag == "Car_main")
        {
            car.GetComponent<ServerCar>().isContect = 1;
        }
    }
}
