using UnityEngine;

public class BudokaiInput : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public bool punch;
    public bool kick;
    public bool guard;
    public bool charge;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        punch = Input.GetKeyDown(KeyCode.J);
        kick = Input.GetKeyDown(KeyCode.K);
        guard = Input.GetKey(KeyCode.L);
        charge = Input.GetKey(KeyCode.I);
    }
}