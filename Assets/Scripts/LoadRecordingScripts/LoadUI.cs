using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadUI : MonoBehaviour
{
    public GameObject VideoPlayerUi;
    public AnimationPlayer animationPlayer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            VideoPlayerUi.SetActive(!VideoPlayerUi.activeSelf);

        if (Input.GetKeyDown(KeyCode.Space))
            animationPlayer.PlayPause();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            animationPlayer.IncrementVideo(false);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            animationPlayer.IncrementVideo(true);
    }

    public void MenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
