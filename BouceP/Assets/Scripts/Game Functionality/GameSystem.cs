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
    public int          currentBounces;
    public int          bouncesGoal;
    public float        speedMultiplier;

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
    [HideInInspector]   public bool     roundEnded      = false;
    [HideInInspector]   public bool     hitGoal         = false;


    [SerializeField] private    List<GameState> _startingGameStates     = new List<GameState>();
    [SerializeField] public    List<LevelInfo> _levelInfos             = new List<LevelInfo>();
    [SerializeField] private    LevelInfo       _currentLevelInfo;
    [SerializeField] private    bool            _encryptionEnabled;

    private Dictionary<string, string>          _gameStateDictionary    = new Dictionary<string, string>();
    private IDataService                        _dataService            = new JSONDataService();
    
    //Accessors
    public List<LevelInfo> LevelInfos
    {
        get
        {
            return _levelInfos;
        }
        set
        {
            _levelInfos = value;
        }
    }
    public LevelInfo CurrentLevelInfo
    {
        get
        {
            return _currentLevelInfo;
        }
        set
        {
            _currentLevelInfo = value;
        }
    }
    public IDataService DataService
    {
        get
        {
            return _dataService;
        }
        set
        {
            _dataService = value;
        }
    }

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
        try
        {
            _levelInfos = _dataService.LoadData<List<LevelInfo>>("/levels.json", _encryptionEnabled);
        }
        catch (Exception e)
        {
            Debug.LogError($"Could not read file.");

            _dataService.SaveData<List<LevelInfo>>("/levels.json", _levelInfos, _encryptionEnabled);
        }
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
            _dataService.SaveData("/level_stats.json", _levelInfos[0], _encryptionEnabled);
            _dataService.LoadData<LevelInfo>("/level_stats.json", _encryptionEnabled);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
           // _dataService.SaveData("/sessioncount.json", Unity.player_session_count, _encryptionEnabled);
            _dataService.LoadData<LevelInfo>("/sessioncount.json", _encryptionEnabled);
        }
    }

    private void FixedUpdate()
    {
        if (State != null)
            State.OnFixedUpdate();
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

    //public void SerializeJson(LevelInfo levelInfo, string pathName)
    //{
    //    //long startTime = DateTime.Now.Ticks;

    //    if (_dataService.SaveData($"/{pathName}.json", levelInfo, _encryptionEnabled))
    //    {
    //        //_saveTime = DateTime.Now.Ticks - startTime;
    //        ////actionText.text = ($"Save Time: {(_saveTime / 10000f) :N4}ms"); // CHange to save text

    //        //startTime = DateTime.Now.Ticks;
    //        try
    //        {
    //            LevelInfo data = ScriptableObject.CreateInstance<LevelInfo>();
    //            data = _dataService.LoadData<LevelInfo>($"/{pathName}.json", _encryptionEnabled);
    //            //_loadTime = DateTime.Now.Ticks - startTime;
    //            //actionText.text = "Loaded from file:\r\n" + JsonConvert.SerializeObject(data, Formatting.Indented); // CHange to load text
    //            //actionText.text = ($"Load Time: {(_loadTime / 10000f):N4}ms"); // CHange to load text
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogError($"Could nor read file.");
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("Could not save file.");

    //    }
    //}

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