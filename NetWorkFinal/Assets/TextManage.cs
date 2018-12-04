using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManage : MonoBehaviour {
    public Text text;
    // Use this for initialization
    void Start () {
        Debug.Log(GameObject.Find("Grade").GetComponent<Grade>().grade);
        text.text = GameObject.Find("Grade").GetComponent<Grade>().grade.ToString();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
