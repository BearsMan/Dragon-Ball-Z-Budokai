using UnityEngine;
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(BudokaiInput))]
public class BudokaiFighter : MonoBehaviour
{
    public Transform opponent;
    public bool isPlayerControlled = false;

    public float moveSpeed = 5f;
    public float sidestepSpeed = 4f;
    public float gravity = -20f;

    public float health = 100f;
    public float ki = 0f;
    public float maxKi = 100f;

    private CharacterController controller;
    private BudokaiInput input;
    private Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<BudokaiInput>();
    }

    void Update()
    {
        FaceOpponent();

        if (isPlayerControlled)
        {
            HandleMovement();
            HandleCombat();
        }

        ApplyGravity();
    }

    void FaceOpponent()
    {
        if (opponent == null)
            return;

        Vector3 direction = opponent.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    void HandleMovement()
    {
        if (input == null)
            return;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 move =
            forward * input.vertical +
            right * input.horizontal;

        controller.Move(move.normalized * moveSpeed * Time.deltaTime);
    }

    void HandleCombat()
    {
        if (input.punch)
            Debug.Log(name + " Punch");

        if (input.kick)
            Debug.Log(name + " Kick");

        if (input.guard)
            Debug.Log(name + " Guard");

        if (input.charge)
            ki = Mathf.Min(maxKi, ki + 25f * Time.deltaTime);
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}