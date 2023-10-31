using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WinLoseUIParent : MonoBehaviour
{
    public Button mainMenu;
    public Button restart;
    public Button next;
    public TMP_Text winLoseText;
    public void RestartGame()
    {
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
}
