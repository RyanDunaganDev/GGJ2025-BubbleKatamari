using UnityEngine;
using UnityEngine.UIElements;

public class BK_GameOver : BK_MasterUI
{
    #region Variables

    private VisualElement root;  // Root visual element of the UI Document
    private VisualElement titleLogo;
    private Button playagainButton;
    private Button mainmenuButton;
    private Button quitButton;

    private VisualElement duckVE;
    private VisualElement partyhatVE;

    [SerializeField] private Texture2D duckNormal;
    [SerializeField] private Texture2D duckCry;

    #endregion

    private void OnEnable()
    {
        // We first get the root visual element from the UIDocument component.
        root = GetComponent<UIDocument>().rootVisualElement;

        // Then we get our buttons by querying (that's what the Q() stands for) the root.
        titleLogo = root.Q<VisualElement>("gameover-logo");
        playagainButton = root.Q<Button>("playagain-button");
        mainmenuButton = root.Q<Button>("mainmenu-button");
        quitButton = root.Q<Button>("quit-button");

        duckVE = root.Q<VisualElement>("duck");
        partyhatVE = root.Q<VisualElement>("party-hat");

        // Once we have references to our buttons, we subscribe functions to their clicked events.
        playagainButton.RegisterCallback<ClickEvent>(PlayAgainButtonPressed);
        mainmenuButton.RegisterCallback<ClickEvent>(MainMenuButtonPressed);
        quitButton.RegisterCallback<ClickEvent>(QuitButtonPressed);

        RegisterButtonSFX(playagainButton);
        RegisterButtonSFX(mainmenuButton);
        RegisterButtonSFX(quitButton);
    }

    private void OnDisable()
    {
        // As always, when we subscribe to events we must also remember to unsubscribe
        playagainButton.UnregisterCallback<ClickEvent>(PlayAgainButtonPressed);
        mainmenuButton.UnregisterCallback<ClickEvent>(MainMenuButtonPressed);
        quitButton.UnregisterCallback<ClickEvent>(QuitButtonPressed);

        UnregisterButtonSFX(playagainButton);
        UnregisterButtonSFX(mainmenuButton);
        UnregisterButtonSFX(quitButton);
    }

    private void Update()
    {
        titleLogo.style.scale = new StyleScale(V2Wobble(0f, 0f, 0.2f));
        playagainButton.style.scale = new StyleScale(V2Wobble(0.2f, 0.3f));
        mainmenuButton.style.scale = new StyleScale(V2Wobble(0.4f, 0.6f));
        quitButton.style.scale = new StyleScale(V2Wobble(0.6f, 0.9f));
        duckVE.style.scale = new StyleScale(V2Wobble(0.9f, 1.2f));

        titleLogo.style.translate = new StyleTranslate(V2Translate(0.3f, 0.15f, 15f, 10f, 2f));
        playagainButton.style.translate = new StyleTranslate(V2Translate(2f, 3f, 15f, 10f));
        mainmenuButton.style.translate = new StyleTranslate(V2Translate(9f, 12f, 15f, 10f));
        quitButton.style.translate = new StyleTranslate(V2Translate(6f, 9f, 15f, 10f));
        duckVE.style.translate = new StyleTranslate(V2Translate(4f, 6f, 25f, 15f));
    }

    private void Start()
    {
        // Here we can subscribe to game state changes in the GameState.
        // It's important that we do this here and not in Awake() to ensure that the GameState has already been initialized.
        BK_GameState.Instance.OnTimeExpired.AddListener(ReceivedOnTimeExpired);
        BK_GameState.Instance.OnPlayerLost.AddListener(ReceivedOnPlayerLost);

        // Initialize our pause menu to not be visible, since the game is not paused when a level loads
        HideMenu();
    }

    private void OnDestroy()
    {
        BK_GameState.Instance.OnTimeExpired.RemoveListener(ReceivedOnTimeExpired);
        BK_GameState.Instance.OnPlayerLost.RemoveListener(ReceivedOnPlayerLost);
    }

    #region Custom Functions

    private void PlayAgainButtonPressed(ClickEvent evt)
    {
        // This function is called when the resume button is pressed.
        // We can access the GameManager through its singleton instance and tell the game to resume.
        BK_GameManager.Instance.LoadScene(BK_Globals.Level1SceneName);
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

    private void ReceivedOnTimeExpired()
    {
        ShowMenu(false);
    }

    private void ReceivedOnPlayerLost()
    {
        ShowMenu(true);
    }

    private void HideMenu()
    {
        // Hide the menu
        root.style.visibility = Visibility.Hidden;
    }

    private void ShowMenu(bool playerLost)
    {
        if (playerLost)
        {
            duckVE.style.backgroundImage = new StyleBackground(duckCry);
            partyhatVE.style.visibility = Visibility.Hidden;
        }
        else
        {
            duckVE.style.backgroundImage = new StyleBackground(duckNormal);
            partyhatVE.style.visibility = Visibility.Visible;
        }

        // Show the menu
        root.style.visibility = Visibility.Visible;
    }

    #endregion
}
