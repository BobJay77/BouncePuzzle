using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public class GameState
{
    public string Key = null;
    public string Value = null;
}

public class GameSystem : StateMachine
{
    public static GameSystem instance = null;

    // VFX
    public GameObject loadedProjectilePrefab;
    public GameObject loadedMuzzlePrefab;
    public GameObject loadedHitPrefab;
    public GameObject projectilePrefabSceneCopy;
    public GameObject muzzlePrefabSceneCopy;
    public GameObject hitPrefabSceneCopy;

    public GameObject ghostBall;
    public GameObject ghostBallSceneCopy;
    public float speedMultiplier;

    // UI variables
    public GameObject winOrLoseParent;
    public GameObject actionTextParent;
    public TMP_Text actionText;
    public TMP_Text bouncesText;
    public int currentBounces;
    public int bouncesGoal;

    // Game States
    public StartGame startGameState;
    public PlayerTurn playerTurnState;
    public Resolution resolutionState;
    public WinLose winLoseState;


    // Audio Collections
    //public AudioCollection winLoseSounds = null;

    [HideInInspector] public Vector3 mousePosition;

    [HideInInspector] public bool roundEnded = false;
    [HideInInspector] public bool hitGoal = false;


    [SerializeField] private List<GameState> _startingGameStates = new List<GameState>();
    [SerializeField] public List<LevelInfo> _levelInfos = new List<LevelInfo>();
    [SerializeField] private LevelInfo _currentLevelInfo;
    [SerializeField] private AccountSettings _accountSettings;
    [SerializeField] private bool _encryptionEnabled;
    [SerializeField] private int _currentSkinIndex;

    // Game states
    private Dictionary<string, string> _gameStateDictionary = new Dictionary<string, string>();

    // Serializing
    private IDataService _dataService = new JSONDataService();

    // Properties
    public AccountSettings AccountSettings
    {
        get
        {
            return _accountSettings;
        }
        set
        {
            _accountSettings = value;
        }
    }
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
    public bool EncryptionEnabled
    {
        get
        {
            return _encryptionEnabled;
        }
        set
        {
            _encryptionEnabled = value;
        }
    }
    public int CurrentSkinIndex
    {
        get
        {
            return _currentSkinIndex;
        }
        set
        {
            _currentSkinIndex = value;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // Initialize the game states
        if (startGameState == null)
            startGameState = new StartGame(instance);

        if (playerTurnState == null)
            playerTurnState = new PlayerTurn(instance);

        if (resolutionState == null)
            resolutionState = new Resolution(instance);

        if (winLoseState == null)
            winLoseState = new WinLose(instance);

        // Accessing data or writing new one if it is the first time
        try
        {
            //Load previous data
            _levelInfos = _dataService.LoadData<List<LevelInfo>>("/levels.json", _encryptionEnabled);
            AccountSettings = _dataService.LoadData<AccountSettings>("/acc.json", _encryptionEnabled);
        }
        catch (Exception e)
        {
            Debug.Log($"Could not read file.");

            //First time saving
            AccountSettings.ActiveSkin = AccountSettings.Skins[0];
            _dataService.SaveData<List<LevelInfo>>("/levels.json", _levelInfos, _encryptionEnabled);
            _dataService.SaveData<AccountSettings>("/acc.json", AccountSettings, _encryptionEnabled);
        }
    }

    private void Start()
    {
        SpawnProjectileInScene();

        CurrentSkinIndex = 0;

        // Iterate through the List of skins to find the current one being used
        foreach (Skin skin in GameSystem.instance.AccountSettings.Skins)
        {
            // Last skin selected
            if (skin.projectileVfx == GameSystem.instance.AccountSettings.ActiveSkin.projectileVfx)
            {
                break;
            }

            CurrentSkinIndex++;
        }
    }

    public GameObject SpawnPrefab(GameObject prefab, Vector3 pos)
    {
        return Instantiate(prefab, pos, Quaternion.identity) as GameObject;
    }

    public void DestroyPrefab(GameObject prefab)
    {
        Destroy(prefab);
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

    public void DeleteBallsInGameScene()
    {
       DestroyPrefab(projectilePrefabSceneCopy);
       DestroyPrefab(ghostBallSceneCopy);
    }

    public void SpawnProjectileInScene()
    {
        // Load last active skin from Resources folder
        loadedProjectilePrefab = (GameObject)Resources.Load("Prefabs/Projectiles/" + AccountSettings.ActiveSkin.projectileVfx);
        loadedMuzzlePrefab = (GameObject)Resources.Load("Prefabs/Muzzles/" + AccountSettings.ActiveSkin.muzzleVfx);
        loadedHitPrefab = (GameObject)Resources.Load("Prefabs/Hits/" + AccountSettings.ActiveSkin.hitVfx);

        // Copy to scene prefab
        projectilePrefabSceneCopy = SpawnPrefab(loadedProjectilePrefab, GameObject.FindGameObjectWithTag("Ball").transform.position);
        projectilePrefabSceneCopy.transform.SetParent(GameObject.FindGameObjectWithTag("Ball").transform);
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

    public void TriggerVFX(GameObject vfx)
    {
        if (vfx != null)
        {
            var _VFX = Instantiate(vfx, projectilePrefabSceneCopy.transform.position, Quaternion.identity);
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