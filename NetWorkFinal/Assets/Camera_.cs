using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_ : MonoBehaviour
{

    GameObject car;
    float rotation;
    // Use this for initialization
    void Start()
    {
        this.car = GameObject.Find("car_main");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(this.car.transform.position.x - this.transform.position.x, this.car.transform.position.y - this.transform.position.y, 0);
    }
}
