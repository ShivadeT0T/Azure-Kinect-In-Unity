using NUnit.Framework.Api;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PlaybackObj : MonoBehaviour
{
    public GameObject m_tracker;

    private List<BackgroundDataNoDepth> frames;
    private int curFrame = 0;
    private int frameLimit;
    private FramesHandler m_framesHandler;

    private bool replayOn = true;

    [SerializeField]
    public float fps = 30f;
    private float timer;
    private float timePerFrame;

    void Start()
    {
        m_framesHandler = new FramesHandler(HandlerType.LOAD);
        frames = m_framesHandler.LoadAnimation(InfoBetweenScenes.AnimationFileName).ToList();
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
            }
            else
            {
                UpdateModelLerp(timer / timePerFrame);
            }

        }
    }

    private void UpdatePlaybackObj()
    {
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
}
