using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Endzone : MonoBehaviour {
    GameObject client;
	// Use this for initialization
	void Start () {
        client = GameObject.Find("Client");
	}
	
	// Update is called once per frame
	void Update () {
    }
    void OnTriggerEnter2D(Collider2D others) //만약 게임 오브젝트가 부딪히면
    {
        if (others.gameObject.tag == "Car") //차가 부딪히면 차를 사라지게 한다.
        {
            Debug.Log("EndOthers");
            others.gameObject.SetActive(false);
        }
        if(others.gameObject.tag == "Car_main") //자신의 차가 부딪히면, 클라이언트에게 보고한다.
        {
            Debug.Log("End");
            client.GetComponent<Client>().isEnd = true;
        }
    }
}
