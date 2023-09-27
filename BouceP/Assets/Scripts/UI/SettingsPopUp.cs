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

    private float prevMasterSliderValue;
    private float prevMusicSliderValue;
    private float prevSFXSliderValue;

    [SerializeField] private Button restartButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button resumeButton;

    [SerializeField] private bool isMainMenu;


    private void Awake()
    {
        isMainMenu = SceneManager.GetActiveScene().name == "MainMenu";
        prevMasterSliderValue = masterSlider.value;
        prevMusicSliderValue = musicSlider.value;
        prevSFXSliderValue = SFXSlider.value;
    }

    private void Start()
    {
        masterSlider.value = GameSystem.instance.AccountSettings.masterVolume;
        musicSlider.value = GameSystem.instance.AccountSettings.musicVolume;
        SFXSlider.value = GameSystem.instance.AccountSettings.SceneVolume;
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

        GameSystem.instance.DataService.SaveData<AccountSettings>("/acc.json", GameSystem.instance.AccountSettings, true);
    }

    private void SetMasterVolume()
    {
        if(prevMasterSliderValue != masterSlider.value)
        {
            prevMasterSliderValue = masterSlider.value;

            AudioManager.instance.SetTrackVolume("Master", masterSlider.value);

            GameSystem.instance.AccountSettings.masterVolume = masterSlider.value;
        }
    }

    private void SetMusicVolume()
    {
        if (prevMusicSliderValue != musicSlider.value)
        {
            prevMusicSliderValue = musicSlider.value;

            AudioManager.instance.SetTrackVolume("Music", musicSlider.value);

            GameSystem.instance.AccountSettings.musicVolume = musicSlider.value;
        }
    }

    private void SetSFXVolume()
    {
        if (prevSFXSliderValue != SFXSlider.value)
        {
            prevSFXSliderValue = SFXSlider.value;

            AudioManager.instance.SetTrackVolume("UI", SFXSlider.value);
            AudioManager.instance.SetTrackVolume("Scene", SFXSlider.value);
            AudioManager.instance.SetTrackVolume("Ball", SFXSlider.value);

            GameSystem.instance.AccountSettings.UIVolume = SFXSlider.value;
            GameSystem.instance.AccountSettings.SceneVolume = SFXSlider.value;
            GameSystem.instance.AccountSettings.BallVolume = SFXSlider.value;
        }
    }

}
