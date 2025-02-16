using Microsoft.Azure.Kinect.BodyTracking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AnimationPlayer : MonoBehaviour
{
    public GameObject floorObj;
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

    // Time variables
    [SerializeField]
    public float fps = 30f;
    private float timer;
    private float timePerFrame;

    [SerializeField]
    public bool lerp = true;

    private void Awake()
    {
        Application.targetFrameRate = 90;
    }

    private void Start()
    {
        m_framesHandler = new FramesHandler(HandlerType.LOAD);
        frames = m_framesHandler.LoadAnimation(InfoBetweenScenes.AnimationFileName).ToList();
        if (!frames.Any()) uiManager.HandleFileError();
        frameLimit = frames.Count;

        timeSlider.onValueChanged.AddListener(HandleTimeSliderValueChanged);
        timeSlider.maxValue = frameLimit - 1;

        timePerFrame = 1f / fps;

        UpdateModel();
        RepositionTracker();
    }

    private void Update()
    {
        if (!isDragging && ReplayON)
        {
            timer += Time.deltaTime;
            if (timer >= timePerFrame)
            {
                //Debug.Log(timer);
                timer -= timePerFrame;
                UpdateModel();
                frameCounter++;
                timeSlider.value = frameCounter;
            }
            else if (lerp)
            {
                UpdateModelLerp(timer / timePerFrame);
            }
        }
    }

    private void UpdateModel()
    {
        if (frameCounter >= frameLimit) frameCounter = LoopOn ? 0 : frameLimit - 1;

        if (frameCounter < 0) frameCounter = 0;
        m_tracker.GetComponent<TrackerHandler>().updateTracker(frames[frameCounter]);
    }

    private void UpdateModelLerp(float t)
    {

        if (frameCounter < 0) frameCounter = 0;

        if (frameCounter < frameLimit - 1)
        {
            m_tracker.GetComponent<TrackerHandler>().updateTrackerLerp(frames[frameCounter], frames[frameCounter + 1], t);
        }
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
        frameCounter = (int)value;
        UpdateModel();
    }

    public void IncrementVideo(bool forward)
    {
        frameCounter = forward ? frameCounter + 10 : frameCounter - 10;
        timeSlider.value = frameCounter;
        UpdateModel();
    }

    public void ChangePlaybackSpeed(int val)
    {
        if (val == 0) fps = 60f;

        if (val == 1) fps = 45f;

        if (val == 2) fps = 30f;

        if (val == 3) fps = 23f;

        if (val == 4) fps = 15f;
        Debug.Log(fps);

        timePerFrame = 1f / fps;
        timer = 0;
    }

    public void ToggleLoop(bool toggle)
    {
        LoopOn = toggle;
    }

    public void ToggleLerp(bool toggle)
    {
        lerp = toggle;
    }

    public void RepositionTracker()
    {
        // Because our frames' Y-coordinate is reversed, we seek the highest instead of lowest
        Debug.Log("Inside reposition function");
        float highestY = Mathf.NegativeInfinity;
        foreach (BackgroundDataNoDepth frame in frames)
        {
            int closestBody = findClosestTrackedBody(frame);
            for (int jointNum = 0; jointNum < (int)JointId.Count; jointNum++)
            {
                if (frame.Bodies[closestBody].JointPositions3D[jointNum].Y > highestY) highestY = frame.Bodies[closestBody].JointPositions3D[jointNum].Y;
            }
        }
        if (highestY > Mathf.NegativeInfinity)
        {
            float heightDifference = highestY + m_tracker.transform.position.y;
            Vector3 newPosition = new Vector3(m_tracker.transform.position.x, floorObj.transform.position.y, m_tracker.transform.position.z);
            newPosition.y = floorObj.transform.position.y + heightDifference;
            m_tracker.transform.position = newPosition;
        }

    }

    private int findClosestTrackedBody(BackgroundDataNoDepth trackerFrameData)
    {
        int closestBody = -1;
        const float MAX_DISTANCE = 5000.0f;
        float minDistanceFromKinect = MAX_DISTANCE;
        for (int i = 0; i < (int)trackerFrameData.NumOfBodies; i++)
        {
            var pelvisPosition = trackerFrameData.Bodies[i].JointPositions3D[(int)JointId.Pelvis];
            Vector3 pelvisPos = new Vector3((float)pelvisPosition.X, (float)pelvisPosition.Y, (float)pelvisPosition.Z);
            if (pelvisPos.magnitude < minDistanceFromKinect)
            {
                closestBody = i;
                minDistanceFromKinect = pelvisPos.magnitude;
            }
        }
        return closestBody;
    }
}
