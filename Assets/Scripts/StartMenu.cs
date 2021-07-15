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
    public GameObject settingsBackMenu;
    public GameObject settingsMenu;
    public GameObject quitMenu;
    public GameObject menuCamera;
    public GameObject title;
    public GameObject menuWorld;
    public GameObject worldGeneratorPrefab;
    public AudioSource music;
    public string ipAddress = "127.0.0.1";
    public int port = 7777;
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
        settingsBackMenu.SetActive(true);
        settingsMenu.SetActive(true);
    }

    public void QuitGame()
    {
        startMenu.SetActive(false);
        quitMenu.SetActive(true);
    }

    public void Host()
    {
        transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        transport.ConnectAddress = ipAddress;
        transport.ConnectPort = port;
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        hostMenu.SetActive(false);
        menuCamera.SetActive(false);
        title.SetActive(false);
        menuWorld.SetActive(false);
        music.mute = true;
        PauseMenu.Resume();

        GameObject worldGenerator = Instantiate(worldGeneratorPrefab);
        NetworkManager.Singleton.StartHost(new Vector3(0, 5, 0), Quaternion.identity);
    }

    public void HostBack()
    {
        startMenu.SetActive(true);
        hostMenu.SetActive(false);
    }

    public void SetIP(string newIPAddress)
    {
        this.ipAddress = newIPAddress;
    }

    public void SetPassword(string newPassword)
    {
        this.password = newPassword;
    }
    public void SetPort(string newPort)
    {
        this.port = int.Parse(newPort);
    }

    public void Join()
    {
        transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        transport.ConnectAddress = ipAddress;
        transport.ConnectPort = port;
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(password);
        NetworkManager.Singleton.StartClient();

        joinMenu.SetActive(false);
        menuCamera.SetActive(false);
        menuWorld.SetActive(false);
        title.SetActive(false);
        music.mute = true;
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
        settingsBackMenu.SetActive(false);
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
