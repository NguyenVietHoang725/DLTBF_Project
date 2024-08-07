using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;
    public TMP_Text winMessageText; // Reference to display the win message

    public PlayerController player1;
    public PlayerController player2;

    private int player1Score = 0;
    private int player2Score = 0;

    public int winningScore = 5; // Define the score needed to win

    void Start()
    {
        UpdateScoreText();
        winMessageText.text = ""; // Hide the win message at the start
    }

    public void IncreaseScore(PlayerController player)
    {
        if (player == player1)
        {
            player1Score++;
        }
        else if (player == player2)
        {
            player2Score++;
        }
        UpdateScoreText();

        // Check if any player has won
        CheckForWin();
    }

    void UpdateScoreText()
    {
        player1ScoreText.text = player1Score.ToString();
        player2ScoreText.text = player2Score.ToString();
    }

    void CheckForWin()
    {
        if (player1Score >= winningScore)
        {
            ShowWinMessage("Player 1 Wins!");
        }
        else if (player2Score >= winningScore)
        {
            ShowWinMessage("Player 2 Wins!");
        }
    }

    void ShowWinMessage(string message)
    {
        winMessageText.text = message;
        // Optionally, you can add logic to stop the game or reset it
        Time.timeScale = 0; // Pause the game
        // Additional logic can be added here to reset the game or navigate to a new scene
    }
}
