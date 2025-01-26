using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.Mathematics;

public class BK_MainMenu : BK_MasterUI
{
    private VisualElement root;

    private VisualElement titleLogo;

    private Button startButton;
    private Button quitButton;
    private Button creditsButton;
    private Button settingsButton;

    private VisualElement tutorialPanel;
    private VisualElement creditsPanel;

    private void OnEnable()
    {
        // The UXML is already instantiated by the UIDocument component
        UIDocument uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        titleLogo = root.Q("title");

        startButton = root.Q("start-button") as Button;
        quitButton = root.Q("quit-button") as Button;
        creditsButton = root.Q("credits-button") as Button;
        settingsButton = root.Q("settings-button") as Button;

        tutorialPanel = root.Q("tutorial-panel") as VisualElement;
        creditsPanel = root.Q("credits-panel") as VisualElement;

        startButton.RegisterCallback<ClickEvent>(StartGame);
        quitButton.RegisterCallback<ClickEvent>(QuitGame);
        creditsButton.RegisterCallback<ClickEvent>(ToggleCredits);
        settingsButton.RegisterCallback<ClickEvent>(ToggleSettings);

        RegisterButtonSFX(startButton);
        RegisterButtonSFX(quitButton);
        RegisterButtonSFX(creditsButton);
        RegisterButtonSFX(settingsButton);
    }

    private void OnDisable()
    {
        startButton.UnregisterCallback<ClickEvent>(StartGame);
        quitButton.UnregisterCallback<ClickEvent>(QuitGame);
        creditsButton.UnregisterCallback<ClickEvent>(ToggleCredits);
        settingsButton.UnregisterCallback<ClickEvent>(ToggleSettings);

        UnregisterButtonSFX(startButton);
        UnregisterButtonSFX(quitButton);
        UnregisterButtonSFX(creditsButton);
        UnregisterButtonSFX(settingsButton);
    }

    private void Update()
    {
        titleLogo.style.scale = new StyleScale(V2Wobble(0f, 0f, 0.2f));
        startButton.style.scale = new StyleScale(V2Wobble(0.2f, 0.3f));
        quitButton.style.scale = new StyleScale(V2Wobble(0.4f, 0.6f));
        creditsButton.style.scale = new StyleScale(V2Wobble(0.6f, 0.9f));
        settingsButton.style.scale = new StyleScale(V2Wobble(0.9f, 1.2f));

        titleLogo.style.translate = new StyleTranslate(V2Translate(0.3f, 0.15f, 25f, 15f, 2f));
        startButton.style.translate = new StyleTranslate(V2Translate(2f, 3f, 25f, 15f));
        quitButton.style.translate =new StyleTranslate(V2Translate(9f, 12f, 25f, 15f));
        creditsButton.style.translate = new StyleTranslate(V2Translate(6f, 9f, 25f, 15f));
        settingsButton.style.translate = new StyleTranslate(V2Translate(4f, 6f, 25f, 15f));
    }

    private void StartGame(ClickEvent evt)
    {
        Debug.Log("Starting the game!");
        BK_GameManager.Instance.LoadScene(BK_Globals.Level1SceneName);
    }

    private void QuitGame(ClickEvent evt)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit(0);
#endif
    }

    private void ToggleCredits(ClickEvent evt)
    {
        if (creditsPanel.style.visibility == Visibility.Visible)
        {
            creditsPanel.style.visibility = Visibility.Hidden;
        }
        else
        {
            creditsPanel.style.visibility = Visibility.Visible;
        }
    }

    private void ToggleSettings(ClickEvent evt)
    {
        if (tutorialPanel.style.visibility == Visibility.Visible)
        {
            tutorialPanel.style.visibility = Visibility.Hidden;
        }
        else
        {
            tutorialPanel.style.visibility = Visibility.Visible;
        }
    }
}
