using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLevelInfoHolder : MonoBehaviour
{
    //public LevelInfo LevelInfo;
    [SerializeField] private int levelInfoIndex;
    [SerializeField] GameObject lockImage;
    [SerializeField] GameObject numberImage;

    private void Update()
    {
        if(lockImage != null) lockImage.SetActive(GameSystem.instance.LevelInfos[levelInfoIndex].locked);
        if(numberImage != null) numberImage.SetActive(!GameSystem.instance.LevelInfos[levelInfoIndex].locked);

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

    public void ButtonClicked(bool mainmenu = false)
    {
        if (mainmenu)
            SceneSwitcher.instance.LoadGameLevel(0);

        else
        {
            GameSystem.instance.CurrentLevelInfo = GameSystem.instance.LevelInfos[levelInfoIndex];  
            SceneSwitcher.instance.LoadGameLevel(GameSystem.instance.LevelInfos[levelInfoIndex].levelID);
        }
    }

    public void NextLevel()
    {
        GameSystem.instance.CurrentLevelInfo = GameSystem.instance.LevelInfos[levelInfoIndex];
        SceneSwitcher.instance.LoadNextGameLevel();
    }

}
