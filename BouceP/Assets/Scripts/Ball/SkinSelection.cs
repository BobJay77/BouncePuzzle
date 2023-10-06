using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelection : MonoBehaviour
{
    private int         maxSize;    
    private GameObject  skinObjectLoad;
    private GameObject  skinObjectCopy;

    [SerializeField] private Button     closeSkinSelection;
    [SerializeField] private GameObject lockSkinIcon;
    [SerializeField] private int        currentSkinIndex    = 0;

    [SerializeField] private AudioSource[] audioSource;
    [SerializeField] private AudioSource ballSFXSource;

    private void OnEnable()
    {
        // Find loop audio sources in Audio Manager
        audioSource = GameObject.FindGameObjectWithTag("AudioManager").GetComponents<AudioSource>();

        if (audioSource.Length >= 3)
        {
            // Access the ball Audio Source (index 2 in the array)
            ballSFXSource = audioSource[2];
        }

        maxSize = GameSystem.instance.AccountSettings.Skins.Count;
        currentSkinIndex = 0;

        // Iterate through the List of skins to find the current one being used
        foreach (Skin skin in GameSystem.instance.AccountSettings.Skins)
        {
            
            // Last skin selected
            if (skin.projectileVfx == GameSystem.instance.AccountSettings.ActiveSkin.projectileVfx)
            {
                skinObjectLoad = (GameObject)Resources.Load("Prefabs/Projectiles/" + GameSystem.instance.AccountSettings.ActiveSkin.projectileVfx);
                skinObjectCopy = GameSystem.instance.SpawnPrefab(skinObjectLoad, GameObject.FindGameObjectWithTag("Ball").transform.position);
                skinObjectCopy.transform.SetParent(GameObject.FindGameObjectWithTag("Ball").transform);

                // Update UI for locked/unlocked skins
                if (skin.isLocked)
                {
                    lockSkinIcon.SetActive(true);
                    closeSkinSelection.interactable = false;
                }
                else
                {
                    lockSkinIcon.SetActive(false);
                    closeSkinSelection.interactable = true;
                    GameSystem.instance.AccountSettings.ActiveSkin = GameSystem.instance.AccountSettings.Skins[currentSkinIndex];
                }

                break;
            }

            currentSkinIndex++;
            GameSystem.instance.CurrentSkinIndex = currentSkinIndex;
        }

        // Play Active skin SFX
        PlaySkinLoopSFX(currentSkinIndex);
        PlayOneShotSkinSFX(currentSkinIndex, AudioManager.instance.muzzlesSFX);
    }

    // View next skin in the list
    public void NextSkin()
    {
        // Destroy previously viewed skin in the scene
        Destroy(skinObjectCopy);

        // Error checking the index for the list of skins
        currentSkinIndex = currentSkinIndex < maxSize - 1 ? currentSkinIndex + 1 : 0;

        // Load and instantiate next skin
        skinObjectLoad = (GameObject)Resources.Load("Prefabs/Projectiles/" + GameSystem.instance.AccountSettings.Skins[currentSkinIndex].projectileVfx);
        skinObjectCopy = GameSystem.instance.SpawnPrefab(skinObjectLoad, GameObject.FindGameObjectWithTag("Ball").transform.position);
        skinObjectCopy.transform.SetParent(GameObject.FindGameObjectWithTag("Ball").transform);

        // Check if skin is unlocked
        if (GameSystem.instance.AccountSettings.Skins[currentSkinIndex].isLocked)
        {
            lockSkinIcon.SetActive(true);
            closeSkinSelection.interactable = false;
        }
        else
        {
            lockSkinIcon.SetActive(false);
            closeSkinSelection.interactable = true;
            GameSystem.instance.AccountSettings.ActiveSkin = GameSystem.instance.AccountSettings.Skins[currentSkinIndex];
        }

        // Play skin SFX
        PlaySkinLoopSFX(currentSkinIndex);
        PlayOneShotSkinSFX(currentSkinIndex, AudioManager.instance.muzzlesSFX);
    }

    // View previous skin in the list
    public void PrevSkin()
    {
        // Destroy previously viewed skin in the scene
        Destroy(skinObjectCopy);

        // Error checking the index for the list of skins
        currentSkinIndex = currentSkinIndex > 0 ? currentSkinIndex - 1 : maxSize - 1;

        // Load and instantiate next skin
        skinObjectLoad = (GameObject)Resources.Load("Prefabs/Projectiles/" + GameSystem.instance.AccountSettings.Skins[currentSkinIndex].projectileVfx);
        skinObjectCopy = GameSystem.instance.SpawnPrefab(skinObjectLoad, GameObject.FindGameObjectWithTag("Ball").transform.position);
        skinObjectCopy.transform.SetParent(GameObject.FindGameObjectWithTag("Ball").transform);

        // Check if skin is unlocked
        if (GameSystem.instance.AccountSettings.Skins[currentSkinIndex].isLocked)
        {
            lockSkinIcon.SetActive(true);
            closeSkinSelection.interactable = false;
        }
        else
        {
            lockSkinIcon.SetActive(false);
            closeSkinSelection.interactable = true;
            GameSystem.instance.AccountSettings.ActiveSkin = GameSystem.instance.AccountSettings.Skins[currentSkinIndex];
        }

        // Play skin SFX
        PlaySkinLoopSFX(currentSkinIndex);
        PlayOneShotSkinSFX(currentSkinIndex, AudioManager.instance.muzzlesSFX);
    }
    

    public void PlayOneShotSkinSFX(int currentSkinIndex, AudioCollection collection)
    {
        AudioClip clip = collection[currentSkinIndex];

        if (clip == null) return;

        AudioManager.instance.PlayOneShotSound(collection.audioGroup,
                                               clip,
                                               Camera.main.transform.position,
                                               collection.volume,
                                               collection.spatialBlend,
                                               collection.priority);
    }

    public void PlaySkinLoopSFX(int currentSkinIndex)
    {
        ballSFXSource.clip = AudioManager.instance.projectilesLoopSFX[currentSkinIndex];

        if (ballSFXSource.clip == null) return;

        ballSFXSource.Play();
    }

    public void StopSkinLoopSFX()
    {
        ballSFXSource.Stop();
    }

    public void UpdateScreenSkin()
    {
        // Destroy viewed skin 
        Destroy(skinObjectCopy);    
        Destroy(GameSystem.instance.projectilePrefabSceneCopy);

        skinObjectLoad = (GameObject)Resources.Load("Prefabs/Projectiles/" + GameSystem.instance.AccountSettings.ActiveSkin.projectileVfx);

        // Load chosen skin
        GameSystem.instance.loadedProjectilePrefab = skinObjectLoad;
        GameSystem.instance.loadedMuzzlePrefab = (GameObject)Resources.Load("Prefabs/Muzzles/" + GameSystem.instance.AccountSettings.ActiveSkin.muzzleVfx); 
        GameSystem.instance.loadedHitPrefab = (GameObject)Resources.Load("Prefabs/Hits/" + GameSystem.instance.AccountSettings.ActiveSkin.hitVfx);

        // Save chosen skin
        GameSystem.instance.DataService.SaveData<AccountSettings>("/acc.json", GameSystem.instance.AccountSettings, GameSystem.instance.EncryptionEnabled);

        GameSystem.instance.projectilePrefabSceneCopy = GameSystem.instance.SpawnPrefab(skinObjectLoad, GameObject.FindGameObjectWithTag("Ball").transform.position);
        GameSystem.instance.projectilePrefabSceneCopy.transform.SetParent(GameObject.FindGameObjectWithTag("Ball").transform);

    }
}
