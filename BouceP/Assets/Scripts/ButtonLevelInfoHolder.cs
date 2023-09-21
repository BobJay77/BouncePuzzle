using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLevelInfoHolder : MonoBehaviour
{
    public LevelInfo LevelInfo;

    public void ButtonClicked(bool mainmenu = false)
    {
        if (mainmenu)
            SceneSwitcher.instance.LoadGameLevel(0);

        else
        {
            GameSystem.instance.levelInfo = LevelInfo;  
            SceneSwitcher.instance.LoadGameLevel(LevelInfo.levelID);
        }
    }

    public void NextLevel()
    {
        GameSystem.instance.levelInfo = LevelInfo;
        SceneSwitcher.instance.LoadNextGameLevel();
    }

}
