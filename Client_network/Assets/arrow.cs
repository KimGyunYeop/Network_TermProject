using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrow : MonoBehaviour {
    GameObject camera;
	// Use this for initialization
	void Start () {
        this.camera = GameObject.Find("Main Camera");
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(this.camera.transform.position.x - this.transform.position.x, this.camera.transform.position.y - this.transform.position.y, 0);
    }
}
