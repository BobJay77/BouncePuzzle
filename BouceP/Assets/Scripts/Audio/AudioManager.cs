using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TrackInfo
{
    public string           Name        = string.Empty;
    public AudioMixerGroup  Group       = null;
    public IEnumerator      TrackFader  = null;
}

public class AudioPoolItem
{
    public GameObject   GameObject      = null;
    public Transform    Transform       = null;
    public AudioSource  AudioSource     = null;
    public float        Unimportance    = float.MaxValue;
    public bool         Playing         = false;
    public IEnumerator  Coroutine       = null;
    public ulong        ID              = 0;
}

public class AudioManager : MonoBehaviour
{
    // Statics
    private static AudioManager _instance = null;
    public  static AudioManager instance
    {
        get
        {
            if (_instance == null)
                _instance = (AudioManager)FindObjectOfType(typeof(AudioManager));
            return _instance;
        }
    }

    // Audio Collections
    public AudioCollection winLoseSounds        = null;
    public AudioCollection bounceSounds         = null;
    public AudioCollection swipeSounds          = null;
    public AudioCollection projectilesLoopSFX   = null;
    public AudioCollection muzzlesSFX           = null;
    public AudioCollection hitsSFX              = null;



    // Inspector Assigned Variables
    [SerializeField] AudioMixer _mixer      = null;
    [SerializeField] int        _maxSounds  = 10;

    // Private Variables
    Dictionary<string, TrackInfo>       _tracks         = new Dictionary<string, TrackInfo>();
    List<AudioPoolItem>                 _pool           = new List<AudioPoolItem>();
    Dictionary<ulong, AudioPoolItem>    _activePool     = new Dictionary<ulong, AudioPoolItem>();
    List<LayeredAudioSource>            _layeredAudio   = new List<LayeredAudioSource>();
    ulong                               _idGiver        = 0;
    Transform _listenerPos = null;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (_instance == null)
            _instance = this;

        else
            Destroy(gameObject);

        if (!_mixer) return;

        // Get all the groups in the mixer
        AudioMixerGroup[] groups = _mixer.FindMatchingGroups(string.Empty);

        // Create our mixer tracks based on group name (Track -> AudioGroup)
        foreach (AudioMixerGroup group in groups)
        {
            TrackInfo trackInfo = new TrackInfo();
            trackInfo.Name = group.name;
            trackInfo.Group = group;
            trackInfo.TrackFader = null;
            _tracks[group.name] = trackInfo;
        }

