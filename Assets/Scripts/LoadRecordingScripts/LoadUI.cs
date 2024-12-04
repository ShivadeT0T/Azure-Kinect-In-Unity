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

    public void HandleFileError()
    {
        InfoBetweenScenes.ErrorMessage = "File could not be found or it contains incorrect data. Please make sure the file exists or has correct data.";
        InfoBetweenScenes.menuState = MenuState.ERROR;
        MenuScene();
    }
}
