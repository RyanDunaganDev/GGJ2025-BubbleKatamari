using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // We need this include to use the Input System package

public class BK_BubbleController : MonoBehaviour
{
    #region Variables

    [Header("Player Input")]
    private PlayerInputActions playerInputActions; // This is the object that listens for inputs at the hardware level
    private Vector2 movementInput;

    [Header("Component / Object References")]
    [SerializeField] private BK_BubbleCharacter bubbleMovement;

    #endregion

    #region Unity Functions

    // Awake is called before Start() when an object is created or when the level is loaded
    private void Awake()
    {
        // Set up our player actions in code
        // This class name is based on what you named your .inputactions asset
        playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        // GameState callback listeners
        BK_GameState.Instance.OnGamePaused.AddListener(OnGamePausedReceived);
        BK_GameState.Instance.OnGameResumed.AddListener(OnGameResumedReceived);
    }

    private void OnEnable()
    {
        // Here we can subscribe functions to our
        // input actions to make code occur when
        // our input actions occur
        SubscribeInputActions();

        // We need to enable our "Player" action map so Unity will listen for our input
        SwitchActionMap("Player");
    }

    private void OnDisable()
    {
        // Here we can unsubscribe our functions
        // from our input actions so our object
        // doesn't try to call functions after
        // it is destroyed
        UnsubscribeInputActions();

        // Disable all action maps
        SwitchActionMap();
    }

    #endregion

    #region Custom Functions

    #region Input Handling

    private void SubscribeInputActions()
    {
        // Here we can bind our input actions to functions
        playerInputActions.Player.Move.started += MoveAction;
        playerInputActions.Player.Move.performed += MoveAction;
        playerInputActions.Player.Move.canceled += MoveAction;

        playerInputActions.Player.Boost.performed += BoostActionPerformed;
        playerInputActions.Player.Boost.canceled += BoostActionCanceled;

        playerInputActions.Player.TogglePause.performed += TogglePauseActionPerformed;
        playerInputActions.UI.TogglePause.performed += TogglePauseActionPerformed;
    }

    private void UnsubscribeInputActions()
    {
        // It is important to unbind and actions that we bind
        // when our object is destroyed, or this can cause issues
        playerInputActions.Player.Move.started -= MoveAction;
        playerInputActions.Player.Move.performed -= MoveAction;
        playerInputActions.Player.Move.canceled -= MoveAction;

        playerInputActions.Player.Boost.performed -= BoostActionPerformed;
        playerInputActions.Player.Boost.canceled -= BoostActionCanceled;

        playerInputActions.Player.TogglePause.performed -= TogglePauseActionPerformed;
        playerInputActions.UI.TogglePause.performed -= TogglePauseActionPerformed;
    }

    /// <summary>
    /// Helper function to switch to a particular action map
    /// in our player's Input Actions Asset.
    /// </summary>
    /// <param name="mapName">The name of the map we want to switch to.</param>
    private void SwitchActionMap(string mapName = "")
    {
        playerInputActions.Player.Disable();
        playerInputActions.UI.Disable();

        switch (mapName)
        {
            case "Player":
                // We need to enable our "Player" action map so Unity will listen for our player input.
                playerInputActions.Player.Enable();

                // Since we are switching into gameplay, we will no longer need control of our mouse cursor
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;

            case "UI":
                // We need to enable our "UI" action map so Unity will listen for our UI input.
                playerInputActions.UI.Enable();

                // Since we are switching into a UI, we will also need control of our mouse cursor
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                break;

            default:
                // Show the mouse cursor
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                break;
        }
    }

    #endregion

    #region Input Actions

    private void MoveAction(InputAction.CallbackContext context)
    {
        // Read in the Vector2 of our player input.
        movementInput = context.ReadValue<Vector2>();

        //Debug.Log("The player is trying to move: " + movementInput);

        bubbleMovement.SetMovementInput(movementInput);
    }

    private void BoostActionPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("The player is trying to Boost!");

        bubbleMovement.StartBoosting();
    }

    private void BoostActionCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("The player is trying to Stop Boosting!");

        bubbleMovement.StopBoosting();
    }

    private void TogglePauseActionPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("The player is trying to pause/unpause!");

        BK_GameManager.Instance.TogglePause();
    }

    #endregion

    #region Pause Callbacks

    private void OnGamePausedReceived()
    {
        SwitchActionMap("UI");
    }

    private void OnGameResumedReceived()
    {
        SwitchActionMap("Player");
    }

    #endregion

    #endregion
}