        // Generate Pool
        for (int i = 0; i < _maxSounds; i++)
        {
            // Create GameObject and assigned AudioSource and Parent
            GameObject go = new GameObject("Pool Item");
            AudioSource audioSource = go.AddComponent<AudioSource>();
            go.transform.parent = transform;

            // Create and configure Pool Item
            AudioPoolItem poolItem = new AudioPoolItem();
            poolItem.GameObject = go;
            poolItem.AudioSource = audioSource;
            poolItem.Transform = go.transform;
            poolItem.Playing = false;
            go.SetActive(false);
            _pool.Add(poolItem);

        }

        
    }

    private void Start()
    {
        //Set volume from the loaded account settings 
        SetTrackVolume("Master", GameSystem.instance.AccountSettings.masterVolume);
        SetTrackVolume("Music", GameSystem.instance.AccountSettings.musicVolume);
        SetTrackVolume("Scene", GameSystem.instance.AccountSettings.SceneVolume);
        SetTrackVolume("Ball", GameSystem.instance.AccountSettings.BallVolume);
        SetTrackVolume("UI", GameSystem.instance.AccountSettings.UIVolume);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    // A new scene has been loaded, now find the listener
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _listenerPos = FindObjectOfType<AudioListener>().transform;
    }

    void Update()
    {
        // Update any layered audio sources
        foreach (LayeredAudioSource las in _layeredAudio)
        {
            if (las != null) las.Update();
        }
    }

    public float GetTrackVolume(string track)
    {
        TrackInfo trackInfo;
        if (_tracks.TryGetValue(track, out trackInfo))
        {
            float volume;
            _mixer.GetFloat(track, out volume);
            return volume;
        }

        return float.MinValue;
    }

    public AudioMixerGroup GetAudioGroupFromTrackName(string name)
    {
        TrackInfo ti;
        if (_tracks.TryGetValue(name, out ti))
        {
            return ti.Group;
        }

        return null;
    }

    public void SetTrackVolume(string track, float volume, float fadeTime = 0.0f)
    {
        if (!_mixer) return;
        TrackInfo trackInfo;
        if (_tracks.TryGetValue(track, out trackInfo))
        {
            // Stop any coroutine that might be in the middle of fading this track
            if (trackInfo.TrackFader != null) StopCoroutine(trackInfo.TrackFader);

            if (fadeTime == 0.0f)
                _mixer.SetFloat(track, Mathf.Log10(volume) * 20);
            else
            {
                trackInfo.TrackFader = SetTrackVolumeInternal(track, volume, fadeTime);
                StartCoroutine(trackInfo.TrackFader);
            }
        }
    }

    //  Implement a fade between volumes of a track over time
    protected IEnumerator SetTrackVolumeInternal(string track, float volume, float fadeTime)
    {
        float startVolume = 0.0f;
        float timer = 0.0f;
        _mixer.GetFloat(track, out startVolume);

        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            _mixer.SetFloat(track, Mathf.Lerp(startVolume, volume, timer / fadeTime));
            yield return null;
        }

        _mixer.SetFloat(track, volume);
    }

    protected ulong ConfigurePoolObject(int poolIndex, string track, AudioClip clip, Vector3 position, float volume, float spatialBlend, float unimportance, float startTime)
    {
        // If poolIndex is out of range abort request
        if (poolIndex < 0 || poolIndex >= _pool.Count) return 0;

        // Get the pool item
        AudioPoolItem poolItem = _pool[poolIndex];

        // Generate new ID so we can stop it later if we want to
        _idGiver++;

        // Configure the audio source's position and colume
        AudioSource source = poolItem.AudioSource;
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = spatialBlend;


        // Assign to requested audio group/track
        source.outputAudioMixerGroup = _tracks[track].Group;

        // Position source at requested position
        source.transform.position = position;

        // Enable GameObject and record that it is now playing
        poolItem.Playing = true;
        poolItem.Unimportance = unimportance;
        poolItem.ID = _idGiver;
        source.time = Mathf.Min(startTime, source.clip.length);
        poolItem.GameObject.SetActive(true);
        source.Play();
        poolItem.Coroutine = StopSoundDelayed(_idGiver, source.clip.length);
        StartCoroutine(poolItem.Coroutine);

        // Add this sound to our active pool with its unique id
        _activePool[_idGiver] = poolItem;

        // Return the id to the caller
        return _idGiver;
    }

    // Stop a one shot sound from playing after a number of seconds
    protected IEnumerator StopSoundDelayed(ulong id, float duration)
    {
        yield return new WaitForSeconds(duration);
        AudioPoolItem activeSound;

        // If this if exists in our active pool
        if (_activePool.TryGetValue(id, out activeSound))
        {
            activeSound.AudioSource.Stop();
            activeSound.AudioSource.clip = null;
            activeSound.GameObject.SetActive(false);
            _activePool.Remove(id);

            // Make it available again
            activeSound.Playing = false;
        }
    }

    // Stop Sound with the passed One Shot ID
    public void StopSound(ulong id)
    {
        AudioPoolItem activeSound;
        if (_activePool.TryGetValue(id, out activeSound))
        {
            activeSound.AudioSource.Stop();
            activeSound.AudioSource.clip = null;
            activeSound.GameObject.SetActive(false);
            _activePool.Remove(id);

            // Make it available again
            activeSound.Playing = false;

        }
    }


    public ulong PlayOneShotSound(string track, AudioClip clip, Vector3 position, float volume, float spatialBlend, int priority = 128, float startTime = 0.0f)
    {
        // Do nothing if track does not exist, clip is null or volume is zero
        if (!_tracks.ContainsKey(track) || clip == null || volume.Equals(0.0f)) return 0;

        float unimportance = (_listenerPos.position - position).sqrMagnitude / Mathf.Max(1, priority);

        int leastImportantIndex = -1;
        float leastImportanceValue = float.MaxValue;

        // Find an available audio source to use
        for (int i = 0; i < _pool.Count; i++)
        {
            AudioPoolItem poolItem = _pool[i];

            // Is this source available
            if (!poolItem.Playing)
                return ConfigurePoolObject(i, track, clip, position, volume, spatialBlend, unimportance, startTime);
            else
            // We have a pool item that is less important than the one we are going to play
            if (poolItem.Unimportance > leastImportanceValue)
            {
                // Record the least important sound we have found so far
                // as a candidate to relace with our new sound request
                leastImportanceValue = poolItem.Unimportance;
                leastImportantIndex = i;
            }
        }

        // If we get here all sounds are being used but we know the least important sound currently being
        // played so if it is less important than our sound request then use replace it
        if (leastImportanceValue > unimportance)
            return ConfigurePoolObject(leastImportantIndex, track, clip, position, volume, spatialBlend, unimportance, startTime);


        // Could not be played (no sound in the pool available)
        return 0;
    }

    public IEnumerator PlayOneShotSoundDelayed(string track, AudioClip clip, Vector3 position, float volume, float spatialBlend, float duration, int priority = 128)
    {
        yield return new WaitForSeconds(duration);
        PlayOneShotSound(track, clip, position, volume, spatialBlend, priority);
    }

    public ILayeredAudioSource RegisterLayeredAudioSource(AudioSource source, int layers)
    {
        if (source != null && layers > 0)
        {
            // First check it doesn't exist already and if so just return the source
            for (int i = 0; i < _layeredAudio.Count; i++)
            {
                LayeredAudioSource item = _layeredAudio[i];
                if (item != null)
                {
                    if (item.audioSource == source)
                    {
                        return item;
                    }
                }
            }

            // Create a new layered audio item and add it to the managed list
            LayeredAudioSource newLayeredAudio = new LayeredAudioSource(source, layers);
            _layeredAudio.Add(newLayeredAudio);

            return newLayeredAudio;
        }

        return null;
    }


    public void UnregisterLayeredAudioSource(ILayeredAudioSource source)
    {
        _layeredAudio.Remove((LayeredAudioSource)source);
    }

    public void UnregisterLayeredAudioSource(AudioSource source)
    {
        for (int i = 0; i < _layeredAudio.Count; i++)
        {
            LayeredAudioSource item = _layeredAudio[i];
            if (item != null)
            {
                if (item.audioSource == source)
                {
                    _layeredAudio.Remove(item);
                    return;
                }
            }
        }
    }
}