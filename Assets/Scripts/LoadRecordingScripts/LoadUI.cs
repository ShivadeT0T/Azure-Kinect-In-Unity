using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadUI : MonoBehaviour
{
    public GameObject VideoPlayerUi;
    public AnimationPlayer animationPlayer;
    public GameObject infoUi;
    public Toggle lerpToggle;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            VideoPlayerUi.SetActive(false);
            infoUi.SetActive(false);
        }
            

        if(Input.GetKeyDown(KeyCode.Escape))
            VideoPlayerUi.SetActive(true);

        if (Input.GetKeyDown(KeyCode.Space))
            animationPlayer.PlayPause();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            animationPlayer.IncrementVideo(false);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            animationPlayer.IncrementVideo(true);

        if (Input.GetKeyDown(KeyCode.L))
            lerpToggle.isOn = !lerpToggle.isOn;
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

    public void ToggleInfo()
    {
        infoUi.SetActive(!infoUi.activeSelf);
    }
}