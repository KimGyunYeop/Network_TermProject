using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu2 : MonoBehaviour {

    public string LevelToLoad;

    public void LoadGame()
    {
        SceneManager.LoadScene("howTo");
    }
}
