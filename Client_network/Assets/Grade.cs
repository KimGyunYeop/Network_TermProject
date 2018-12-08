using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grade : MonoBehaviour {
    public int grade;
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject); //점수는 씬이 바꿔어도 그대로 남아있도록 하기위해
    }
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	}
}
