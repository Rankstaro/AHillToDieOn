using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Transports.UNET;
using MLAPI.Messaging;
using System;

public class StartMenu : NetworkBehaviour
{
    public GameObject startMenu;
    public GameObject hostMenu;
    public GameObject joinMenu;
    public GameObject settingsMenu;
    public GameObject quitMenu;
    public GameObject menuCamera;
    public GameObject title;
    public GameObject menuWorld;
    public GameObject prefabWorld;
    public string ipAddress = "127.0.0.1";
    public string password = "";
    UNetTransport transport;

    private void ApprovalCheck(byte[] connectionData, ulong clientID, NetworkManager.ConnectionApprovedDelegate callback)
    {
        bool approve = System.Text.Encoding.ASCII.GetString(connectionData) == password;
        callback(true, null, approve, new Vector3(0, 8, 0), Quaternion.identity);
    }

    public void HostGame()
    {
        startMenu.SetActive(false);
        hostMenu.SetActive(true);
    }

    public void JoinGame()
    {
        startMenu.SetActive(false);
        joinMenu.SetActive(true);
    }

    public void Settings()
    {
        startMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void QuitGame()
    {
        startMenu.SetActive(false);
        quitMenu.SetActive(true);
    }

    public void Host()
    {
        hostMenu.SetActive(false);
        menuCamera.SetActive(false);
        title.SetActive(false);
        menuWorld.SetActive(false);
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost(new Vector3(0, 8, 0), Quaternion.identity);
        GameObject worldInstance = Instantiate(prefabWorld, Vector3.zero, Quaternion.identity);
        worldInstance.GetComponent<NetworkObject>().Spawn();
        PauseMenu.Resume();
    }

    public void HostBack()
    {
        startMenu.SetActive(true);
        hostMenu.SetActive(false);
    }

    public void ChangeIP(string newIPAddress)
    {
        this.ipAddress = newIPAddress;
    }

    public void SetPassword(string pass)
    {
        this.password = pass;
    }

    public void Join()
    {
        transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        transport.ConnectAddress = ipAddress;
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(password);
        NetworkManager.Singleton.StartClient();
        joinMenu.SetActive(false);
        menuCamera.SetActive(false);
        menuWorld.SetActive(false);
        title.SetActive(false);
        PauseMenu.Resume();
    }

    public void JoinBack()
    {
        startMenu.SetActive(true);
        joinMenu.SetActive(false);
    }

    public void SettingsBack()
    {
        startMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void QuitBack()
    {
        startMenu.SetActive(true);
        quitMenu.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
