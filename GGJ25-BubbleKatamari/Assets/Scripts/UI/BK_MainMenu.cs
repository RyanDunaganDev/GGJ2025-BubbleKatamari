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

        float xOff = 25f;
        float yOff = 15f;
        titleLogo.style.translate = new StyleTranslate(new Translate(new Length((Mathf.PerlinNoise1D(Time.time * 0.3f) * 2f - 1f) * xOff * 2f), new Length((Mathf.PerlinNoise1D(Time.time * 0.15f) * 2f - 1f) * yOff * 2f)));
        startButton.style.translate = new StyleTranslate(new Translate(new Length((Mathf.PerlinNoise1D((Time.time + 2f) * 0.3f) * 2f - 1f) * xOff), new Length((Mathf.PerlinNoise1D((Time.time + 3f) * 0.15f) * 2f - 1f) * yOff)));
        quitButton.style.translate = new StyleTranslate(new Translate(new Length((Mathf.PerlinNoise1D((Time.time + 9f) * 0.3f) * 2f - 1f) * xOff), new Length((Mathf.PerlinNoise1D((Time.time + 12f) * 0.15f) * 2f - 1f) * yOff)));
        creditsButton.style.translate = new StyleTranslate(new Translate(new Length((Mathf.PerlinNoise1D((Time.time + 6f) * 0.3f) * 2f - 1f) * xOff), new Length((Mathf.PerlinNoise1D((Time.time + 9f) * 0.15f) * 2f - 1f) * yOff)));
        settingsButton.style.translate = new StyleTranslate(new Translate(new Length((Mathf.PerlinNoise1D((Time.time + 4f) * 0.3f) * 2f - 1f) * xOff), new Length((Mathf.PerlinNoise1D((Time.time + 6f) * 0.15f) * 2f - 1f) * yOff)));
    }

    private Vector2 V2Wobble(float offsetX, float offsetY, float totalScale = 0.025f)
    {
        float xScl = Mathf.Sin((Time.time + offsetX) * 2f);
        float yScl = Mathf.Cos((Time.time + offsetY));

        return new Vector2(math.remap(-1f, 1f, 0.8f, 1f, xScl), math.remap(-1f, 1f, 1f - totalScale, 1f, yScl));
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
