using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecordingManager : MonoBehaviour
{
    public int Countdown = 5;
    private int Seconds;
    private string FileName;
    private bool Precision;
    private bool RecordingOn;

    private FramesHandler framesHandler;

    public GameObject recordingObj;
    public liveRecording recordingScript;

    public GameObject replayObj;
    public replayRecording replayScript;

    public RecordingUI uiManager;

    public int fps = 30;
    public int counter = 0;


    private void Awake()
    {
        Application.targetFrameRate = fps;
        RecordingOn = false;
    }

    private void Start()
    {
        framesHandler = new FramesHandler(HandlerType.SAVE);
        uiManager.UpdateCountdown(Countdown);
    }

    private IEnumerator StartCountdown()
    {
        for (int i = Countdown; i > 0; i--)
        {
            uiManager.UpdateCountdown(i);
            yield return new WaitForSeconds(1);
        }
        BeginRecording();
    }

    public void SetUpRecording(int seconds, bool precision)
    {
        Seconds = seconds;
        Precision = precision;
        framesHandler.SetUpForRecording(seconds * fps);
        uiManager.UpdateTimer(seconds);

        StartCoroutine(StartCountdown());
    }

    private void BeginRecording()
    {
        uiManager.ProceedToRecording();
        recordingScript.UpdateTimestamp();
        RecordingOn = true;
    }

    public void ProcessRecordingFrame(BackgroundDataNoDepth frame)
    {
        if (!RecordingOn) return;
        UpdateSeconds();
        if (!framesHandler.ProcessingFrames(frame))
        {
            RecordingOn = false;
            uiManager.ProceedToConfirmation();
            recordingObj.SetActive(false);
            RecordingPlayback();
        }
    }

    public void UpdateSeconds()
    {
        if (counter == fps)
        {
            uiManager.UpdateTimer(--Seconds);
            counter = 0;
        }
        else
        {
            counter++;
        }
    }

    public void RecordingPlayback()
    {
        replayObj.SetActive(true);
        replayScript.SetUpReplay(framesHandler.ReturnAnimationList());
    }

    public void SaveAnimationFile(string fileName)
    {
        framesHandler.SaveAnimation(fileName);
        uiManager.MenuScene();
    }

    public void CheckPrecision()
    {
        if (!RecordingOn) return;
        if (Precision)
        {
            RecordingOn = false;
            uiManager.ReturnToStart();
        }
    }

    public void NoTrackerHandling()
    {
        uiManager.MenuScene();
    }
}
