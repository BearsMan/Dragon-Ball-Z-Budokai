using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 0f;
    public float jumpForce;
    public float gravity = 20f;
    private float verticalVelocity;
    private bool isGrounded;
    public int kiPowerUp = 0;
    public CharacterController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        PlayerMovementHandler();
        ApplyGravity();
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            JumpInput();
        }
    }
    private void PlayerMovementHandler()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticialInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.TransformDirection(new Vector3(horizontalInput, 0, verticialInput));
        controller.Move(moveDirection * Time.deltaTime);
    }
    public void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
        else
        {
            verticalVelocity = -0.1f;
        }
        Vector3 gravityVector = new Vector3(0, verticalVelocity, 0);
        controller.Move(gravityVector * Time.deltaTime);
    }
    public void JumpInput()
    {
        verticalVelocity = jumpForce;
    }
}
