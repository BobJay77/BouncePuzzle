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

    private void OnEnable()
    {
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
        }
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
