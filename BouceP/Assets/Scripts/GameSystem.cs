using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public string Key = null;
    public string Value = null;
}

public class GameSystem : StateMachine
{
    public static GameSystem instance = null;
    
    public GameObject   playerBall;
    public GameObject   ghostBall;
    public GameObject   barrierBall;
    public GameObject   winOrLoseParent;

    public TMP_Text     actionText;
    public TMP_Text     bouncesText;
    public LevelInfo    levelInfo;
    public int          currentBounces;
    public int          bouncesGoal;

    //Game States
    public StartGame    startGameState;
    public PlayerTurn   playerTurnState;
    public Resolution   resolutionState;
    public WinLose      winLoseState;

    [HideInInspector]   public Vector3  mousePosition;
    [HideInInspector]   public bool     roundEnded = false;

    [SerializeField] public     float           multiplier;

    [SerializeField] private    List<GameState> _startingGameStates     = new List<GameState>();            

    private Dictionary<string, string>          _gameStateDictionary    = new Dictionary<string, string>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;

        else
            Destroy(gameObject);

        startGameState  = new StartGame(instance);
        playerTurnState = new PlayerTurn(instance);
        resolutionState = new Resolution(instance);
        winLoseState    = new WinLose(instance);
    }

    private void Start()
    {
        SetState(startGameState);
    }

    private void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        State.OnUpdate();
    }

    void ResetGameStates()
    {
        _gameStateDictionary.Clear();

        // Copy starting game states into game state dictionary
        for (int i = 0; i < _startingGameStates.Count; i++)
        {
            GameState gs = _startingGameStates[i];
            _gameStateDictionary[gs.Key] = gs.Value;
        }
    }

    public bool AreStatesSet(List<GameState> requiredStates)
    {

        // Assume the states are all set and then loop to find a state to disprove this
        for (int i = 0; i < requiredStates.Count; i++)
        {
            GameState state = requiredStates[i];

            // Check if the current state exist in the dictionary
            string result = GetGameState(state.Key);
            if (string.IsNullOrEmpty(result) || !result.Equals(state.Value)) return false;
        }

        return true;
    }

    public bool SetGameState(string key, string value)
    {
        if (key == null || value == null) return false;

        _gameStateDictionary[key] = value;


        return true;
    }

    public string GetGameState(string key)
    {
        string result = null;
        _gameStateDictionary.TryGetValue(key, out result);
        return result;
    }

    public void TurnOnNextButton(bool on)
    {
        winOrLoseParent.SetActive(true);
       
        if (on)
        {
            winOrLoseParent.GetComponent<WinLoseUIParent>().next.gameObject.SetActive(true);
        }
    } 
}