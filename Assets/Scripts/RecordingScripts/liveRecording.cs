using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;

public class liveRecording : MonoBehaviour
{
    // Handler for SkeletalTracking thread.
    public GameObject m_tracker;
    private SkeletalTrackingProvider m_skeletalTrackingProvider;
    public BackgroundDataNoDepth m_lastFrameData = new BackgroundDataNoDepth();
    private float timeStamp;

    // The "brain" of the scene which will get frames from this script
    public RecordingManager manager;

    void Start()
    {
        timeStamp = 0;
        //tracker ids needed for when there are two trackers
        const int TRACKER_ID = 0;
        m_skeletalTrackingProvider = new SkeletalTrackingProvider(TRACKER_ID);
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_skeletalTrackingProvider.IsRunning)
        {
            if (m_skeletalTrackingProvider.GetCurrentFrameData(ref m_lastFrameData))
            {
                if (m_lastFrameData.NumOfBodies != 0)
                {
                    m_lastFrameData.TimestampInMs -= timeStamp;
                    manager.ProcessRecordingFrame(m_lastFrameData);
                    m_tracker.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData);
                }
                else
                {
                    manager.CheckPrecision();
                }
            }
        }
    }

    public void UpdateTimestamp()
    {
        timeStamp = timeStamp + m_lastFrameData.TimestampInMs;
        Debug.Log("Update timestamp: " + timeStamp);
    }

    private void ChangedActiveScene(UnityEngine.SceneManagement.Scene current, UnityEngine.SceneManagement.Scene next)
    {
        DisposingOfObjects();
    }

    void OnApplicationQuit()
    {
        DisposingOfObjects();
    }

    private void OnDisable()
    {
        DisposingOfObjects();
    }


    private void DisposingOfObjects()
    {
        if (m_skeletalTrackingProvider != null)
        {
            m_skeletalTrackingProvider.Dispose();
        }
    }
    public void NewSkeletalTracking()
    {
        if (m_skeletalTrackingProvider == null)
        {
            const int TRACKER_ID = 0;
            m_skeletalTrackingProvider = new SkeletalTrackingProvider(TRACKER_ID);
        }
    }
}
