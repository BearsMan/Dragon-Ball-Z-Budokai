using UnityEngine;

public class BudokaiCamera : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    public float sideDistance = 14f;
    public float height = 6f;
    public float lookHeight = 2.5f;
    public float smoothSpeed = 6f;

    void LateUpdate()
    {
        if (player1 == null || player2 == null)
            return;

        Vector3 midpoint = (player1.position + player2.position) * 0.5f;

        Vector3 fightDirection = player2.position - player1.position;
        fightDirection.y = 0f;

        if (fightDirection.sqrMagnitude < 0.01f)
            fightDirection = Vector3.right;

        fightDirection.Normalize();

        Vector3 cameraSide = Vector3.Cross(Vector3.up, fightDirection).normalized;

        Vector3 desiredPosition =
            midpoint + cameraSide * sideDistance + Vector3.up * height;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime);

        transform.LookAt(midpoint + Vector3.up * lookHeight);
    }
}