using System;
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
    public GameObject   winOrLoseParent;

    public TMP_Text     actionText;
    public TMP_Text     bouncesText;
    public LevelInfo    levelInfo;
    public int          currentBounces;
    public int          bouncesGoal;

    // Game States
    public StartGame    startGameState;
    public PlayerTurn   playerTurnState;
    public Resolution   resolutionState;
    public WinLose      winLoseState;

    // VFX
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;

    // Audio Collections
    public AudioCollection winLoseSounds = null;

    [HideInInspector]   public Vector3  mousePosition;
    [HideInInspector]   public bool     roundEnded = false;
    [HideInInspector]   public bool     hitGoal = false;

    [SerializeField] public     float           multiplier;

    [SerializeField] private    List<GameState> _startingGameStates     = new List<GameState>();            

    private Dictionary<string, string>          _gameStateDictionary    = new Dictionary<string, string>();

    // Save and Load
    private IDataService _dataService = new JSONDataService();
    [SerializeField] public bool _encryptionEnabled;
    private long _saveTime;
    private long _loadTime;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (startGameState == null)
            startGameState = new StartGame(instance);

        if (playerTurnState == null)
            playerTurnState = new PlayerTurn(instance);

        if (resolutionState == null)
            resolutionState = new Resolution(instance);

        if (winLoseState == null)
            winLoseState = new WinLose(instance);
    }

    public void StartGameState()
    {
        SetState(startGameState);
    }

    private void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

        if (State != null)
            State.OnUpdate();

        if (Input.GetKeyDown(KeyCode.J))
        {
            SerializeJson();
        }
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

    public void SerializeJson()
    {
        long startTime = DateTime.Now.Ticks;

        if (_dataService.SaveData("/level-stats.json", levelInfo, _encryptionEnabled))
        {
            _saveTime = DateTime.Now.Ticks - startTime;
            //actionText.text = ($"Save Time: {(_saveTime / 10000f) :N4}ms"); // CHange to save text

            startTime = DateTime.Now.Ticks;
            try
            {
                LevelInfo data = ScriptableObject.CreateInstance<LevelInfo>();
                data = _dataService.LoadData<LevelInfo>("/level-stats.json", _encryptionEnabled);
                _loadTime = DateTime.Now.Ticks - startTime;
                //actionText.text = "Loaded from file:\r\n" + JsonConvert.SerializeObject(data, Formatting.Indented); // CHange to load text
                //actionText.text = ($"Load Time: {(_loadTime / 10000f):N4}ms"); // CHange to load text
            }
            catch (Exception e)
            {
                Debug.LogError($"Could nor read file.");
            }
        }
        else
        {
            Debug.LogError("Could not save file.");

        }
    }

    public void TurnOnNextButton(bool on)
    {
        winOrLoseParent.SetActive(true);
       
        if (on)
        {
            winOrLoseParent.GetComponent<WinLoseUIParent>().next.gameObject.SetActive(true);
        }
    } 

    public void TriggerVFX(GameObject vfx)
    {
        if (vfx != null)
        {
            var _VFX = Instantiate(vfx, playerBall.transform.position, Quaternion.identity);
            //_VFX.transform.forward = playerBall.transform.forward;
            var ps = _VFX.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(_VFX, ps.main.duration);
            else
            {
                var psChild = _VFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(_VFX, psChild.main.duration);
            }
        }
    }
}