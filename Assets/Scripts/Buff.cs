using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType
{
    Speed,
    IncreasePlayerSize,
    DecreasePlayerSize,
    IncreaseBallSize,
    DecreaseBallSize
}

public class Buff : MonoBehaviour
{
    public BuffType buffType;
    public float buffDuration = 7f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            BallController ball = other.GetComponent<BallController>();
            if (ball != null)
            {
                PlayerController lastPlayerTouched = ball.GetLastPlayerTouched();
                if (lastPlayerTouched != null)
                {
                    ApplyPlayerBuff(lastPlayerTouched);
                }
                ApplyBallBuff(ball);
                Destroy(gameObject);
            }
        }
    }

    private void ApplyPlayerBuff(PlayerController player)
    {
        PlayerController opponent = FindOpponent(player);

        switch (buffType)
        {
            case BuffType.Speed:
                player.StartCoroutine(player.IncreaseSpeed(buffDuration));
                break;
            case BuffType.IncreasePlayerSize:
                player.StartCoroutine(player.IncreaseSize(buffDuration));
                break;
            case BuffType.DecreasePlayerSize:
                if (opponent != null)
                {
                    opponent.StartCoroutine(opponent.DecreaseSize(buffDuration));
                }
                break;
        }
    }

    private void ApplyBallBuff(BallController ball)
    {
        switch (buffType)
        {
            case BuffType.IncreaseBallSize:
                ball.StartCoroutine(ball.ChangeSize(1.5f, buffDuration));
                break;
            case BuffType.DecreaseBallSize:
                ball.StartCoroutine(ball.ChangeSize(0.5f, buffDuration));
                break;
        }
    }

    private PlayerController FindOpponent(PlayerController player)
    {
        BallController ballController = FindObjectOfType<BallController>();

        if (ballController.player1 == player)
        {
            return ballController.player2;
        }
        else if (ballController.player2 == player)
        {
            return ballController.player1;
        }
        return null;
    }
}
