using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class BK_PauseMenu : MonoBehaviour
{
    #region Variables

    private VisualElement root;  // Root visual element of the UI Document
    private Button resumeButton;
    private Button mainmenuButton;
    private Button quitButton;

    #endregion

    #region Unity Functions

    private void OnEnable()
    {
        // We first get the root visual element from the UIDocument component.
        root = GetComponent<UIDocument>().rootVisualElement;

        // Then we get our buttons by querying (that's what the Q() stands for) the root.
        resumeButton = root.Q<Button>("resume-button");
        mainmenuButton = root.Q<Button>("mainmenu-button");
        quitButton = root.Q<Button>("quit-button");

        // Once we have references to our buttons, we subscribe functions to their clicked events.
        resumeButton.RegisterCallback<ClickEvent>(ResumeButtonPressed);
        mainmenuButton.RegisterCallback<ClickEvent>(MainMenuButtonPressed);
        quitButton.RegisterCallback<ClickEvent>(QuitButtonPressed);
    }

    private void Start()
    {
        // Here we can subscribe to game state changes in the GameState.
        // It's important that we do this here and not in Awake() to ensure that the GameState has already been initialized.
        BK_GameState.Instance.OnGamePaused.AddListener(ReceivedOnGamePaused);
        BK_GameState.Instance.OnGameResumed.AddListener(ReceivedOnGameResumed);

        // Initialize our pause menu to not be visible, since the game is not paused when a level loads
        ReceivedOnGameResumed();
    }

    private void OnDisable()
    {
        // As always, when we subscribe to events we must also remember to unsubscribe
        resumeButton.UnregisterCallback<ClickEvent>(ResumeButtonPressed);
        mainmenuButton.UnregisterCallback<ClickEvent>(MainMenuButtonPressed);
        quitButton.UnregisterCallback<ClickEvent>(QuitButtonPressed);
    }

    private void OnDestroy()
    {
        BK_GameState.Instance.OnGamePaused.RemoveListener(ReceivedOnGamePaused);
        BK_GameState.Instance.OnGameResumed.RemoveListener(ReceivedOnGameResumed);
    }

    #endregion

    #region Custom Functions

    private void ResumeButtonPressed(ClickEvent evt)
    {
        // This function is called when the resume button is pressed.
        // We can access the GameManager through its singleton instance and tell the game to resume.
        BK_GameManager.Instance.ResumeGame();
    }

    private void MainMenuButtonPressed(ClickEvent evt)
    {
        // This function is called when the quit button is pressed.
        // Here we can simply use Unity SceneManagement to swap to a scene named "MainMenu".
        BK_GameManager.Instance.LoadScene(BK_Globals.MainMenuSceneName);
        BK_GameManager.Instance.ResumeGame();
    }

    private void QuitButtonPressed(ClickEvent evt)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit(0);
#endif
    }

    private void ReceivedOnGamePaused()
    {
        // Contains the functionality to execute when the game is paused.
        // Show the Pause UI by changing the visibility of the root component
        root.style.visibility = Visibility.Visible;
    }

    private void ReceivedOnGameResumed()
    {
        // Contains the functionality to execute when the game is resumed.
        // Hide the Pause UI by changing the visibility of the root component
        root.style.visibility = Visibility.Hidden;
    }

    #endregion
}