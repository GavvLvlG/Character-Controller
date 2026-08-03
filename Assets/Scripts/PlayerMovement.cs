using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // Movement speed of the player
    public float playerDamage = 5f;
    public float gravity = -9.81f; // Gravity force applied to the player
    public float jumpHeight = 2f; // Height the player can jump
    private Transform cameraTransform; // Reference to the main camera (used for movement direction)

    private CharacterController controller; // CharacterController component for handling collisions and movement
    private Vector3 velocity; // Stores vertical velocity for jumping and gravity
    private bool isGrounded; // Checks if the player is on the ground
    private float turnSmoothVelocity; // Used to smooth the turning of the player
    private Animator playerAnimator;

    public List<GameObject> enemyList = new List<GameObject>();

    void Awake()
    {
        controller = GetComponent<CharacterController>(); // Get the CharacterController component
        playerAnimator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor
    }

    void Start()
    {
        cameraTransform = Camera.main.transform; // Store reference to the main camera's transform
    }

    void Update()
    {
        // Check if the player is touching the ground
        isGrounded = controller.isGrounded;

        CalculateMoveRot();

        // If grounded and falling down, apply a small downward force
        // This keeps the player "snapped" to the ground instead of slightly floating
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Set to a small negative value to maintain contact with ground
            playerAnimator.SetBool("isJumping", false);
        }

        // If the jump button is pressed and the player is on the ground, calculate jump velocity
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            // Use physics formula to calculate upward velocity for desired jump height
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            HandleAnimation("Jump");
        }

        if (isGrounded && Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        // Apply gravity to the vertical velocity
        velocity.y += gravity * Time.deltaTime;

        // Apply the vertical movement (from gravity and jumping)
        controller.Move(velocity * Time.deltaTime);
    }

    private void CalculateMoveRot()
    {
        // Get input axes for movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Create a normalized direction vector from input
        Vector3 inputDir = new Vector3(moveX, 0, moveZ).normalized;

        // Only process movement and rotation if there's input
        if (inputDir.magnitude >= 0.01f)
        {
            // Calculate the direction relative to the camera's current Y rotation
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            // Smooth the rotation to avoid instant snapping
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0.1f);

            // Rotate the player toward the target angle
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            // Calculate movement direction based on the target angle
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // Move the player using the CharacterController
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
            HandleAnimation("Run");
        }
        else
        { 
            playerAnimator.SetBool("isMoving", false);
        }
    }

    private void Attack()
    {
        HandleAnimation("Attack");
        foreach (var enemy in enemyList)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(playerDamage);
                Debug.Log(damageable.GetHealth());
            }
        }
    }

    void HandleAnimation(string animationString)
    {
        switch (animationString)
        {
            case "Run":
                if (isGrounded)
                {
                    playerAnimator.SetBool("isMoving", true);
                    playerAnimator.SetBool("isJumping", false);
                }
                else
                {
                    playerAnimator.SetBool("isMoving", false);
                }
                break;
            case "Jump":
                playerAnimator.SetBool("isMoving", false);
                playerAnimator.SetBool("isJumping", true);
                break;
            case "Attack":
                playerAnimator.SetBool("isMoving", false);
                playerAnimator.SetBool("isJumping", false);
                playerAnimator.SetTrigger("isAttacking");
                break;
            default:
                break;
        }
    }
}
