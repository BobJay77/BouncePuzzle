using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;

public class SceneSwitcher : MonoBehaviour
{
    public static SceneSwitcher instance;

    [SerializeField] int worldSceneIndex = 1;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }   
    }

    public IEnumerator LoadWorldScene()
    {
        //IF YOU JUST WANT 1 WORLD SCENE USE THIS
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        if (loadOperation.isDone)  
        {
            // Not main menu switch
            if (worldSceneIndex > 0)
            {
                GameSystem.instance.winOrLoseParent = GameObject.FindObjectOfType<WinLoseUIParent>().gameObject;
                GameSystem.instance.winOrLoseParent.SetActive(false);

                if (GameSystem.instance.CurrentLevelInfo.hasTutorial)
                {
                    Debug.Log(GameSystem.instance.CurrentLevelInfo.levelID);
                    GameSystem.instance.actionText = GameObject.Find("Action Text").GetComponent<TMP_Text>();
                    GameSystem.instance.actionTextParent = GameSystem.instance.actionText.transform.parent.gameObject;
                    GameSystem.instance.actionTextParent.SetActive(false);
                }
                GameSystem.instance.bouncesText = GameObject.Find("Bounces Required Text").GetComponent<TMP_Text>();

                // Play bgm music
                var bgm = AudioManager.instance.bgmSource;

                bgm.clip = AudioManager.instance.backgroundMusic[GameSystem.instance.CurrentLevelInfo.backgroundMusicIndex];

                if (bgm.clip != null)
                    bgm.Play();

                GameSystem.instance.StartGameState();
            }
            // Main menu switch
            else
            {
                // Play bgm music
                var bgm = AudioManager.instance.bgmSource;

                bgm.clip = AudioManager.instance.backgroundMusic[0];

                if (bgm.clip != null)
                    bgm.Play();

                // Play bgm music
                var projectileLoopSource = AudioManager.instance.ballSFXSource;

                projectileLoopSource.Stop();

                GameSystem.instance.SpawnProjectileInScene();
            }
           
        }

        yield return null;
    }

    public void LoadGameLevel(int index)
    {
        worldSceneIndex = index;

        StartCoroutine(LoadWorldScene());

       // return null;
    }

    public void LoadNextGameLevel()
    {
        worldSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if(worldSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            worldSceneIndex = 0;
        }

        StartCoroutine(LoadWorldScene());
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
