using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private static PauseMenu Instance { get; set; }

    public static bool CanPause = true;

    public static bool IsPaused { get; private set; }

    [SerializeField] private GameObject container;

    private void Start()
    {
        Instance = this;

        if (IsPaused)
            Pause();
    }

    public static void Pause()
    {
        if (!Instance || !CanPause) return;

        Instance.container.SetActive(true);

        Time.timeScale = 0;

        IsPaused = true;
    }

    public void Resume()
    {
        container.SetActive(false);

        Time.timeScale = 1;

        IsPaused = false;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
