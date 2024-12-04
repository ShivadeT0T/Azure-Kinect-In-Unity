using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RecordingUI : MonoBehaviour
{

    // User inputs
    public TMP_InputField Seconds;
    public TMP_InputField FileName;
    public Toggle PrecisionToggle;

    // Countdown text

    public TMP_Text CountdownToStart;
    public TMP_Text SecondsLeft;

    // Canvases

    public GameObject RecordingSetup;
    public GameObject CountdownCanvas;
    public GameObject SecondsLeftCanvas;
    public GameObject ConfirmationDialog;

    // Logic script
    public RecordingManager recordingManager;

    public void CloseCanvas(GameObject canvas)
    {
        canvas.SetActive(false);
    }

    public void ShowCanvas(GameObject canvas)
    {
        canvas.SetActive(true);
    }

    public void MenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void UpdateCountdown(int countdown)
    {
        CountdownToStart.text = countdown.ToString();
    }

    public void UpdateTimer(int timer)
    {
        SecondsLeft.text = timer.ToString();
    }

    public void BeginRecordingProcess()
    {
        if (!int.TryParse(Seconds.text, out int seconds) || seconds <= 0)
        {
            Debug.Log("Seconds can't be negative, and it must contain some value");
            return;
        }

        recordingManager.SetUpRecording(seconds, PrecisionToggle.isOn);
        CloseCanvas(RecordingSetup);
        ShowCanvas(CountdownCanvas);
    }

    public void ProceedToRecording()
    {
        CloseCanvas(CountdownCanvas);
        ShowCanvas(SecondsLeftCanvas);
    }

    public void ProceedToConfirmation()
    {
        CloseCanvas(SecondsLeftCanvas);
        ShowCanvas(ConfirmationDialog);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SaveAnimation()
    {
        if (string.IsNullOrEmpty(FileName.text)) return;

        recordingManager.SaveAnimationFile(FileName.text);
    }

    public void ReturnToStart()
    {
        CloseCanvas(SecondsLeftCanvas);
        ShowCanvas(RecordingSetup);
    }
    public void HandleCameraError()
    {
        InfoBetweenScenes.ErrorMessage = "Could not connect to Azure camera. Make sure your camera is connected.";
        InfoBetweenScenes.menuState = MenuState.ERROR;
        MenuScene();
    }
}
