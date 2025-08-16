using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Presentation");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
