using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AnimationPlayer : MonoBehaviour
{
    public GameObject m_tracker;
    private List<BackgroundDataNoDepth> frames;
    private int frameCounter = 0;
    private int frameLimit;
    private bool ReplayON = false;
    private FramesHandler m_framesHandler;

    [SerializeField] Slider timeSlider;
    private bool isDragging = false;

    private void Awake()
    {
        Application.targetFrameRate = 30;
    }

    private void Start()
    {
        m_framesHandler = new FramesHandler(HandlerType.LOAD);
        frames = m_framesHandler.LoadAnimation(InfoBetweenScenes.AnimationFileName).ToList();
        frameLimit = frames.Count;

        timeSlider.onValueChanged.AddListener(HandleTimeSliderValueChanged);
    }

    private void Update()
    {
        if (!isDragging && ReplayON)
        {
            StartCoroutine(Waiter());
            timeSlider.value = frameCounter;

            if (timeSlider.maxValue != frameLimit)
            {
                timeSlider.maxValue = frameLimit;
            }
        }
    }

    IEnumerator Waiter()
    {
        //Debug.Log(frameCounter);
        if (frameCounter == frameLimit) frameCounter = 0;
        UpdateModel(frames[frameCounter]);
        frameCounter++;
        yield return new WaitForEndOfFrame();
    }

    private void UpdateModel(BackgroundDataNoDepth frame)
    {
        m_tracker.GetComponent<TrackerHandler>().updateTracker(frame);
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
        }
    }
}
