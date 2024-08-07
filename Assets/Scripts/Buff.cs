using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType
{
    Speed,
    Size
}

public class Buff : MonoBehaviour
{
    public BuffType buffType; // The type of buff (Speed or Size)
    public float buffDuration = 7f; // Duration the buff remains active on the player

    // Handle the collision with the ball
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            // Find the player who last touched the ball
            BallController ball = other.GetComponent<BallController>();
            if (ball != null)
            {
                PlayerController player = ball.GetLastPlayerTouched();
                if (player != null)
                {
                    ApplyBuff(player);
                }

                // Destroy the buff object
                Destroy(gameObject);
            }
        }
    }

    private void ApplyBuff(PlayerController player)
    {
        switch (buffType)
        {
            case BuffType.Speed:
                player.StartCoroutine(player.IncreaseSpeed(buffDuration));
                break;
            case BuffType.Size:
                player.StartCoroutine(player.IncreaseSize(buffDuration));
                break;
        }
    }
}
