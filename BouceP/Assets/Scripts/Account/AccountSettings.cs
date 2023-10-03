using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Account Settings")]
public class AccountSettings : ScriptableObject
{
    public float masterVolume = 5;
    public float musicVolume = 5;
    public float UIVolume = 5;
    public float SceneVolume = 5;
    public float BallVolume = 5;

    [SerializeField] private List<Skin> _skins;
    [SerializeField] private Skin       _activeSkin;

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


}
