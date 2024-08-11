using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static System.Collections.Specialized.BitVector32;

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

    public AudioClip scoreIncreaseSound; // Sound effect for increasing score
    public AudioClip setWinSound;        // Sound effect for winning a set
    public AudioClip matchWinSound;      // Sound effect for winning the match
    private AudioSource audioSource;

    private int player1Score = 0;
    private int player2Score = 0;
    private int player1SetsWon = 0;
    private int player2SetsWon = 0;

    public int winningScore = 5;       // Define the score needed to win a set
    public int setsToWinMatch = 2;     // Number of sets needed to win the match
    public float setDelay = 1f;        // Delay between sets

    public bool matchWon = false;     // Flag to check if match is won

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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

        // Play the score increase sound
        if (player1Score != 5 && player2Score != 5 && scoreIncreaseSound != null)
        {
            audioSource.PlayOneShot(scoreIncreaseSound);
        }

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
        bool setWon = false;

        if (player1Score >= winningScore)
        {
            player1SetsWon++;
            setWon = true;
            StartCoroutine(ShowSetEndedAndResetScores());
        }
        else if (player2Score >= winningScore)
        {
            player2SetsWon++;
            setWon = true;

            StartCoroutine(ShowSetEndedAndResetScores());
        }

        if (setWon && player1SetsWon != 2 && player2SetsWon != 2 && setWinSound != null)
        {
            audioSource.PlayOneShot(setWinSound); // Play the set win sound
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
        matchWon = true;

        if (player1SetsWon >= setsToWinMatch)
        {
            winMatchImage.sprite = p1WonSprite;
        }
        else if (player2SetsWon >= setsToWinMatch)
        {
            winMatchImage.sprite = p2WonSprite;
        }

        winMatchImage.enabled = true;
        setEndedImage.enabled = false;

        // Stop the background music
        MusicManager.instance.GetMusicSource().Pause();

        // Play the match win sound
        if (matchWinSound != null)
        {
            audioSource.PlayOneShot(matchWinSound);
        }

        Time.timeScale = 0;

        // Show the end menu panel
        FindObjectOfType<EndMenuController>().ShowEndMenu();
    }
}
