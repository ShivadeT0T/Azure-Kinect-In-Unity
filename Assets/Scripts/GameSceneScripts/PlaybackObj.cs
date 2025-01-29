using NUnit.Framework.Api;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PlaybackObj : MonoBehaviour
{
    public GameObject m_tracker;
    public GameObject m_poseTracker;

    private List<BackgroundDataNoDepth> frames;
    private List<BackgroundDataNoDepth> poses;
    private int curFrame = 0;
    private int frameLimit;
    private FramesHandler m_framesHandler;

    private bool replayOn = true;

    [SerializeField]
    public float fps = 30f;
    private float timer;
    private float timePerFrame;
    private int poseFrame;

    void Start()
    {
        poseFrame = InfoBetweenScenes.poseInterval * (int) fps;
        m_framesHandler = new FramesHandler(HandlerType.LOAD);
        frames = m_framesHandler.LoadAnimation(InfoBetweenScenes.AnimationFileName).ToList();
        poses = frames.Where((item, index) => index % poseFrame == poseFrame - 1).ToList();
        frameLimit = frames.Count;

        timePerFrame = 1f / fps;

    }
    void Update()
    {
        if (replayOn)
        {
            timer += Time.deltaTime;
            if (timer >= timePerFrame)
            {
                //Debug.Log(timer);
                timer -= timePerFrame;
                UpdatePlaybackObj();
                PoseGenerator();
            }
            else
            {
                UpdateModelLerp(timer / timePerFrame);
            }

        }
    }

    private void UpdatePlaybackObj()
    {
        // TODO: Put condition later to stop it from looping
        if (true)
        {
            m_tracker.GetComponent<TrackerHandler>().updateTracker(frames[curFrame%frames.Count]);
            curFrame++;
        }
    }

    private void UpdateModelLerp(float t)
    {

        if (curFrame < frameLimit - 1)
        {
            m_tracker.GetComponent<TrackerHandler>().updateTrackerLerp(frames[curFrame], frames[curFrame + 1], t);
        }
    }

    private void PoseGenerator()
    {
        if (curFrame % (int) fps == 0)
        {
            m_poseTracker.GetComponent<TrackerHandler>().updateTracker(poses[curFrame / (int) fps % poses.Count]);
        }
    }
}
