using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class BK_GameState : MonoBehaviour
{
    #region Variables

    // Represents the status of the game
    public GameStatus CurrentGameStatus { get; private set; }

    // Represents whether or not the game is paused or not
    private bool isPaused = false;
    public bool IsPaused { get { return isPaused; } }

    private float gameScore = 0f;
    private float gameTime = 5f;

    // Here we can have UnityEvents that fire when our game changes to specific states. Other
    // objects can then listen to these events and execute code when they're invoked.
    // You can also use the native C# "Action" type for events, but UnityEvents can also be configured in the Editor
    public UnityEvent OnGamePaused;
    public UnityEvent OnGameResumed;
    public UnityEvent OnGameInitializing;
    public UnityEvent OnGameInProgress;
    public UnityEvent OnPlayerLost;

    public UnityEvent<float> OnScoreChanged;
    public UnityEvent<float> OnTimerChanged;

    public UnityEvent OnTimeExpired;

    // Static (global) reference to the single existing instance of the object
    private static BK_GameState _instance = null;

    // Public property to allow access to the Singleton instance
    // A property is a member that provides a flexible mechanism to read, write, or compute the value of a data field.
    public static BK_GameState Instance
    {
        get { return _instance; }
    }

    #endregion

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

        // Reset the GameState to default values
        ResetGameState();

        // We want to make sure that if we switch levels (e.g. to the main menu), clean up after ourselves.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // As always, when we subscribe to events we must also remember to unsubscribe to clean up after ourselves
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    #region Custom Functions

    // Pause the game
    public void PauseGame()
    {
        isPaused = true;
        OnGamePaused?.Invoke();
    }

    // Resume the game
    public void ResumeGame()
    {
        isPaused = false;
        OnGameResumed?.Invoke();
    }

    /// <summary>
    /// Public function to set the GameStatus. Should only be called by the GameManager.
    /// </summary>
    /// <param name="newGameStatus"></param>
    /// <returns>True if the status successfully changed.</returns>
    public bool UpdateGameStatus(GameStatus newGameStatus)
    {
        // Return false if the status did not change
        if (CurrentGameStatus == newGameStatus) { return false; }

        // Update our game status and invoke a corresponding event so objects listening for the event will execute relevant code
        CurrentGameStatus = newGameStatus;
        switch (newGameStatus)
        {
            case GameStatus.GameInitializing:
                OnGameInitializing?.Invoke();
                break;
            case GameStatus.InProgress:
                OnGameInProgress?.Invoke();
                break;
            case GameStatus.TimeExpired:
                OnTimeExpired?.Invoke();
                break;
            case GameStatus.PlayerLost:
                OnPlayerLost?.Invoke();
                break;
            default:
                Debug.LogError("Unhandled GameStatus! This shouldn't happen.");
                break;
        }

        // Return true, since we successfully updated our state
        return true;
    }

    public void DeltaGameScore(float scoreDelta)
    {
        gameScore += scoreDelta;
        Debug.Log($"Game score: {gameScore}");

        OnScoreChanged?.Invoke(gameScore);
    }

    public void TickTimer(float deltaTimer)
    {
        gameTime += deltaTimer;
        //Debug.Log($"Game time: {gameTime}");

        OnTimerChanged?.Invoke(gameTime);

        if (gameTime <= 0f)
        {
            gameTime = 0f;

            // Set time expired and end game
            UpdateGameStatus(GameStatus.TimeExpired);
        }
    }

    public void ResetGameState()
    {
        //Debug.Log("Resetting Game State!");

        // Default to InProgress Status and not paused
        CurrentGameStatus = GameStatus.InProgress;
        isPaused = false;
        Time.timeScale = 1f;

        gameScore = 0f;
        gameTime = 5f;

        // Clear all events
        OnGamePaused.RemoveAllListeners();
        OnGameResumed.RemoveAllListeners();
        OnGameInProgress.RemoveAllListeners();
        OnPlayerLost.RemoveAllListeners();
        OnTimeExpired.RemoveAllListeners();
    }

    // Called after awake and start
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetGameState();

        StartCoroutine(DelayInit(scene, mode));
    }

    private IEnumerator DelayInit(Scene scene, LoadSceneMode mode)
    {
        yield return null;
        yield return null;

        // If this is the Main Menu scene, Reset the GameState
        //if (scene.name != BK_Globals.MainMenuSceneName)
        //{
            // Start the game
            UpdateGameStatus(GameStatus.GameInitializing);
        //}
    }

    #endregion
}

#region Data Structures

// We can track the state of our game with a handy enum we'll call GameState.
public enum GameStatus
{
    GameInitializing,     // The game is in progress
    InProgress,     // The game is in progress
    TimeExpired,      // The player has won the game
    PlayerLost      // The player has lost the game
}

#endregion