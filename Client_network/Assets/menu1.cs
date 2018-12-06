using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu1 : MonoBehaviour {

    public string LevelToLoad;

    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }
}
