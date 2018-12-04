using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handler : MonoBehaviour {
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.A))
        {
            if (transform.rotation.z < 0.90f)
            {
                this.transform.Rotate(0, 0, 1f); }
            //회전
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (transform.rotation.z >= -0.90f)
                transform.Rotate(0, 0, -1f);//회전
        }
        else
        {
            if (this.transform.rotation.z != 0)
            {
                if (this.transform.rotation.z > 0)
                    this.transform.Rotate(0, 0, -2.5f);
                else
                    this.transform.Rotate(0, 0, 2.5f);
            }
        }
    }
}
