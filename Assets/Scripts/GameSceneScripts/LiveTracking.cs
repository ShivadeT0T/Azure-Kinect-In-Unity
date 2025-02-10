using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LiveTracking : MonoBehaviour
{

    public PlaybackObj mainLogic;
    public GameObject m_tracker;
    public SkeletalTrackingProvider m_skeletalTrackingProvider;
    public BackgroundDataNoDepth m_lastFrameData = new BackgroundDataNoDepth();
    public JointCalibration jointCalibrator;
    private bool beginGame = false;

    void Start()
    {
        jointCalibrator = new JointCalibration();
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
                    handleGamePrep(m_lastFrameData);
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

    private void handleGamePrep(BackgroundDataNoDepth frame)
    {
        if (!beginGame)
        {
            beginGame = jointCalibrator.CalibrationComplete(frame);
        }
        else
        {
            jointCalibrator.CalculateJointAverage();
            mainLogic.BeginPlayback();
        }
    }
}
