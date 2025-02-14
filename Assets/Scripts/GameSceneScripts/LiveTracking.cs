using Microsoft.Azure.Kinect.BodyTracking;
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

    Quaternion X_180_FLIP = new Quaternion(1.0f, 0.0f, 0.0f, 0.0f);

    void Start()
    {
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
                    if(!beginGame) handleGamePrep(m_lastFrameData);
                    m_tracker.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData);
                }
            }
        }
    }

    private void handleGamePrep(BackgroundDataNoDepth frame)
    {
        if (jointCalibrator.CalibrationComplete(frame)){
            beginGame = true;
            jointCalibrator.CalculateLiveJointAverage();
            mainLogic.BeginPlayback();
        }
    }

    public float CompareCoordinates(Vector2 leftWrist, Vector2 rightWrist)
    {
        // Coordinates before applying local position
        //Debug.Log("live frame's left wrist: " + m_lastFrameData.Bodies[0].JointPositions2D[(int)JointId.WristLeft].Y + " Y, " + m_lastFrameData.Bodies[0].JointPositions2D[(int)JointId.WristLeft].X + " X");
        //Debug.Log("live frame's right wrist: " + m_lastFrameData.Bodies[0].JointPositions2D[(int)JointId.WristRight].Y + " Y, " + m_lastFrameData.Bodies[0].JointPositions2D[(int)JointId.WristRight].X + " X");

        // X-coordinates negative to match with the playback's coordinates (mirrored)

        Vector2 localLivePos = new Vector2(-m_lastFrameData.Bodies[0].JointPositions2D[(int)JointId.Pelvis].X, m_lastFrameData.Bodies[0].JointPositions2D[(int)JointId.Pelvis].Y);
        Vector2 leftLiveWrist = new Vector2(-m_lastFrameData.Bodies[0].JointPositions2D[(int)JointId.WristLeft].X, m_lastFrameData.Bodies[0].JointPositions2D[(int)JointId.WristLeft].Y);
        Vector2 rightLiveWrist = new Vector2(-m_lastFrameData.Bodies[0].JointPositions2D[(int)JointId.WristRight].X, m_lastFrameData.Bodies[0].JointPositions2D[(int)JointId.WristRight].Y);

        Vector2 leftLivePos = leftLiveWrist - localLivePos;
        Vector2 rightLivePos = rightLiveWrist - localLivePos;

        //Debug.Log("live frame's left wrist: " + leftLivePos.y + " Y, " + leftLivePos.x + " X");
        //Debug.Log("live frame's right wrist: " + rightLivePos.y + " Y, " + rightLivePos.x + " X");

        float leftWristDistance = Vector2.Distance(leftWrist, leftLivePos);
        float rightWristDistance = Vector2.Distance(rightWrist, rightLivePos);

        //Debug.Log("Left wrist distance: " + leftWristDistance);
        //Debug.Log("Right wrist distance: " + rightWristDistance);

        return (leftWristDistance + rightWristDistance) / 2;


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

}
