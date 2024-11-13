using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{

    public GameObject main;
    public void BackToMenuButton()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadAnimation()
    {
        if (!main.activeInHierarchy)
        {
            main.SetActive(true);
        }
        else
        {
            main.SetActive(false);
        }
    }
}
