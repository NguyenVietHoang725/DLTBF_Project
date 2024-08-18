using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    public Image player1CharacterImage;
    public Image player2CharacterImage;
    public Sprite[] playerSprites;

    public Button confirmButtonPlayer1;
    public Button confirmButtonPlayer2;
    public Button startGameButton;
    public Button player1LeftButton;
    public Button player1RightButton;
    public Button player2LeftButton;
    public Button player2RightButton;

    private int player1Index = 0;
    private int player2Index = 0;

    private bool isPlayer1Confirmed = false;
    private bool isPlayer2Confirmed = false;

    void Start()
    {
        UpdateCharacterUI();
        startGameButton.gameObject.SetActive(false);

        confirmButtonPlayer1.onClick.AddListener(OnConfirmPlayer1);
        confirmButtonPlayer2.onClick.AddListener(OnConfirmPlayer2);
        startGameButton.onClick.AddListener(OnStartGame);
        player1LeftButton.onClick.AddListener(OnPlayer1Left);
        player1RightButton.onClick.AddListener(OnPlayer1Right);
        player2LeftButton.onClick.AddListener(OnPlayer2Left);
        player2RightButton.onClick.AddListener(OnPlayer2Right);

        UpdateControlInteractivity();
    }

    public void OnPlayer1Left()
    {
        if (!isPlayer1Confirmed)
        {
            player1Index = (player1Index - 1 + playerSprites.Length) % playerSprites.Length;
            UpdateCharacterUI();
        }
    }

    public void OnPlayer1Right()
    {
        if (!isPlayer1Confirmed)
        {
            player1Index = (player1Index + 1) % playerSprites.Length;
            UpdateCharacterUI();
        }
    }

    public void OnPlayer2Left()
    {
        if (!isPlayer2Confirmed)
        {
            player2Index = (player2Index - 1 + playerSprites.Length) % playerSprites.Length;
            UpdateCharacterUI();
        }
    }

    public void OnPlayer2Right()
    {
        if (!isPlayer2Confirmed)
        {
            player2Index = (player2Index + 1) % playerSprites.Length;
            UpdateCharacterUI();
        }
    }

    private void UpdateCharacterUI()
    {
        player1CharacterImage.sprite = playerSprites[player1Index];
        player2CharacterImage.sprite = playerSprites[player2Index];
    }

    public void OnConfirmPlayer1()
    {
        isPlayer1Confirmed = true;
        UpdateControlInteractivity();
        CheckReadyToStart();
    }

    public void OnConfirmPlayer2()
    {
        isPlayer2Confirmed = true;
        UpdateControlInteractivity();
        CheckReadyToStart();
    }

    private void CheckReadyToStart()
    {
        if (isPlayer1Confirmed && isPlayer2Confirmed)
        {
            startGameButton.gameObject.SetActive(true);
        }
    }

    private void UpdateControlInteractivity()
    {
        player1LeftButton.interactable = !isPlayer1Confirmed;
        player1RightButton.interactable = !isPlayer1Confirmed;
        player2LeftButton.interactable = !isPlayer2Confirmed;
        player2RightButton.interactable = !isPlayer2Confirmed;
    }

    public void OnStartGame()
    {
        PlayerPrefs.SetInt("Player1CharacterIndex", player1Index);
        PlayerPrefs.SetInt("Player2CharacterIndex", player2Index);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Gameplay");
    }
}
