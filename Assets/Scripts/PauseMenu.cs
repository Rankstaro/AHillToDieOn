using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Transports.UNET;

public class PauseMenu : NetworkBehaviour
{
    public GameObject startMenu;
    public GameObject settingsMenu;
    public GameObject pauseMenu;
    public GameObject quitMenu;
    public GameObject menuCamera;
    public GameObject title;
    public GameObject menuWorld;
    public static GameObject pauseMenuStatic;
    [System.NonSerialized]
    public static bool paused = false;

    private void Start()
    {
        pauseMenuStatic = pauseMenu;
    }

    public static void Pause()
    {
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenuStatic.SetActive(true);
    }

    public static void Resume()
    {
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenuStatic.SetActive(false);
    }

    public void Settings()
    {
        pauseMenuStatic.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void QuitSession()
    {
        pauseMenuStatic.SetActive(false);
        quitMenu.SetActive(true);
    }

    public void SettingsBack()
    {
        pauseMenuStatic.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void QuitBack()
    {
        pauseMenuStatic.SetActive(true);
        quitMenu.SetActive(false);
    }

    public void Quit()
    {
        if (IsHost)
        {
            NetworkManager.Singleton.StopHost();
        }
        else if (IsClient)
        {
            NetworkManager.Singleton.StopClient();
        }
        quitMenu.SetActive(false);
        menuCamera.SetActive(true);
        menuWorld.SetActive(true);
        title.SetActive(true);
        startMenu.SetActive(true);
    }
}
