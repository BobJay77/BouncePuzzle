using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Information")]
public class LevelInfo : ScriptableObject
{
    public int  levelID             = 0;    // ID to indicate what level it is
    public int  numOfBouncesToWin   = 1;    // Number of bounces needed to complete a level
    public bool locked              = true; // Determine is level is locked
    public int  unlockSkinOnLevel   = 0;    // Set to 0 if no skin is unlocked at from the level
    public int  backgroundMusicIndex;

    // List of game states related to the level
    [SerializeField] private List<GameState> _startingGameStates = new List<GameState>(); 

    // Properties
    public List<GameState> StartingGameStates
    {
        get { return _startingGameStates; }
        set { _startingGameStates = value; }
    }
}
