using Prototype.NetworkLobby;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class UiHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject _tutorialScreen;

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
    }

    public void NewMatch()
    {
        LobbyManager existingLobbyManager = GameObject.FindObjectOfType<LobbyManager>();
        if (existingLobbyManager != null)
        {
            existingLobbyManager.SendReturnToLobby();
        }
    }

    public void OpenTutorial()
    {
        _tutorialScreen.SetActive(true);
    }

    public void CloseTutorial()
    {
        _tutorialScreen.SetActive(false);
    }
}
