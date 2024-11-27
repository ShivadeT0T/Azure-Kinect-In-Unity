using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecordingManager : MonoBehaviour
{
    public int Countdown = 5;
    private int Seconds;
    private string FileName;

    private FramesHandler framesHandler;
    public GameObject recordingObj;
    public GameObject replayObj;
    public GameObject uiManager;


    private void Awake()
    {
        Application.targetFrameRate = 30;
    }

    private void Start()
    {
        framesHandler = new FramesHandler(HandlerType.SAVE);
    }

    private IEnumerator Waiter()
    {
        for (int i = Countdown; i > 0; i--)
        {
            yield return new WaitForEndOfFrame();
        }
    }
}
