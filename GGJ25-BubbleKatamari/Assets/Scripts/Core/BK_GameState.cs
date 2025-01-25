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

    // The name of the Main Menu scene, so we know if we loaded into it
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    // Here we can have UnityEvents that fire when our game changes to specific states. Other
    // objects can then listen to these events and execute code when they're invoked.
    // You can also use the native C# "Action" type for events, but UnityEvents can also be configured in the Editor
    public UnityEvent OnGamePaused;
    public UnityEvent OnGameResumed;
    public UnityEvent OnGamePlaying;
    public UnityEvent OnPlayerWon;
    public UnityEvent OnPlayerLost;

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
            case GameStatus.InProgress:
                OnGamePlaying?.Invoke();
                break;
            case GameStatus.PlayerWon:
                OnPlayerWon?.Invoke();
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

    public void ResetGameState()
    {
        //Debug.Log("Resetting Game State!");

        // Default to Paused Status
        CurrentGameStatus = GameStatus.InProgress;
        isPaused = true;

        // Clear all events
        OnGamePaused.RemoveAllListeners();
        OnGameResumed.RemoveAllListeners();
        OnPlayerWon.RemoveAllListeners();
        OnPlayerLost.RemoveAllListeners();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If this is the Main Menu scene, Reset the GameState
        if (scene.name == mainMenuSceneName)
        {
            ResetGameState();
        }
    }

    #endregion
}

#region Data Structures

// We can track the state of our game with a handy enum we'll call GameState.
public enum GameStatus
{
    InProgress,     // The game is in progress
    PlayerWon,      // The player has won the game
    PlayerLost      // The player has lost the game
}

#endregion