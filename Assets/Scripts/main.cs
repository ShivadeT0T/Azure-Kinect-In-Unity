using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class main : MonoBehaviour
{
    // Handler for SkeletalTracking thread.
    public GameObject m_tracker;
    private SkeletalTrackingProvider m_skeletalTrackingProvider;
    public BackgroundDataNoDepth m_lastFrameData = new BackgroundDataNoDepth();

    // Handler for animation frames
    private FramesHandler m_framesHandler;

    void Start()
    {
        // Give desired frame limit for the animation to the frames handler
        const int frameLimit = 450;
        m_framesHandler = new FramesHandler(frameLimit, HandlerType.SAVE);

        //tracker ids needed for when there are two trackers
        const int TRACKER_ID = 0;
        m_skeletalTrackingProvider = new SkeletalTrackingProvider(TRACKER_ID);
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    void Update()
    {
        if (m_skeletalTrackingProvider.IsRunning)
        {
            if (m_skeletalTrackingProvider.GetCurrentFrameData(ref m_lastFrameData))
            {
                if (m_lastFrameData.NumOfBodies != 0)
                {
                    m_framesHandler.ProcessFrame(m_lastFrameData);
                    m_tracker.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData);
                }
            }
        }
    }
    private void ChangedActiveScene(UnityEngine.SceneManagement.Scene current, UnityEngine.SceneManagement.Scene next)
    {
        DisposingOfObjects();
    }

    void OnApplicationQuit()
    {
        DisposingOfObjects();
        m_framesHandler.SaveAnimation("dummy");
    }


    private void DisposingOfObjects()
    {
        if (m_skeletalTrackingProvider != null)
        {
            m_skeletalTrackingProvider.Dispose();
        }
    }
}
