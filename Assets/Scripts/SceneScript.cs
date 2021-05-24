using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneScript : NetworkBehaviour
{
    public TMP_Text _canvasStatusText;
    public PlayerScript _playerScript;
    public SceneReference sceneReference;

    [SyncVar(hook = nameof(OnStatusTextChanged))]
    public string _statusText;

    void OnStatusTextChanged(string _Old, string _New)
    {
        //called from sync var hook, to update info on screen for all players
        _canvasStatusText.text = _statusText;
    }

    public void ButtonSendMessage()
    {
        if (_playerScript != null)  
        {
            _playerScript.CmdSendPlayerMessage();
        }
    }
    
    public void ButtonChangeScene()
    {
        if (isServer)
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name == "MyScene") { NetworkManager.singleton.ServerChangeScene("MyOtherScene"); }

            else { NetworkManager.singleton.ServerChangeScene("MyScene"); }
        }
        else { Debug.Log("You are not Host."); }
    }
   
}
