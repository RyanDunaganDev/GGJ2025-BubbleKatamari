using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class BK_PauseMenu : BK_MasterUI
{
    #region Variables

    private VisualElement root;  // Root visual element of the UI Document
    private VisualElement titleLogo;
    private Button resumeButton;
    private Button mainmenuButton;
    private Button quitButton;
    private VisualElement duck;

    #endregion

    #region Unity Functions

    private void OnEnable()
    {
        // We first get the root visual element from the UIDocument component.
        root = GetComponent<UIDocument>().rootVisualElement;

        // Then we get our buttons by querying (that's what the Q() stands for) the root.
        titleLogo = root.Q<VisualElement>("pause-logo");
        resumeButton = root.Q<Button>("resume-button");
        mainmenuButton = root.Q<Button>("mainmenu-button");
        quitButton = root.Q<Button>("quit-button");
        duck = root.Q<VisualElement>("duck");

        // Once we have references to our buttons, we subscribe functions to their clicked events.
        resumeButton.RegisterCallback<ClickEvent>(ResumeButtonPressed);
        mainmenuButton.RegisterCallback<ClickEvent>(MainMenuButtonPressed);
        quitButton.RegisterCallback<ClickEvent>(QuitButtonPressed);

        RegisterButtonSFX(resumeButton);
        RegisterButtonSFX(mainmenuButton);
        RegisterButtonSFX(quitButton);
    }

    private void OnDisable()
    {
        // As always, when we subscribe to events we must also remember to unsubscribe
        resumeButton.UnregisterCallback<ClickEvent>(ResumeButtonPressed);
        mainmenuButton.UnregisterCallback<ClickEvent>(MainMenuButtonPressed);
        quitButton.UnregisterCallback<ClickEvent>(QuitButtonPressed);

        UnregisterButtonSFX(resumeButton);
        UnregisterButtonSFX(mainmenuButton);
        UnregisterButtonSFX(quitButton);
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

    private void Update()
    {
        titleLogo.style.scale = new StyleScale(V2Wobble(0f, 0f, 0.2f));
        resumeButton.style.scale = new StyleScale(V2Wobble(0.2f, 0.3f));
        mainmenuButton.style.scale = new StyleScale(V2Wobble(0.4f, 0.6f));
        quitButton.style.scale = new StyleScale(V2Wobble(0.6f, 0.9f));
        duck.style.scale = new StyleScale(V2Wobble(0.9f, 1.2f));

        titleLogo.style.translate = new StyleTranslate(V2Translate(0.3f, 0.15f, 15f, 10f, 2f));
        resumeButton.style.translate = new StyleTranslate(V2Translate(2f, 3f, 15f, 10f));
        mainmenuButton.style.translate = new StyleTranslate(V2Translate(9f, 12f, 15f, 10f));
        quitButton.style.translate = new StyleTranslate(V2Translate(6f, 9f, 15f, 10f));
        duck.style.translate = new StyleTranslate(V2Translate(4f, 6f, 25f, 15f));
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