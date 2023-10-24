using UnityEngine;

/// <summary>
/// Manages pausing and quitting the game.
/// </summary>
public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject _pauseMenu;

    [Header("Listening To")]
    [SerializeField] EmptyEventChannel _pauseEvent;

    bool _paused = false;

    void OnEnable()
    {
        _pauseEvent.Listeners += TogglePause;
    }

    void OnDisable()
    {
        _pauseEvent.Listeners -= TogglePause;
    }

    void Start()
    {
        ResumeGame();
    }

    public void TogglePause()
    {
        if (_paused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        _pauseMenu.SetActive(true);
        _paused = true;

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        _pauseMenu.SetActive(false);
        _paused = false;

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
