using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLevelInfoHolder : MonoBehaviour
{
    //public LevelInfo LevelInfo;
    [SerializeField] private int    levelInfoIndex;
    [SerializeField] GameObject     lockImage;
    [SerializeField] GameObject     numberImage;
    [SerializeField] GameObject     starImage;
    [SerializeField] Sprite         starLocked;
    [SerializeField] Sprite         starUnlocked;
    [SerializeField] bool           isALevelButton = false;

    private void Update()
    {
        int maxLevelInfoIndex = GameSystem.instance.LevelInfos.Count;

        if (maxLevelInfoIndex > levelInfoIndex)
        {
            if (lockImage != null) lockImage.SetActive(GameSystem.instance.LevelInfos[levelInfoIndex].locked);
            if (numberImage != null) numberImage.SetActive(!GameSystem.instance.LevelInfos[levelInfoIndex].locked);
            

            if (isALevelButton)
            {
                if (!GameSystem.instance.LevelInfos[levelInfoIndex].locked) starImage.SetActive(true);
                var stars = GameSystem.instance.LevelInfos[levelInfoIndex].stars;
                Image[] images = this.GetComponentsInChildren<Image>(); // stars are index 2, 3, 4

                if (images != null)
                {
                    Debug.Log(stars);
                    // At least 1 star has been earned
                    if (stars > 0)
                    {
                        if (stars == 1)
                        {
                            if(images[2].sprite != starUnlocked)
                            {
                                images[2].sprite = starUnlocked;
                                images[3].sprite = starLocked;
                                images[4].sprite = starLocked;
                            }
                        }
                        else if (stars == 2)
                        {
                            if (images[2].sprite != starUnlocked && images[3].sprite != starUnlocked)
                            {
                                images[2].sprite = starUnlocked;
                                images[3].sprite = starUnlocked;
                                images[4].sprite = starLocked;
                            }
                        }
                        else if (stars == 3)
                        {
                            if (images[2].sprite != starUnlocked && images[3].sprite != starUnlocked && images[4].sprite != starUnlocked)
                            {
                                images[2].sprite = starUnlocked;
                                images[3].sprite = starUnlocked;
                                images[4].sprite = starUnlocked;
                            }
                        }
                    }
                }
                
            }

            if (GameSystem.instance.LevelInfos.Count >= GameSystem.instance.LevelInfos[levelInfoIndex].levelID)
            {
                if (GameSystem.instance.LevelInfos[levelInfoIndex].locked)
                {
                    GetComponent<Button>().interactable = false;
                }
                else
                {
                    GetComponent<Button>().interactable = true;
                }
            }
        }
        else
        {
            levelInfoIndex = maxLevelInfoIndex - 1;

        }
    }
    public void ButtonClicked(bool mainmenu = false)
    {
        // Loading level
        if (mainmenu)
        {
            SceneSwitcher.instance.LoadGameLevel(0);
        }
        else
        {
            GameSystem.instance.CurrentLevelInfo = GameSystem.instance.LevelInfos[levelInfoIndex];  
            LeanTween.moveLocalY(gameObject, 5000, 0.5f).setEase(LeanTweenType.easeInOutQuint).setOnComplete(delegate() { SceneSwitcher.instance.LoadGameLevel(GameSystem.instance.LevelInfos[levelInfoIndex].levelID);
            });
        }
    }

    // Player last unlocked level
    public void PlayButton() 
    {
        LevelInfo nextLevel = null;

        foreach (var level in GameSystem.instance.LevelInfos)
        {
            if (!level.locked)
            {
                nextLevel = level;
            }
            else
                break;
        }
        GameSystem.instance.CurrentLevelInfo = nextLevel;
        SceneSwitcher.instance.LoadGameLevel(nextLevel.levelID);
    }

    public void NextLevel()
    {
        GameSystem.instance.CurrentLevelInfo = GameSystem.instance.LevelInfos[GameSystem.instance.CurrentLevelInfo.levelID];
        SceneSwitcher.instance.LoadNextGameLevel();
    }

    public void PlayButtonClickSound(AudioClip clip)
    {
        AudioManager.instance.GetComponents<AudioSource>()[1].PlayOneShot(clip);
    }

    public void RestartGame()
    {
        //Adding to the ad counter whenever we exit winlose state
        AdMobAds.instance.LoadInterstitialAd();

        GameSystem.instance.DeleteBallsInGameScene();
        GameObject.FindGameObjectWithTag("Goal").GetComponent<Animator>().SetBool("activated", false);
        GameSystem.instance.SetState(GameSystem.instance.startGameState);

        #region Star Handling
        GameObject[] starObjects = GameObject.FindGameObjectsWithTag("Star");

        // Iterate through the found objects and access a component (e.g., Collider)
        foreach (GameObject starObject in starObjects)
        {
            Star star = starObject.GetComponent<Star>();

            // Check if the component exists before using it
            if (star != null)
            {
                star.Restart();
            }
        }
        #endregion
    }

    public void ResetBlackHoleShot()
    {
        GameSystem.instance.blackHoleShot = false;
    }
}
