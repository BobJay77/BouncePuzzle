using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SettingsPopUp : MonoBehaviour
{

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;

    [SerializeField] private Button restartButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button resumeButton;

    [SerializeField] private bool isMainMenu;


    private void Awake()
    {
        isMainMenu = SceneManager.GetActiveScene().name == "MainMenu";
    }

    private void Update()
    {
        SetVolumes();
    }

    public void BackButton()
    {
        if (isMainMenu)
        {
            Application.Quit();
        }
    }

    private void SetVolumes()
    {
        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
    }

    private void SetMasterVolume()
    {
        AudioManager.instance.SetTrackVolume("Master", masterSlider.value);
    }

    private void SetMusicVolume()
    {
        //AudioManager.instance.SetTrackVolume("Music", musicSlider.value);
    }

    private void SetSFXVolume()
    {
        //AudioManager.instance.SetTrackVolume("SFX", SFXSlider.value);
    }

}
