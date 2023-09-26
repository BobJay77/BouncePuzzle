using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Information")]
public class LevelInfo : ScriptableObject
{
    public int  levelID             = 0;
    public int  numOfBouncesToWin   = 1;
    public bool locked              = true;

    [SerializeField] private List<GameState> _startingGameStates = new List<GameState>();

    // Public property to access _startingGameStates
    public List<GameState> StartingGameStates
    {
        get { return _startingGameStates; }
        set { _startingGameStates = value; }
    }
}
