using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public void BackToMenuButton()
    {
        SceneManager.LoadScene(0);
    }
}
