using TMPro;
using UnityEngine;
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

    public void CloseCanvas(GameObject canvas)
    {
        canvas.SetActive(false);
    }

    public void ShowCanvas(GameObject canvas)
    {
        canvas.SetActive(true);
    }


}
