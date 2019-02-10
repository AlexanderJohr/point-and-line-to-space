using Prototype.NetworkLobby;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class UiHandler : MonoBehaviour
{

    public void TabScreenToStart()
    {
        GameObject.Find("Canvas").transform.Find("Menu").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("Title").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("TapScreenToStart").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("TabScreenToStartButton").gameObject.SetActive(false);

    }

    public void StartGame()
    {
        SceneManager.LoadScene("Network Lobby");
    }

    public void BackTitle()
    {
        GameObject.Find("Canvas").transform.Find("Menu").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Title").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("TapScreenToStart").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("TabScreenToStartButton").gameObject.SetActive(true);

    }

    public void MenuDisplay(String selectMenuName)
    {
        GameObject.Find("MenuPanel").transform.Find(selectMenuName).gameObject.SetActive(true);
    }

    public void LoadTitleScreen()
    {

        SceneManager.LoadScene("Title Screen");

        LobbyManager existingLobbyManager = GameObject.FindObjectOfType<LobbyManager>();
        if (existingLobbyManager != null)
        {
           existingLobbyManager.backDelegate();
            //Destroy(existingLobbyManager.gameObject);
        }

       // NetworkManager.Shutdown();

    }
}
