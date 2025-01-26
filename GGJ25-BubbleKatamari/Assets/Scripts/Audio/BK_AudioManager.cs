using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class BK_AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource bgmAudioSource;

    [SerializeField] private Vector2 volume = new Vector2(0.5f, 0.9f);
    [SerializeField] private Vector2 pitch = new Vector2(0.8f, 1.2f);

    [SerializeField] private List<AudioClip> bubblePopSounds;

    // Static (global) reference to the single existing instance of the object
    private static BK_AudioManager _instance = null;

    // Public property to allow access to the Singleton instance
    // A property is a member that provides a flexible mechanism to read, write, or compute the value of a data field.
    public static BK_AudioManager Instance
    {
        get { return _instance; }
    }

    #region Unity Functions

    private void Awake()
    {
        #region Singleton

        // If an instance of the object does not already exist
        if (_instance == null)
        {
            // Make this object the one that _instance points to
            _instance = this;

            // We want this object to persist between scenes, so don't destroy it on load
            DontDestroyOnLoad(gameObject);
        }
        // Otherwise if an instance already exists and it's not this one
        else
        {
            // Destroy this object
            Destroy(gameObject);
        }

        #endregion

        // We want to make sure that if we switch levels (e.g. to the main menu), clean up after ourselves.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // Subscribe our functions to the relevant events in the GameState
        BK_GameState.Instance.OnGamePaused.AddListener(() => PauseAudio(true));
        BK_GameState.Instance.OnGameResumed.AddListener(() => PauseAudio(false));

        bgmAudioSource.ignoreListenerPause = true;
    }

    #endregion

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // TODO: Music blending?
    }

    public void PauseAudio(bool pause)
    {
        // Note: This static boolean pauses ALL AudioSources
        AudioListener.pause = pause;

        // If you want an AudioSource to be
        // able to play sounds when paused
        // (ex: for Pause UI sounds), then you
        // can use "AudioSource.ignoreListenerPause = true;"
        // to bypass this pause value
    }

    // Play a clip from a designated AudioSource
    public void PlaySFXRaw(AudioClip clip)
    {
        if (sfxAudioSource == null)
            return;

        // Randomize volume and pitch
        sfxAudioSource.volume = 1f;
        sfxAudioSource.pitch = 1f;

        // We update the clip 
        sfxAudioSource.clip = clip;
        // We ensure the audio source is stopped before we play it again
        sfxAudioSource.Stop();
        sfxAudioSource.Play();
    }

    // Play a clip from a designated AudioSource
    public void PlaySFX(AudioClip clip)
    {
        if (sfxAudioSource == null)
            return;

        // Randomize volume and pitch
        sfxAudioSource.volume = Random.Range(volume.x, volume.y);
        sfxAudioSource.pitch = Random.Range(pitch.x, pitch.y);

        // We update the clip 
        sfxAudioSource.clip = clip;
        // We ensure the audio source is stopped before we play it again
        sfxAudioSource.Stop();
        sfxAudioSource.Play();
    }

    public void PlayOneshot(AudioClip clip)
    {
        sfxAudioSource.PlayOneShot(clip);
    }

    public void PlayBubblePopOneshot()
    {
        if (bubblePopSounds != null && bubblePopSounds.Count > 0)
        {
            sfxAudioSource.PlayOneShot(bubblePopSounds[UnityEngine.Random.Range(0, bubblePopSounds.Count)]);
        }
    }

    public void PlayOneshotAtPosition(AudioClip clip, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip, position, Random.Range(volume.x, volume.y));
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmAudioSource == null)
            return;

        bgmAudioSource.volume = 1f;
        bgmAudioSource.pitch = 1f;

        // We update the clip 
        bgmAudioSource.clip = clip;
        // We ensure the audio source is stopped before we play it again
        bgmAudioSource.Stop();
        bgmAudioSource.Play();
    }
}