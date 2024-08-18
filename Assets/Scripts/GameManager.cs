using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController player1;
    public PlayerController player2;

    void Start()
    {
        InitializePlayers();
    }

    private void InitializePlayers()
    {
        if (player1 != null)
        {
            player1.InitializePlayer();
        }
        else
        {
            Debug.LogError("Player 1 not assigned in GameManager");
        }

        if (player2 != null)
        {
            player2.InitializePlayer();
        }
        else
        {
            Debug.LogError("Player 2 not assigned in GameManager");
        }
    }
}