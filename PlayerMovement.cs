using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float flySpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float flyForce = 15f;
    [SerializeField] private float rotationSpeed = 10f;
    // Start is called before the first frame update
    void Start()
    {

    }

    private Rigidbody rb;
    private bool isGrounded = false;
    private bool isFlying = false;
    private bool isAttacking = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);

        // Handle movement using traditional Unity Input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 moveInput = new Vector2(horizontalInput, verticalInput);
        Move(moveInput);

        // Handle jumping and flying using traditional Unity Input
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                Jump();
            }
            else
            {
                ToggleFlying();
            }
        }
    }

    private void Move(Vector2 moveInput)
    {
        // Apply horizontal movement
        Vector3 moveDirection = transform.forward * moveInput.y * moveSpeed;
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

        // Handle rotation
        if (moveInput.x != 0f)
        {
            float rotationY = transform.eulerAngles.y + (moveInput.x * rotationSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(0f, rotationY, 0f);
        }

        // Apply vertical movement (fly up/down)
        if (isFlying)
        {
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 flyDirection = transform.up * verticalInput * flySpeed;
            rb.velocity = new Vector3(rb.velocity.x, flyDirection.y, rb.velocity.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    private void ToggleFlying()
    {
        if (isFlying)
        {
            rb.useGravity = true;
            isFlying = false;
        }
        else
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * flyForce, ForceMode.Impulse);
            isFlying = true;
        }
    }
}
