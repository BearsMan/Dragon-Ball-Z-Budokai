using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterMovement : MonoBehaviour
{
    public Transform targetPlayer;
    public float rotationSpeed = 5.0f;
    public float moveSpeed = 2.0f;

    private Animator animator;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    private void Update()
    {
        // Calculate the direction from this player to the target player
        Vector3 toTarget = targetPlayer.position - transform.position;
        toTarget.y = 0; // Ignore vertical difference
        toTarget.Normalize();

        // Calculate the right vector based on the direction to the target player
        Vector3 rightVector = Vector3.Cross(Vector3.up, toTarget);

        // Calculate movement input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = (toTarget * verticalInput + rightVector * horizontalInput).normalized;

        if (moveDirection.magnitude > 0)
        {
            // Rotate the player to face the target
            Quaternion targetRotation = Quaternion.LookRotation(toTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move the player around the target
            Vector3 targetPosition = targetPlayer.position + rightVector * 2.0f; // Adjust distance
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Play walking animation if you have one
            animator.SetBool("IsWalking", true);
        }
        else
        {
            // Stop walking animation
            animator.SetBool("IsWalking", false);
        }
    }
}
