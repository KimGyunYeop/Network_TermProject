using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class back : MonoBehaviour {

    public string LevelToLoad;

    public void LoadMain()
    {
        SceneManager.LoadScene("Menu");
    }
}
