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

        if (loadOperation.isDone && worldSceneIndex > 0)  
        {
            //GameSystem.instance.ghostBall = GameObject.FindGameObjectWithTag("Ghost Ball");
            //GameSystem.instance.ghostBall.SetActive(false);
            //GameSystem.instance.playerBall = GameObject.FindObjectOfType<BallManager>().gameObject;
            GameSystem.instance.winOrLoseParent = GameObject.FindObjectOfType<WinLoseUIParent>().gameObject;
            GameSystem.instance.winOrLoseParent.SetActive(false);
            GameSystem.instance.actionText = GameObject.Find("Action Text").GetComponent<TMP_Text>();
            GameSystem.instance.bouncesText = GameObject.Find("Bounces Required").GetComponent<TMP_Text>();
            GameSystem.instance.StartGameState();
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
