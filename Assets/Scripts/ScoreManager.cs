using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public Image[] player1ScoreImages;
    public Image[] player2ScoreImages;
    public Sprite[] scoreNumberSprites;
    public Sprite[] setNumberSprites;

    public Image setEndedImage;
    public Image winMatchImage;

    public Sprite p1WonSprite;
    public Sprite p2WonSprite;

    public Image[] player1SetImages;
    public Image[] player2SetImages;

    public PlayerController player1;
    public PlayerController player2;

    public AudioClip scoreIncreaseSound;
    public AudioClip setWinSound;
    public AudioClip matchWinSound;
    private AudioSource audioSource;

    private int player1Score = 0;
    private int player2Score = 0;
    private int player1SetsWon = 0;
    private int player2SetsWon = 0;

    public int winningScore = 5;
    public int setsToWinMatch = 2;
    public float setDelay = 1f;

    public bool matchWon = false;
    private PlayerController winningPlayer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateScoreImages();
        UpdateSetImages();
        setEndedImage.enabled = false;
        winMatchImage.enabled = false;
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

        if (player1Score != 5 && player2Score != 5 && scoreIncreaseSound != null)
        {
            audioSource.PlayOneShot(scoreIncreaseSound);
        }

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
            audioSource.PlayOneShot(setWinSound);
        }

        UpdateSetImages();

        if (player1SetsWon >= setsToWinMatch || player2SetsWon >= setsToWinMatch)
        {
            matchWon = true;
            ShowWinMatchImage();
        }
    }

    IEnumerator ShowSetEndedAndResetScores()
    {
        if (matchWon) yield break;

        setEndedImage.enabled = true;
        winMatchImage.enabled = false;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(setDelay);
        Time.timeScale = 1;
        ResetScores();
        setEndedImage.enabled = false;
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
            player1.DisplayVictorySprite();
            winningPlayer = player1;
        }
        else if (player2SetsWon >= setsToWinMatch)
        {
            winMatchImage.sprite = p2WonSprite;
            player2.DisplayVictorySprite();
            winningPlayer = player2;
        }

        winMatchImage.enabled = true;
        setEndedImage.enabled = false;

        MusicManager.instance.GetMusicSource().Pause();

        if (matchWinSound != null)
        {
            audioSource.PlayOneShot(matchWinSound);
            StartCoroutine(ResumeMusicAfterWinSound());
        }

        Time.timeScale = 0;

        FindObjectOfType<EndMenuController>().ShowEndMenu();
    }

    IEnumerator ResumeMusicAfterWinSound()
    {
        yield return new WaitForSeconds(matchWinSound.length);
        MusicManager.instance.ResumeMusic();
    }

    public PlayerController GetWinningPlayer()
    {
        return winningPlayer;
    }
}