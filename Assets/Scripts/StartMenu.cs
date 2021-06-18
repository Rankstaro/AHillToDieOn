using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class StartMenu : MonoBehaviour
{
    public GameObject startMenu;

    public void host()
    {
        NetworkManager.Singleton.StartHost();
        startMenu.SetActive(false);
    }

    public void join()
    {
        NetworkManager.Singleton.StartClient();
        startMenu.SetActive(false);
    }

    public void settings()
    {
        // Open settings menu
        startMenu.SetActive(false);
    }

    public void quit()
    {
        Application.Quit();
    }
}
