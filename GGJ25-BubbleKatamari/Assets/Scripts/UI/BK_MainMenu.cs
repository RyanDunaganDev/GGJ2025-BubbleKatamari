using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class BK_MainMenu : MonoBehaviour
{
    private VisualElement root;

    private Button startButton;
    private Button quitButton;
    private Button creditsButton;
    private Button settingsButton;

    private void OnEnable()
    {
        // The UXML is already instantiated by the UIDocument component
        UIDocument uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        startButton = root.Q("start-button") as Button;
        quitButton = root.Q("quit-button") as Button;
        //creditsButton = root.Q("credits-button") as Button;
        //settingsButton = root.Q("settings-button") as Button;

        startButton.RegisterCallback<ClickEvent>(StartGame);
        quitButton.RegisterCallback<ClickEvent>(QuitGame);
        //creditsButton.RegisterCallback<ClickEvent>(ToggleCredits);
        //settingsButton.RegisterCallback<ClickEvent>(ToggleSettings);
    }

    private void OnDisable()
    {
        startButton.UnregisterCallback<ClickEvent>(StartGame);
        quitButton.UnregisterCallback<ClickEvent>(QuitGame);
        //creditsButton.UnregisterCallback<ClickEvent>(ToggleCredits);
        //settingsButton.UnregisterCallback<ClickEvent>(ToggleSettings);
    }

    private void StartGame(ClickEvent evt)
    {
        Debug.Log("Starting the game!");
        BK_GameManager.Instance.LoadScene("Level1Scene");
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

    }

    private void ToggleSettings(ClickEvent evt)
    {

    }
}
