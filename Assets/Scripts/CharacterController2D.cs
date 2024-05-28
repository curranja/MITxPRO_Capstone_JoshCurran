using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 targetPosition;
    private bool isMoving = false;
    private float originalScaleX; // Store the original x-scale
    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScaleX = transform.localScale.x; // Store the original x-scale
        mainCamera = Camera.main; // Get the main camera
    }

    void Update()
    {
        HandleMovementInput();
        ClampPositionWithinCamera();
    }

    void HandleMovementInput()
    {
        // WASD Movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        if (movement != Vector2.zero)
        {
            // Set animator parameters
            animator.SetFloat("Horizontal", moveHorizontal);
            animator.SetFloat("Vertical", moveVertical);
            animator.SetBool("IsWalking", true); // Set IsWalking to true when moving

            MoveCharacter(movement);

            // Flip the character sprite based on movement direction
            if (moveHorizontal > 0)
                transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z); // Facing right
            else if (moveHorizontal < 0)
                transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z); // Facing left

            // Stop point-to-click movement if WASD movement is detected
            isMoving = false;
        }
        else
        {
            // Point-to-Click Movement
            if (Input.GetMouseButtonDown(0))
            {
                targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                targetPosition = new Vector2(targetPosition.x, targetPosition.y);
                isMoving = true;

                // Set animator parameters
                animator.SetBool("IsWalking", true); // Set IsWalking to true when moving
            }

            if (isMoving)
            {
                // Move towards the target position
                Vector2 direction = (targetPosition - rb.position).normalized;
                MoveCharacter(direction);

                // If close enough to the target, stop moving
                if (Vector2.Distance(rb.position, targetPosition) < 0.1f)
                {
                    isMoving = false;
                    rb.velocity = Vector2.zero;

                    // Set animator parameters
                    animator.SetBool("IsWalking", false); // Set IsWalking to false when not moving
                }

                // Flip the character sprite based on movement direction
                if (direction.x > 0)
                    transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z); // Facing right
                else if (direction.x < 0)
                    transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z); // Facing left
            }
            else
            {
                // If not moving, stop the character
                rb.velocity = Vector2.zero;

                // Set animator parameters
                animator.SetBool("IsWalking", false); // Set IsWalking to false when not moving
            }
        }
    }

    void MoveCharacter(Vector2 direction)
    {
        rb.velocity = direction.normalized * speed;
    }

    void ClampPositionWithinCamera()
    {
        Vector3 position = transform.position;
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(position);

        viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.05f, 0.95f);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.05f, 0.95f);

        transform.position = mainCamera.ViewportToWorldPoint(viewportPosition);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the character collides with an obstacle
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Stop the character's movement
            rb.velocity = Vector2.zero;
        }
    }
}