using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndMenuController : MonoBehaviour
{
    public GameObject endMenuPanel; // The panel that contains the end menu UI
    public Button restartButton;    // Button to restart the game
    public Button mainMenuButton;   // Button to return to the main menu

    private void Start()
    {
        endMenuPanel.SetActive(false); // Hide the end menu panel at the start

        // Assign button listeners
        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    // This method should be called when a player wins the match
    public void ShowEndMenu()
    {
        endMenuPanel.SetActive(true);  // Show the end menu panel
        Time.timeScale = 0;            // Pause the game
    }

    private void RestartGame()
    {
        Time.timeScale = 1; // Resume the game
        MusicManager.instance.ResumeMusic(); // Resume the background music
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Restart the current scene
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1; // Resume the game
        SceneManager.LoadScene("Main Menu");
    }
}