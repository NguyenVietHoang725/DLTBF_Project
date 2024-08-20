using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject volumeMenuPanel;
    public BallController ballController;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        volumeMenuPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    void TogglePauseMenu()
    {
        bool isActive = pauseMenuPanel.activeSelf;
        pauseMenuPanel.SetActive(!isActive);
        volumeMenuPanel.SetActive(false);
        Time.timeScale = isActive ? 1 : 0;
        ballController.isPaused = !isActive;

        if (isActive)
        {
            ballController.ResumeCountdownSound();
        }
        else
        {
            ballController.PauseCountdownSound();
        }
    }

    public void OnResumeButtonClicked()
    {
        TogglePauseMenu(); 
    }

    public void OnRestartButtonClicked()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMainMenuButtonClicked()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene("Main Menu");
    }

    public void OnVolumeButtonClicked()
    {
        pauseMenuPanel.SetActive(false);
        volumeMenuPanel.SetActive(true);
    }

    public void OnBackToPauseMenuButtonClicked()
    {
        volumeMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }
}
