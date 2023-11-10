using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Account Settings")]
public class AccountSettings : ScriptableObject
{
    public float masterVolume = 5;  // All game volume
    public float musicVolume = 5;   // BGM volume
    public float UIVolume = 5;      // UI sounds volume
    public float SceneVolume = 5;   // Scene feedback volume
    public float BallVolume = 5;    // Skin volume

    [SerializeField] private List<Skin> _skins;         // List of all available skins in the game
    [SerializeField] private Skin       _activeSkin;    // Current active skin that is being used
    [SerializeField] private int        _latestLevelUnlocked = 1;
    [SerializeField] private bool       _noAds = false;

    // Properties
    public List<Skin> Skins
    {
        get { return _skins; }
        set { _skins = value; }
    }
    public Skin ActiveSkin
    {
        get { return _activeSkin; }
        set { _activeSkin = value; }
    }
    public bool NoAds
    {
        get { return _noAds; }
        set { _noAds = value; }
    }

    public int LatestLevelUnlocked
    {
        get { return _latestLevelUnlocked; }
        set { _latestLevelUnlocked = value; }
    }
}
