using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Camera settings
    [Header("Camera Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float maxLookAngle = 90;
    [SerializeField] private float minLookAngle = -85;
    [SerializeField] private float rotationSpeed = 5f;

    // Movement settings
    [Header("Movement Settings")]
    private Vector2 movement;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool viewbob = true;
    [SerializeField] private float bobFrequency = 6f;
    [SerializeField] private float bobHorizontalAmplitude = 0.05f;


    // Animation settings
    [Header("Animation Settings")]
    [SerializeField] private Animator playerAnimator;

    // Other settings
    [Header("Other Settings")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float gravityValue = -9.81f;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Preventing rotation flip on the camera ::Only happens sometimes at the start for some reason::
        if (playerCamera.transform.localRotation.y != 0) playerCamera.transform.localRotation = Quaternion.Euler(playerCamera.transform.localRotation.x, 0, playerCamera.transform.localRotation.z);
        // Ground check
        groundedPlayer = characterController.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        // Gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Movement
        Vector3 moveDirection = new Vector3(movement.x * walkSpeed, 0, movement.y * walkSpeed);
        Vector3 newPosition = transform.right * moveDirection.x + transform.forward * moveDirection.z;
        newPosition.y = playerVelocity.y;

        // View bobbing
        if (viewbob && movement.magnitude > 0 && groundedPlayer)
        {
            float _bobFrequency;
            if (walkSpeed > 5F) _bobFrequency = bobFrequency * 1.5f; // Increase frequency when sprinting
            else _bobFrequency = bobFrequency;
            float bobOffsetX = Mathf.Sin(Time.time * _bobFrequency) * bobHorizontalAmplitude;
            float bobOffsetY = Mathf.Cos(Time.time * _bobFrequency * 2) * bobHorizontalAmplitude;
            playerCamera.transform.localPosition = new Vector3(bobOffsetX, 0.56f + bobOffsetY, 0);
        }
        else
        {
            playerCamera.transform.localPosition = new Vector3(0, 0.56f, 0); // Reset to default position
        }

        characterController.Move(newPosition * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
        if (movement.magnitude == 0)
        {
            playerAnimator.SetTrigger("Idle");
        }
        else if (movement.magnitude > 0 && playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            if (walkSpeed > 5f) playerAnimator.SetTrigger("Sprinting");
            else playerAnimator.SetTrigger("Running");
        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();
        Vector3 rotation = rotationSpeed * Time.deltaTime * new Vector3(0, lookInput.x, 0);
        characterController.transform.Rotate(rotation);
        playerCamera.transform.Rotate(-lookInput.y * rotationSpeed * Time.deltaTime, 0, 0);
        // Clamp camera x rotation to prevent flipping
        Vector3 cameraRotation = playerCamera.transform.localEulerAngles;
        cameraRotation.x = cameraRotation.x > 180 ? cameraRotation.x - 360 : cameraRotation.x; // Normalize to -180 to 180
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, minLookAngle, maxLookAngle); // Limit
        playerCamera.transform.localEulerAngles = new Vector3(cameraRotation.x, playerCamera.transform.localEulerAngles.y, 0);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.action.triggered && canJump && characterController.isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }
    }

    public void Sprinting(InputAction.CallbackContext context)
    {
        if (context.performed && canSprint)
        {
            walkSpeed = 8f;
            if (movement.magnitude > 0) playerAnimator.SetTrigger("Sprinting");
        }
        else if (context.canceled && canSprint)
        {
            walkSpeed = 5f;
            if (movement.magnitude > 0) playerAnimator.SetTrigger("Running");
        }
    }
}
