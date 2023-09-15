using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Information")]
public class LevelInfo : ScriptableObject
{
    public int levelID = 0;
    public int numOfBouncesToWin = 1;

    [SerializeField] private List<GameState> _startingGameStates = new List<GameState>();
}
