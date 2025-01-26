using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BK_LoadingScreenManager : BK_MasterUI
{
    #region Variables

    [Header("Scene Transition Settings")]
    [Tooltip("The name of the scene to transition to.")]
    [SerializeField] private string targetSceneName = "DefaultSceneName";
    [Tooltip("Used to prevent double scene switching.")]
    private bool isLoadingScene = false;

    [Header("Loading Screen Settings")]
    [Tooltip("The amount of time it takes for the loading screen to fade in/out.")]
    [SerializeField] private float loadingScreenFadeDuration = 0.75f;
    [Tooltip("The rate at which the loading icon rotates. 1 = 1 rotation per second")]
    [SerializeField] private float loadingIconRotationSpeed = 1f;
    [Tooltip("The rate at which the continue text color pulses. 1 = 1 pulse cycle per second")]
    [SerializeField] private float continueTextPulseRate = 4f;
    [Tooltip("The current visibility state of the loading screen.")]
    private bool loadingScreenVisible = true;
    [Tooltip("How much extra time to spend with the loading spinner.")]
    [SerializeField] private float fakeAsyncDelay = 2f;


    [Header("UI Elements")]
    [Tooltip("The root VisualElement in the UI Document.")]
    private VisualElement root;
    [Tooltip("The black background image in the loading screen.")]
    private VisualElement background;
    [Tooltip("The loading icon in the loading screen.")]
    private VisualElement loadingIcon;
    [Tooltip("The text telling the player how to progress to the next level.")]
    private Label continueText;

    [Header("Component References")]
    [Tooltip("The UIDocument component that displays the loading screen.")]
    [SerializeField] private UIDocument uiDocument;

    #endregion

    #region Unity Functions

    #region Singleton

    // Static (global) reference to the single existing instance of the object
    private static BK_LoadingScreenManager _instance = null;

    // Public property to allow access to the Singleton instance
    // A property is a member that provides a flexible mechanism to read, write, or compute the value of a data field.
    public static BK_LoadingScreenManager Instance
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
    }

    #endregion

    private void OnEnable()
    {
        // We first get the root visual element
        // from the UIDocument component.
        root = uiDocument.rootVisualElement;

        // Then we get our UI elements by querying (that's
        // what the Q() stands for) the root. 
        background = root.Q<VisualElement>("background");
        loadingIcon = root.Q<VisualElement>("loading-icon");
        continueText = root.Q<Label>("continue-text");
    }

    void Start()
    {
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // We are not loading a scene, as we just finished loading one
        isLoadingScene = false;

        // Since the loading screen in on by default,
        // hide it when the scene is loaded
        StartCoroutine(HideLoadingScreen());
    }

    private void Update()
    {
        // Rotate the loading icon every frame if it's visible
        if (loadingIcon != null && loadingIcon.style.visibility == Visibility.Visible)
        {
            loadingIcon.style.rotate = new Rotate(Mathf.Floor((Time.time * 8f * loadingIconRotationSpeed) % 8f) * 45f);
        }
    }

    #endregion

    #region Custom Functions

    #region Loading Screen

    public void LoadScene(string targetScene, bool async = true)
    {
        if (isLoadingScene) { return; }

        int buildIndex = SceneUtility.GetBuildIndexByScenePath(targetScene);
        if (buildIndex == -1) {
            Debug.LogWarning($"No such scene \"{targetScene}\" exists.");
            return;
        }

        targetSceneName = targetScene;

        if (async)
        {
            Debug.Log($"Loading scene {targetSceneName} Asynchronously!");
            StartCoroutine(LoadSceneAsynchronous(targetSceneName == BK_Globals.MainMenuSceneName));
        }
        else
        {
            Debug.Log($"Loading scene {targetSceneName} Synchronously!");
            StartCoroutine(LoadSceneSynchronous());
        }
    }

    /// <summary>
    /// Helper function to set visibility of the loading icon.
    /// </summary>
    /// <param name="visible"></param>
    private void LoadingIconVisibility(bool visible)
    {
        if (visible) { loadingIcon.style.visibility = Visibility.Visible; }
        else { loadingIcon.style.visibility = Visibility.Hidden; }
    }

    private void LoadingScreenVisibilityInstant(bool visible)
    {
        LoadingIconVisibility(visible);
        background.style.opacity = new StyleFloat(visible ? 1f : 0f);
        loadingScreenVisible = visible;

        if (visible) { background.pickingMode = PickingMode.Position; }
        else { background.pickingMode = PickingMode.Ignore; }
    }

    /// <summary>
    /// Coroutine that fades in the loading screen over <paramref name="duration"/> seconds.
    /// </summary>
    /// <param name="duration">The duration of the background fade.</param>
    /// <returns>Coroutine IEnumerator reference.</returns>
    private IEnumerator ShowLoadingScreen(float duration = -1f)
    {
        if (duration < 0f) { duration = loadingScreenFadeDuration; }

        // Fade background from 0 to 1 opacity
        float count = 0f;
        float currentOpacity = 0f;
        while (count < duration)
        {
            currentOpacity += (1f / duration) * Time.deltaTime;
            background.style.opacity = new StyleFloat(currentOpacity);
            count += Time.deltaTime;
            yield return null;
        }

        // Show loading screen and icon completely
        LoadingScreenVisibilityInstant(true);
    }

    /// <summary>
    /// Coroutine that fades out the loading screen over <paramref name="duration"/> seconds.
    /// </summary>
    /// <param name="duration">The duration of the background fade.</param>
    /// <returns>Coroutine IEnumerator reference.</returns>
    private IEnumerator HideLoadingScreen(float duration = -1f)
    {
        if (duration < 0f) { duration = loadingScreenFadeDuration; }

        // Hide loading icon before fade out
        LoadingIconVisibility(false);
        // Hide continue text
        continueText.style.opacity = new StyleFloat(0f);

        //Debug.Break();

        // Fade background from 1 to 0 opacity
        float count = 0f;
        float currentOpacity = 1f;
        while (count < duration)
        {
            currentOpacity -= (1f / duration) * Time.deltaTime;
            background.style.opacity = new StyleFloat(currentOpacity);
            count += Time.deltaTime;

            yield return null;
        }

        // Hide loading screen completely
        LoadingScreenVisibilityInstant(false);
    }

    #endregion

    #region Scene Switching

    private IEnumerator LoadSceneSynchronous()
    {
        isLoadingScene = true;

        // Show the loading screen
        yield return ShowLoadingScreen();

        // Synchronously load the scene
        SceneManager.LoadScene(targetSceneName);
    }

    private IEnumerator LoadSceneAsynchronous(bool allowInstantLoad = false)
    {
        isLoadingScene = true;

        // Note: We can actually start loading our scene even before we show our loading screen!
        // That way, while our loading screen is fading in, we're already performing loading operations.
        // Since the operation is asynchronous, it won't block other code, like showing our loading screen.

        // Asynchronously load the scene
        AsyncOperation asyncLoadOperation;
        asyncLoadOperation = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Single);

        // Don't allow moving to the next scene yet
        asyncLoadOperation.allowSceneActivation = false;

        // Show the loading screen
        yield return ShowLoadingScreen();

        // Skip load waiting after fade
        asyncLoadOperation.allowSceneActivation = allowInstantLoad;

        // Add 2 seconds of artificial loading time since the scene load is almost instant
        yield return new WaitForSeconds(fakeAsyncDelay);

        // While the async load is in progress
        float counter = (Mathf.PI / (2f * continueTextPulseRate)) * -1f; // Start at -pi/(2*rate) so the pulse value starts at 0
        continueText.style.opacity = new StyleFloat(0f);
        while (!asyncLoadOperation.isDone)
        {
            // Here you could use the value of "asyncLoadOperation.progress" (0 to 1) to populate a
            // progress bar or percentage text if you wanted to convey the progress to the player.

            // Once the load is ready to go
            if (asyncLoadOperation.progress >= 0.9f)
            {
                // Disable the loading icon since we're done loading
                LoadingIconVisibility(false);

                // Pulse the continue text directions
                float pulse = ((Mathf.Sin(counter * continueTextPulseRate) + 1f) * 0.5f);
                continueText.style.opacity = new StyleFloat(pulse);
                counter += Time.deltaTime; // Increment counter used for opacity pulsing

                // When the player presses spacebar
                if (Keyboard.current.anyKey.wasPressedThisFrame)
                {
                    // Allow moving to the next scene once the load is complete and the player presses spacebar
                    //Debug.Log("Asynchronous scene transition can complete!");
                    asyncLoadOperation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }

    #endregion

    #endregion

}
