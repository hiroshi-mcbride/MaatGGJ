using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    #region FMOD_events
    [Header("Music")]
    [FMODUnity.EventRef] public string explorationMusicRef = "";
    [FMODUnity.EventRef] public string stairsMusicRef = "";
    [FMODUnity.EventRef] public string ascendMusicRef = "";
    [FMODUnity.EventRef] public string unlockMusicRef = "";
    [FMODUnity.EventRef] public string endingMusicRef = "";

    [Header("SFX")]
    [FMODUnity.EventRef] public string stairsTurningSFXRef = "";
    [FMODUnity.EventRef] public string interactSFXRef = "";
    [FMODUnity.EventRef] public string pickupSFXRef = "";

    private EventInstance explorationMusic;
    private EventInstance stairsMusic;
    private EventInstance ascendMusic;
    private EventInstance unlockMusic;
    private EventInstance endingMusic;

    [HideInInspector] public EventInstance stairsTurningSFX;
    [HideInInspector] public EventInstance interactSFX;
    [HideInInspector] public EventInstance pickupSFX;

    private Dictionary<AudioType, EventInstance> musicEvents;
    private Dictionary<AudioType, EventInstance> sfxEvents;
    #endregion

    bool coroutineRunning = false;
    bool inOrOut;
    bool musicIsPlaying = false;
    bool StairsFinished
    {
        get { return stairsFinished; }
        set
        {
            if (!stairsFinished && value)
            {

                EventManager.RaiseEvent(EventType.STAIRS_MUSIC_FINISHED);
                stairsFinished = value;
            }
        }
    }
    bool stairsFinished = false;
    int stairsPlayCount = 0;
    bool exploreIsPlaying = false;
    IEnumerator coroutine;

    FMOD.Studio.PARAMETER_ID fadeParameterId;
    float faderLevel = 1.0f;

    public bool bankLoaded = false;

    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (instance != this)
        {
            Destroy(gameObject);
        }
        
        
    }

    private IEnumerator Start()
    {

        yield return new WaitUntil(() => FMODUnity.RuntimeManager.HasBankLoaded("Master"));
        bankLoaded = true;
        
        explorationMusic = FMODUnity.RuntimeManager.CreateInstance(explorationMusicRef);
        stairsMusic = FMODUnity.RuntimeManager.CreateInstance(stairsMusicRef);
        ascendMusic = FMODUnity.RuntimeManager.CreateInstance(ascendMusicRef);
        unlockMusic = FMODUnity.RuntimeManager.CreateInstance(unlockMusicRef);
        endingMusic = FMODUnity.RuntimeManager.CreateInstance(endingMusicRef);

        stairsTurningSFX = FMODUnity.RuntimeManager.CreateInstance(stairsTurningSFXRef);
        interactSFX = FMODUnity.RuntimeManager.CreateInstance(interactSFXRef);
        pickupSFX = FMODUnity.RuntimeManager.CreateInstance(pickupSFXRef);

        musicEvents = new Dictionary<AudioType, EventInstance>()
        {
            {AudioType.MUSIC_EXPLORE, explorationMusic },
            {AudioType.MUSIC_STAIRS, stairsMusic },
            {AudioType.MUSIC_ASCEND, ascendMusic },
            {AudioType.MUSIC_UNLOCK, unlockMusic },
            {AudioType.MUSIC_ENDING, endingMusic }
        };

        sfxEvents = new Dictionary<AudioType, EventInstance>()
        {
            {AudioType.SFX_STAIRS_TURN, stairsTurningSFX },
            {AudioType.SFX_PICKUP, pickupSFX },
            {AudioType.SFX_INTERACT, interactSFX }
        };

        //PlayMusic(AudioType.MUSIC_EXPLORE);



        FMOD.Studio.EventDescription fadeEventDescription;
        musicEvents[AudioType.MUSIC_EXPLORE].getDescription(out fadeEventDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION fadeParameterDescription;
        fadeEventDescription.getParameterDescriptionByName("VolumeFade", out fadeParameterDescription);
        fadeParameterId = fadeParameterDescription.id;
        float currentFadeValue;
        musicEvents[AudioType.MUSIC_EXPLORE].getParameterByID(fadeParameterId, out currentFadeValue);
        
    }

    private void Update()
    {
        if (bankLoaded)
        {
            musicEvents[AudioType.MUSIC_EXPLORE].setParameterByID(fadeParameterId, faderLevel);
            
            if (stairsPlayCount > 0)
            {
                FMOD.Studio.PLAYBACK_STATE state;
                stairsMusic.getPlaybackState(out state);
                if (state == PLAYBACK_STATE.STOPPED)
                {
                    StairsFinished = true;
                }
            }
        }
    }

    public void StartFade(AudioType _track, bool _in)
    {

        if (!IsPlaying(_track))
        {
            if (_in && !IsPlaying(AudioType.MUSIC_ASCEND) && !IsPlaying(AudioType.MUSIC_ENDING) && !IsPlaying(AudioType.MUSIC_STAIRS))
            {
                PlayMusic(_track);
            }
            return;
        }



        coroutine = FadeMusic(_track, _in);

        if ((IsPlaying(AudioType.MUSIC_ASCEND) || IsPlaying(AudioType.MUSIC_ENDING) || IsPlaying(AudioType.MUSIC_STAIRS)) && _in)
        {
            Debug.Log("here!");
            if(coroutineRunning)
            {
                StopCoroutine(coroutine);
                coroutineRunning = false;
            }
            coroutine = FadeMusic(AudioType.MUSIC_EXPLORE, false);
            StartCoroutine(coroutine);
            return;
        }

        if (_in != inOrOut && coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutineRunning = false;
        }
        inOrOut = _in;

        if (coroutineRunning)
        {
            //Debug.Log("coroutine already running");
            return;
        }
        
        StartCoroutine(coroutine);
    }

    public void PlaySFX(AudioType _track)
    {
        sfxEvents[_track].start();
    }

    public void PlayMusic(AudioType _track)
    {
        if (!IsPlaying(_track))
        {
            if (_track != AudioType.MUSIC_EXPLORE && exploreIsPlaying)
            {
                StartFade(AudioType.MUSIC_EXPLORE, false);
            }

            if(_track == AudioType.MUSIC_STAIRS)
            {
                if (stairsPlayCount > 0)
                {
                    return;
                }
                stairsPlayCount++;
                //StairsPlayed = true;
            }

            if(_track == AudioType.MUSIC_ASCEND || _track == AudioType.MUSIC_ENDING)
            {
                StopMusic(AudioType.MUSIC_STAIRS);
                StopMusic(AudioType.MUSIC_EXPLORE);
            }

            musicEvents[_track].start();
        }
        Debug.Log(IsPlaying(AudioType.MUSIC_STAIRS));
    }

    public void StopMusic(AudioType _track)
    {
        if (IsPlaying(_track))
        {
            musicEvents[_track].stop(STOP_MODE.ALLOWFADEOUT);
        }
    }
    public void StopSFX(AudioType _track)
    {
        if (IsPlaying(_track))
        {
            sfxEvents[_track].stop(STOP_MODE.IMMEDIATE);
        }
    }

    private bool IsPlaying(AudioType _track)
    {
        PLAYBACK_STATE playbackState = PLAYBACK_STATE.STOPPED;
        if (musicEvents.ContainsKey(_track))
        {
            musicEvents[_track].getPlaybackState(out playbackState);
        }
        else if (sfxEvents.ContainsKey(_track))
        {
            sfxEvents[_track].getPlaybackState(out playbackState);
        }


        if (playbackState == PLAYBACK_STATE.PLAYING)
        {
            Debug.Log("selected audio already playing");
            return true;
        }
        return false;
    }

    private IEnumerator FadeMusic(AudioType _track, bool _in)
    {
        if (coroutineRunning)
        {
            coroutineRunning = false;
            yield break;
        }
        coroutineRunning = true;

        PLAYBACK_STATE playbackState;
        musicEvents[_track].getPlaybackState(out playbackState);

        if (playbackState == PLAYBACK_STATE.STOPPED)
        {
            Debug.Log("selected audio not playing");
            yield break;
        }

        
        

        float fadeLength = 2.7f;

        if (_in)
        {
            exploreIsPlaying = true;
            while(faderLevel<1f)
            {
                faderLevel += 0.01f / fadeLength;
                yield return new WaitForSeconds(0.01f);
            }
            
        }
        else if (!_in)
        {
            while (faderLevel > 0f)
            {
                faderLevel -= 0.01f / fadeLength;
                yield return new WaitForSeconds(0.01f);
            }
            exploreIsPlaying = false;
        }

        coroutineRunning = false;
    }

}