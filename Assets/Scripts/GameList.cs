using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameList : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("Menu");
    }
}
