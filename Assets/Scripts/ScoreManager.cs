using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public Image[] player1ScoreImages;
    public Image[] player2ScoreImages;
    public Sprite[] scoreNumberSprites; // Sprites for regular score (0, 1, 2, 3, 4, 5)
    public Sprite[] setNumberSprites;   // Sprites for set scores (0, 1, 2)

    public Image setEndedImage;         // Image to display "Set Ended" message
    public Image winMatchImage;         // Image to display win match message

    public Sprite p1WonSprite; // Sprite for "P1 Won"
    public Sprite p2WonSprite; // Sprite for "P2 Won"

    public Image[] player1SetImages;    // Images to display the number of sets won by player 1
    public Image[] player2SetImages;    // Images to display the number of sets won by player 2

    public PlayerController player1;
    public PlayerController player2;

    private int player1Score = 0;
    private int player2Score = 0;
    private int player1SetsWon = 0;
    private int player2SetsWon = 0;

    public int winningScore = 5;       // Define the score needed to win a set
    public int setsToWinMatch = 2;     // Number of sets needed to win the match
    public float setDelay = 1f;        // Delay between sets

    private bool matchWon = false;     // Flag to check if match is won

    void Start()
    {
        UpdateScoreImages();
        UpdateSetImages();
        setEndedImage.enabled = false; // Hide "Set Ended" image at the start
        winMatchImage.enabled = false; // Hide win match image at the start
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
        UpdateScoreImages();

        // Check if any player has won the set
        CheckForSetWin();
    }

    void UpdateScoreImages()
    {
        UpdatePlayerScoreImages(player1Score, player1ScoreImages, scoreNumberSprites);
        UpdatePlayerScoreImages(player2Score, player2ScoreImages, scoreNumberSprites);
    }

    void UpdateSetImages()
    {
        UpdatePlayerScoreImages(player1SetsWon, player1SetImages, setNumberSprites);
        UpdatePlayerScoreImages(player2SetsWon, player2SetImages, setNumberSprites);
    }

    void UpdatePlayerScoreImages(int score, Image[] scoreImages, Sprite[] numberSprites)
    {
        string scoreStr = score.ToString();

        for (int i = 0; i < scoreImages.Length; i++)
        {
            if (i < scoreStr.Length)
            {
                int digit = int.Parse(scoreStr[scoreStr.Length - 1 - i].ToString());
                scoreImages[scoreImages.Length - 1 - i].sprite = numberSprites[digit];
                scoreImages[scoreImages.Length - 1 - i].enabled = true;
            }
            else
            {
                scoreImages[scoreImages.Length - 1 - i].enabled = false;
            }
        }
    }

    void CheckForSetWin()
    {
        if (player1Score >= winningScore)
        {
            player1SetsWon++;
            StartCoroutine(ShowSetEndedAndResetScores());
        }
        else if (player2Score >= winningScore)
        {
            player2SetsWon++;
            StartCoroutine(ShowSetEndedAndResetScores());
        }

        UpdateSetImages();

        if (player1SetsWon >= setsToWinMatch || player2SetsWon >= setsToWinMatch)
        {
            ShowWinMatchImage();
        }
    }

    IEnumerator ShowSetEndedAndResetScores()
    {
        if (matchWon) yield break; // If the match is already won, do nothing

        setEndedImage.enabled = true; // Show the "Set Ended" image
        winMatchImage.enabled = false; // Hide win match image
        Time.timeScale = 0; // Pause the game
        yield return new WaitForSecondsRealtime(setDelay); // Wait for the specified delay
        Time.timeScale = 1; // Resume the game
        ResetScores(); // Reset scores for the next set
        setEndedImage.enabled = false; // Hide the "Set Ended" image
    }

    void ResetScores()
    {
        player1Score = 0;
        player2Score = 0;
        UpdateScoreImages();
    }

    void ShowWinMatchImage()
    {
        matchWon = true; // Set match won flag

        if (player1SetsWon >= setsToWinMatch)
        {
            winMatchImage.sprite = p1WonSprite; // Assign the "P1 Won" sprite
        }
        else if (player2SetsWon >= setsToWinMatch)
        {
            winMatchImage.sprite = p2WonSprite; // Assign the "P2 Won" sprite
        }
        winMatchImage.enabled = true; // Display the win match image
        setEndedImage.enabled = false; // Hide the "Set Ended" image
        Time.timeScale = 0; // Pause the game
    }
}
