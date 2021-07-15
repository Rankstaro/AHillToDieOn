using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public GameObject gameplayMenu;
    public GameObject controlsMenu;
    public GameObject graphicsMenu;
    public GameObject videoMenu;
    public GameObject audioMenu;

    private void Start()
    {
        DeactivateAll();
        gameplayMenu.SetActive(true);
    }

    public void DeactivateAll()
    {
        gameplayMenu.SetActive(false);
        controlsMenu.SetActive(false);
        graphicsMenu.SetActive(false);
        videoMenu.SetActive(false);
        audioMenu.SetActive(false);
    }

    public void GameplayMenu()
    {
        DeactivateAll();
        gameplayMenu.SetActive(true);
    }

    public void ControlsMenu()
    {
        DeactivateAll();
        controlsMenu.SetActive(true);
    }

    public void GraphicsMenu()
    {
        DeactivateAll();
        graphicsMenu.SetActive(true);
    }

    public void VideoMenu()
    {
        DeactivateAll();
        videoMenu.SetActive(true);
    }

    public void AudioMenu()
    {
        DeactivateAll();
        audioMenu.SetActive(true);
    }
}
