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
        GameSystem.instance.SetState(GameSystem.instance.startGameState);
    }
}
