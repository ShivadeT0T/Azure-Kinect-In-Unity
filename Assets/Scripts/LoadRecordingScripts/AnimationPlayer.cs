using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AnimationPlayer : MonoBehaviour
{
    public GameObject m_tracker;
    public LoadUI uiManager;
    public bool LoopOn = true;

    private List<BackgroundDataNoDepth> frames;
    private int frameCounter = 0;
    private int frameLimit;
    private bool ReplayON = false;
    private FramesHandler m_framesHandler;

    [SerializeField] Slider timeSlider;
    private bool isDragging = false;

    [SerializeField]
    private int fps = 30;

    private void Awake()
    {
        Application.targetFrameRate = fps;
    }

    private void Start()
    {
        m_framesHandler = new FramesHandler(HandlerType.LOAD);
        frames = m_framesHandler.LoadAnimation(InfoBetweenScenes.AnimationFileName).ToList();
        if (!frames.Any()) uiManager.HandleFileError();
        frameLimit = frames.Count;

        timeSlider.onValueChanged.AddListener(HandleTimeSliderValueChanged);
        timeSlider.maxValue = frameLimit - 1;

        UpdateModel();
    }

    private void Update()
    {
        if (!isDragging && ReplayON)
        {
            StartCoroutine(Waiter());
            timeSlider.value = frameCounter;
        }
    }

    IEnumerator Waiter()
    {
        //Debug.Log(frameCounter);
        UpdateModel();
        frameCounter++;
        yield return new WaitForEndOfFrame();
    }

    private void UpdateModel()
    {
        if (frameCounter >= frameLimit) frameCounter = LoopOn ? 0 : frameLimit - 1;

        if (frameCounter < 0) frameCounter = 0;
        m_tracker.GetComponent<TrackerHandler>().updateTracker(frames[frameCounter]);
    }

    public void PlayPause()
    {
        ReplayON = !ReplayON;
    }

    public void BeginDrag()
    {
        isDragging = true;
    }

    public void EndDrag()
    {
        isDragging = false;
    }

    public void HandleTimeSliderValueChanged(float value)
    {
        if (isDragging)
        {
            frameCounter = (int) value;
            UpdateModel();
        }
    }

    public void IncrementVideo(bool forward)
    {
        frameCounter = forward ? frameCounter + 10 : frameCounter - 10;
        timeSlider.value = frameCounter;
        UpdateModel();
    }

     public void ChangePlaybackSpeed(int val)
   {
        if (val == 0) fps = 60;

        if (val == 1) fps = 45;

        if (val == 2) fps = 30;

        if (val == 3) fps = 23;

        if (val == 4) fps = 15;
        Debug.Log(fps);
        Application.targetFrameRate = fps;
    }

    public void ToggleLoop(bool toggle)
    {
        LoopOn = toggle;
    }
}
