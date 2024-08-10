using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuPanel;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
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
        Time.timeScale = isActive ? 1 : 0; 
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
}
