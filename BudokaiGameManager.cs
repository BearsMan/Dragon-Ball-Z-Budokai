using UnityEngine;

public class BudokaiGameManager : MonoBehaviour
{
    public Transform player1Spawn;
    public Transform player2Spawn;

    public BudokaiFighter player1;
    public BudokaiFighter player2;

    void Start()
    {
        player1.transform.position = player1Spawn.position;
        player2.transform.position = player2Spawn.position;

        player1.opponent = player2.transform;
        player2.opponent = player1.transform;

        player1.isPlayerControlled = true;
        player2.isPlayerControlled = false;
    }
}