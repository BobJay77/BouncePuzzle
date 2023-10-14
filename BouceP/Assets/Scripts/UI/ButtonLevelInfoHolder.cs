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

    private void Update()
    {
        int maxLevelInfoIndex = GameSystem.instance.LevelInfos.Count;

        if (maxLevelInfoIndex > levelInfoIndex)
        {
            if (lockImage != null) lockImage.SetActive(GameSystem.instance.LevelInfos[levelInfoIndex].locked);
            if (numberImage != null) numberImage.SetActive(!GameSystem.instance.LevelInfos[levelInfoIndex].locked);

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
        GameSystem.instance.DeleteBallsInGameScene(); 
        GameSystem.instance.SetState(GameSystem.instance.startGameState);
    }

    public void ResetBlackHoleShot()
    {
        GameSystem.instance.blackHoleShot = false;
    }
}
