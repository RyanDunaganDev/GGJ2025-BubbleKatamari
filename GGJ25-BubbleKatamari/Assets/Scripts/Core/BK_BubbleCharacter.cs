using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class BK_BubbleCharacter : MonoBehaviour
{
    #region Variables

    [Header("Character - Ground Movement")]
    [Tooltip("The current max move speed of this character.")]
    public float currentMaxSpeed = 4f;

    [Header("Character - Character Input")]
    [Tooltip("The 2D movement input from the controller.")]
    protected Vector2 movementInput;
    [Tooltip("The 3D direction in which this character should move.")]
    protected Vector3 movementDirection;

    [Header("Player - Ground Movement")]
    [SerializeField] private float accelerationRate = 60f;      // The speed at which this character accelerates in m/s
    [SerializeField] private float decelerationRate = 12f;      // The speed at which this character decelerates in m/s
    [SerializeField] private float maxRollSpeed = 4f;           // The max horizontal speed of this character (when moving) in m/s
    [SerializeField] private float maxBoostSpeed = 7f;          // The max horizontal speed of this character (when boosting) in m/s
    [SerializeField] private float maxVerticalSpeed = 10f;      // The maximum vertical move speed of this character in m/s
    private bool isBoosting = false;                            // Indicates whether you are boosting or not
    [SerializeField] private float moveSpeedChangeRate = 10f;   // The rate per second that the move speed updates to new targets

    [Header("Player - Air Movement")]
    [SerializeField] private float airControlMultiplier = 0.4f; // The multiplier used to affect the amount of control you have in the air

    [Header("Player - Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.1f;  // The distance below the player to check for ground
    [SerializeField] LayerMask environmentLayerMask;            // Which layers are considered to be the environment
    private bool wasGroundedLastFrame = false;                  // Denotes whether you were on the ground last frame or not
    private bool isGrounded = false;                            // Denotes whether you are on the ground or not

    // WALL STICK DEMO: Variables
    [Header("Player - WallStick Handling")]
    [SerializeField] private bool shouldHandleWallStick = false;    // Should we perform WallStick handling?
    [SerializeField] private bool showWallStickDebug = false;       // Should we show WallStick debug gizmos?
    private Vector3 wallStickDirection = Vector3.zero;              // The direction that we should adjust our wallsticking for
    private float wallStickAngleDiff = 0f;                          // The difference in angle between movementDirection and wallStickDirection
    [SerializeField] private int wallStickFrameLookahead = 1;       // How many frames ahead to check for collisions and adjust wallstick direction
    [SerializeField] private float minWallAngle = 45f;              // Minimum angle (in degrees) that a collision must be to be considered a "wall"
    protected Vector3 finalAdjustedMovementDirection;               // The movementDirection adjusted by wallstick direction

    [Header("Character - Component/Object References")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected SphereCollider sphereCollider;
    [SerializeField] protected Rigidbody rigidbody;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected Transform characterModel;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;         // The transform component of our Character's Camera

    #endregion

    #region Unity Functions

    private void Start()
    {
        // Find the camera if not set
        if (cameraTransform == null) { cameraTransform = Camera.main.transform; }

        // Start out not boosting
        StopBoosting();
    }

    private void FixedUpdate()
    {
        // Check if we are on the ground.
        CheckIsGrounded();

        // WALL STICK DEMO: Check if we're moving into a wall
        // and record the direction to adjust to
        CheckWallStick();

        // Move our character each frame.
        MoveCharacter();

        // Every physics update, make sure that
        // we are not exceeding our current
        // maximum allowed velocity.
        LimitVelocity();
    }

    private void Update()
    {
        // Every frame we should recalculate our
        // camera relative inputs since the camera
        // can move at any time, and our inputs
        // can change at any time.
        CalculateCameraRelativeInput();

        // WALL STICK DEMO: Every frame we should recalculate
        // our WallStick relative inputs since we can
        // walk into a wall at any time.
        CalculateWallStickRelativeInput();
    }

    #endregion

    #region Custom Functions

    #region Input

    public void SetMovementInput(Vector2 moveInput)
    {
        // Set the value of this Movement script's movement input
        movementInput = moveInput;
    }

    /// <summary>
    /// This function will calculate our movement input relative
    /// to our camera's orientation. If we do not do this, our input
    /// will be relative to world coordinates, and will not be intuitive
    /// to use. This makes it so that if you input movement to the right,
    /// rather than "global right" (1f,0f), it will point to the right
    /// of whatever direction the camera is facing in.
    /// </summary>
    void CalculateCameraRelativeInput()
    {
        // Do nothing if there is no movement input.
        // No need to waste CPU time on pointless calculations.
        if (movementInput == Vector2.zero)
        {
            movementDirection = Vector3.zero;
            finalAdjustedMovementDirection = Vector3.zero;
            return;
        }

        // Get the (flat) Forward and Right Vectors of our Camera (without any y value)
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f; // Remove vertical component
        cameraForward.Normalize(); // Make vector length 1
        Vector3 cameraRight = cameraTransform.right; // Right vector is always flat, since the camera doesn't roll

        // Calculate the direction that we should move
        // in by adjusting our movement input to match
        // our camera orientation. We do this by scaling
        // our camera direction vectors by the by the
        // magnitude of our input and add them together.
        movementDirection = cameraForward * movementInput.y + cameraRight * movementInput.x;

        // If our input has a magnitude of greater than 1, we
        // should normalize our input direction vector.
        // We only want to do this if the value is greater than
        // 1, since smaller values are desirable if we aren't
        // performing a full input, such as with a joystick
        // partially moved in a direction.
        if (movementDirection.sqrMagnitude > 1f)
        {
            movementDirection.Normalize();
        }

        // Update final adjusted movement direction
        finalAdjustedMovementDirection = movementDirection;
    }

    /// <summary>
    /// WALL STICK DEMO: This function will calculate our movement input
    /// relative to any walls that we're moving into (as determined by
    /// CheckWallStick().
    /// </summary>
    private void CalculateWallStickRelativeInput()
    {
        // Debug draw movementDirection before modification
        if (showWallStickDebug) { Debug.DrawRay(transform.position + (Vector3.up * 1.1f), movementDirection, Color.blue); }

        // Do nothing if we aren't moving into a wall
        if (wallStickDirection == Vector3.zero) { return; }

        // Align our finalAdjustedMovementDirection with our movementDirection
        finalAdjustedMovementDirection = movementDirection;

        // Don't adjust movement direction for wallstick if angle is too large
        if (wallStickAngleDiff < 90f)
        {
            // Align our finalAdjustedMovementDirection with the direction of our wallStickDirection
            // Find Y-axis rotation between finalAdjustedMovementDirection and WallStickDirection
            Vector3 yLessMoveDir = new Vector3(finalAdjustedMovementDirection.x, 0f, finalAdjustedMovementDirection.z);
            Vector3 yLessWallStickDir = new Vector3(wallStickDirection.x, 0f, wallStickDirection.z);

            // Apply rotation to finalAdjustedMovementDirection
            Quaternion yLessAlignQuat = Quaternion.FromToRotation(yLessMoveDir, yLessWallStickDir);
            finalAdjustedMovementDirection = yLessAlignQuat * finalAdjustedMovementDirection;

            // Decrease finalAdjustedMovementDirection magnitude based on angle difference
            // 90 degree angle = no speed, 0 degree angle = full speed
            float wallStickMagnitudeModifier = (1f - (wallStickAngleDiff / 90f));
            finalAdjustedMovementDirection *= wallStickMagnitudeModifier;
        }

        // Debug draw movementDirection after modification
        if (showWallStickDebug) { Debug.DrawRay(transform.position + (Vector3.up * 2.2f), finalAdjustedMovementDirection, Color.red); }
    }

    #endregion

    #region Movement

    protected void MoveCharacter()
    {
        // We only need to apply forces to move
        // if we are trying to move. Thus, if we
        // aren't inputting anything, don't apply
        // any forces (they'd be 0 anyways).
        // WALL STICK DEMO: Check the final direction for movement
        if (finalAdjustedMovementDirection != Vector3.zero)
        {
            // If we are on the ground we want to move according to our movespeed.
            if (isGrounded)
            {
                // Apply our movement Force.
                rigidbody.AddForce(finalAdjustedMovementDirection * accelerationRate, ForceMode.Acceleration);
            }
            // Otherwise, if we are in the air we want to
            // move according to our movespeed modified by
            // our airControlMultiplier.
            else //Is in air
            {
                // Apply our movement force multiplied by
                // our airControlMultiplier.
                rigidbody.AddForce(finalAdjustedMovementDirection * (accelerationRate * airControlMultiplier), ForceMode.Acceleration);
            }
        }
        // If we're not trying to move but we're on the ground
        else if (isGrounded)
        {
            // And if we're still moving, let's decelerate
            Vector3 currentVelocity = GetHorizontalRBVelocity();
            if (currentVelocity.magnitude > 0.5f)
            {
                // Use an opposing acceleration force to slow down gradually.
                Vector3 counteractDirection = currentVelocity.normalized * -1f;
                rigidbody.AddForce(counteractDirection * decelerationRate, ForceMode.Acceleration);
            }
        }
    }

    /// <summary>
    /// This function is called in every FixedUpdate call.
    /// This will ensure that if we are moving faster than
    /// our maximum allowed velocity, we will slow down to
    /// that maximum velocity.
    /// </summary>
    private void LimitVelocity()
    {
        // Limit Horizontal Velocity
        // If our current velocity is greater than our maximum allowed velocity...
        Vector3 currentVelocity = GetHorizontalRBVelocity();
        // Note: Square root is an expensive operation! Comparing the squared distances is cheaper.
        if (currentVelocity.sqrMagnitude > (currentMaxSpeed * currentMaxSpeed))
        {
            // Use an impulse force to counteract our velocity to slow down to max allowed velocity.
            Vector3 counteractDirection = currentVelocity.normalized * -1f;
            float counteractAmount = currentVelocity.magnitude - currentMaxSpeed;
            rigidbody.AddForce(counteractDirection * counteractAmount, ForceMode.VelocityChange);
        }

        // Limit Vertical Velocity
        // If our current speed is greater than our max speed
        if (Mathf.Abs(rigidbody.linearVelocity.y) > maxVerticalSpeed)
        {
            Vector3 counteractDirection = Vector3.up * Mathf.Sign(rigidbody.linearVelocity.y) * -1f;
            float counteractAmount = Mathf.Abs(rigidbody.linearVelocity.y) - maxVerticalSpeed;
            rigidbody.AddForce(counteractDirection * counteractAmount, ForceMode.VelocityChange);
        }
    }

    ///// <summary>
    ///// This function is called when our jump action
    ///// is performed. It will launch the character
    ///// upwards (if we are able to jump) based on our jumpForce.
    ///// </summary>
    //public override void Jump()
    //{
    //    // If we're ready to jump (cooldown) and we're either on the ground or still have more jumps we can perform
    //    if (readyToJump && (isGrounded || currentJump < maxJumps))
    //    {
    //        // Increment and track our current jump
    //        currentJump += 1;

    //        // Jump, accounting for any pre-existing vertical velocity (for consistent jump height)
    //        float adjustedJumpForce = jumpForce - rigidbody.velocity.y;
    //        rigidbody.AddForce(Vector3.up * adjustedJumpForce, ForceMode.VelocityChange);

    //        //Start our jump cooldown
    //        readyToJump = false;
    //        StartCoroutine(JumpCooldownCoroutine());
    //    }
    //}

    //private IEnumerator JumpCooldownCoroutine()
    //{
    //    yield return new WaitForSeconds(jumpCooldown);
    //    readyToJump = true;
    //}

    ///// <summary>
    ///// This function is called when our jump action
    ///// is canceled. If we are still heading upwards,
    ///// we will halve our vertical velocity. This enables
    ///// us to perform partial jumps.
    ///// </summary>
    //public override void CancelJump()
    //{
    //    // If we're still moving upwards
    //    if (rigidbody.velocity.y > 0f)
    //    {
    //        // Halve our vertical velocity
    //        rigidbody.AddForce(Vector3.down * (rigidbody.velocity.y * 0.5f), ForceMode.VelocityChange);
    //    }
    //}

    /// <summary>
    /// Tell the CharacterMovement to begin sprinting.
    /// </summary>
    public void StartBoosting()
    {
        isBoosting = true;

        StartCoroutine(UpateMaxSpeed(maxBoostSpeed));
    }

    /// <summary>
    /// Tell the CharacterMovement to end sprinting.
    /// </summary>
    public void StopBoosting()
    {
        isBoosting = false;

        StartCoroutine(UpateMaxSpeed(maxRollSpeed));
    }

    private IEnumerator UpateMaxSpeed(float newSpeedTarget)
    {
        while (currentMaxSpeed != newSpeedTarget)
        {
            float diff = newSpeedTarget - currentMaxSpeed;
            float direction = Mathf.Sign(diff);

            float change = moveSpeedChangeRate * Time.deltaTime;

            if (Mathf.Abs(diff) < change)
            {
                currentMaxSpeed = newSpeedTarget;
                break;
            }
            currentMaxSpeed += direction * change;
            
            yield return null;
        }
    }

    #endregion

    #region Ground Checking

    /// <summary>
    /// This function is called every FixedUpdate to check if our player is on the ground.
    /// </summary>
    private void CheckIsGrounded()
    {
        // Record the grounded status from the previous check
        wasGroundedLastFrame = isGrounded;

        // Calculate the center of the spheres on the bottom and top of the capsule for the Capsule Cast
        RaycastHit hit;
        isGrounded = Physics.SphereCast(transform.position + (Vector3.up * 0.1f),  // Center of first circle 
                                        sphereCollider.radius,  // Radius of capsule 
                                        Vector3.down,           // Direction of cast 
                                        out hit,                // RaycastHit that receives information about hit
                                        groundCheckDistance + 0.1f,    // Length of cast
                                        environmentLayerMask);  // LayerMask to specify hittable layers

        // We became grounded this frame
        if (!wasGroundedLastFrame && isGrounded)
        {
            //// Reset jumps when we hit the ground
            //currentJump = 0;
        }
        // We became airborne this frame
        else if (wasGroundedLastFrame && !isGrounded)
        {
            //// If we did not jump
            //if (currentJump == 0)
            //{
            //    // Expend a jump when becoming airborne (walking off a ledge)
            //    currentJump = 1;
            //}
        }
    }

    /// <summary>
    /// WALL STICK DEMO: Performs a capsulecast in the direction of
    /// movement to see if we're moving into something.
    /// </summary>
    private void CheckWallStick()
    {
        // If we shouldn't check for WallStick, stop here
        if (!shouldHandleWallStick)
        {
            wallStickAngleDiff = 0f;
            wallStickDirection = Vector3.zero;
            return;
        }

        // If we're not trying to move, no need to check so stop here
        if (movementDirection == Vector3.zero)
        {
            wallStickAngleDiff = 0f;
            wallStickDirection = Vector3.zero;
            return;
        }

        // Check for walls in the direction of our motion
        Vector3 checkDirection = movementDirection;

        // Check as far as we'll move in the next "wallStickFrameLookahead" frames
        // Add 0.1f to account for start movement
        float checkDistance = (currentMaxSpeed * wallStickFrameLookahead * Time.fixedDeltaTime) + 0.1f;

        // Find any colliders we're going to hit
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, sphereCollider.radius * 0.95f, checkDirection, checkDistance, environmentLayerMask, QueryTriggerInteraction.Ignore);

        // If we did not hit anything, stop here
        if (hits.Length == 0)
        {
            wallStickAngleDiff = 0f;
            wallStickDirection = Vector3.zero;
            return;
        }

        // Find the farthest hit, so if we're touching multiple colliders we adjust
        // for the one we'll walk into next
        bool validWallFound = false;
        RaycastHit farthestHit = hits[0];

        // Check each hit to find the farthest away hit that is considered a wall (based on slope angle)
        foreach (RaycastHit hit in hits)
        {
            // If the angle of the surface we hit is too great to be sloped ground, then it is a wall
            bool countsAsWall = (Vector3.Angle(Vector3.up, hit.normal) >= minWallAngle);
            // Move onto the next hit if this hit isn't a wall
            if (!countsAsWall) { continue; }

            // If this hit is farther, make it the farthest
            if (hit.distance >= farthestHit.distance)
            {
                // Update farthest hit
                farthestHit = hit;

                // We found a hit that is a wall
                validWallFound = true;
            }
        }

        // If no valid wall was found, don't adjust for wall stick
        if (!validWallFound)
        {
            wallStickAngleDiff = 0f;
            wallStickDirection = Vector3.zero;
            return;
        }

        // Find local cross vectors
        Vector3 localRight = Vector3.Cross(farthestHit.normal, Vector3.up).normalized;
        Vector3 localLeft = localRight * -1f;

        // Calculate angles between left and right possible cross vectors
        float rightAngle = Vector3.Angle(checkDirection, localRight);
        float leftAngle = Vector3.Angle(checkDirection, localLeft);

        // Pick the vector closer to the movement direction and record the closer angle
        Vector3 matchVector = localRight;
        wallStickAngleDiff = rightAngle;
        if (leftAngle < rightAngle)
        {
            wallStickAngleDiff = leftAngle;
            matchVector = localLeft;
        }

        // Set the wall stick direction
        wallStickDirection = matchVector;

        if (showWallStickDebug)
        {
            // Debug World Axes
            Debug.DrawRay(farthestHit.point, Vector3.up, Color.green); // World Up
            Debug.DrawRay(farthestHit.point, Vector3.right, Color.red); // World Right
            Debug.DrawRay(farthestHit.point, Vector3.forward, Color.blue); // World Forward
            // Debug Local Axes
            Debug.DrawRay(farthestHit.point, farthestHit.normal, Color.white); // Normal (Local Forward)
            Debug.DrawRay(farthestHit.point, localRight, Color.magenta); // Local Right
            Debug.DrawRay(farthestHit.point, localLeft, Color.cyan); // Local Left
            // Debug Match Vector
            Debug.DrawRay(farthestHit.point + (Vector3.up * 0.1f), matchVector + (Vector3.up * 0.1f), Color.black); // Match Vector
        }
    }

    #endregion

    #region Helper Functions

    /// <summary>
    /// This is a helper function that will strip the y component out of
    /// our Rigidbody velocity and give us back a vector with just the
    /// x and z (horizontal) components of our velocity.
    /// </summary>
    /// <returns>The XZ (horizontal) velocity Vector3.</returns>
    private Vector3 GetHorizontalRBVelocity()
    {
        return new Vector3(rigidbody.linearVelocity.x, 0f, rigidbody.linearVelocity.z);
    }

    #endregion

    #endregion

    private void OnDrawGizmos()
    {
        //// Show ground check
        //Gizmos.color = isGrounded ? Color.green : Color.red;
        //Vector3 p1 = transform.position + (Vector3.up * (capsuleCollider.radius - groundCheckDistance));
        //Gizmos.DrawWireSphere(p1, capsuleCollider.radius);

        // WALL STICK DEMO: Show wallstick check
        if (shouldHandleWallStick && showWallStickDebug)
        {
            Gizmos.color = Color.blue;

            // Check for walls in the direction of our motion
            Vector3 checkDirection = movementDirection;

            // Get the center of the bottom and top spheres of the capsule
            // Move bottom collider up slightly so we don't think we're running into the ground
            // Move start positions in opposite of checkDirection by 0.1f units to make sure
            // capsule doesn't overlap wall at beginning position
            Vector3 sphereStart = transform.position + (checkDirection * -0.1f);

            // Check as far as we'll move in the next "wallStickFrameLookahead" frames
            // Add 0.1f to account for start movement
            float checkDistance = (currentMaxSpeed * wallStickFrameLookahead * Time.fixedDeltaTime) + 0.1f;

            // Draw start position capsule
            Gizmos.DrawWireSphere(sphereStart, sphereCollider.radius);

            Gizmos.color = Color.magenta;

            // Get end position capsule positions
            Vector3 sphereEnd = sphereStart + (checkDirection * checkDistance);

            // Draw end position capsule
            // Draw start position capsule
            Gizmos.DrawWireSphere(sphereEnd, sphereCollider.radius);
        }
    }
}