using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnQuitButton()
    {
        Application.Quit();
        Debug.Log("Quit button pressed");
    }
}
