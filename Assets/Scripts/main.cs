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

    // Stuff for recording animation
    private static int frameLimit = 450;
    private static int frameCount = 0;
    // public List<BackgroundDataNoDepth> frames = new List<BackgroundDataNoDepth>();
    public BackgroundDataNoDepth[] frames;
    ConcurrentQueue<BackgroundDataNoDepth> framesProcessor = new ConcurrentQueue<BackgroundDataNoDepth>();
    //BlockingCollection<BackgroundDataNoDepth> framesProcessor;

    void Start()
    {
        frames = new BackgroundDataNoDepth[frameLimit];
        //framesProcessor = new BlockingCollection<BackgroundDataNoDepth>(new ConcurrentQueue<BackgroundDataNoDepth>(),frameLimit);

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
                    if (frameCount < frameLimit)
                    {
                        //Debug.Log($"Inside framecount {frameCount}");
                        Debug.Log(m_lastFrameData.TimestampInMs);
                        framesProcessor.Enqueue(BackgroundDataNoDepth.DeepCopy(m_lastFrameData));
                        if (frameCount == frameLimit - 1)
                        {
                            Debug.Log("Last frame reached");
                            framesProcessor.CopyTo(frames, 0);
                        }
                        frameCount++;
                    }
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
        try
        {
            DisposingOfObjects();
            string output = JsonConvert.SerializeObject(frames, Formatting.Indented);
            string filePath = Path.Combine(Application.streamingAssetsPath, "test2.json");
            File.WriteAllText(filePath, output);

            Debug.Log("Saving JSON successful");

        } catch (Exception e)
        {
            Debug.Log($"Failed to write to a file: {e.Message}");
        }
    }


    private void DisposingOfObjects()
    {
        if (m_skeletalTrackingProvider != null)
        {
            m_skeletalTrackingProvider.Dispose();
        }
    }
}
