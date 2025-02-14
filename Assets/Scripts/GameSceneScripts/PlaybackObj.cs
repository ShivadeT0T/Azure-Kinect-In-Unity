using Microsoft.Azure.Kinect.BodyTracking;
using NUnit.Framework.Api;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public enum ScoringType
{
    Excellent,
    Great,
    Alright,
    Miss
}

public class PlaybackObj : MonoBehaviour
{
    Dictionary<ScoringType, float> precisionSystem;
    Dictionary<ScoringType, float> scoringSystem;
    float totalScore;
    float currentScore = 0.0f;

    public JointCalibration jointCalibrationScript;
    public LiveTracking liveTrackingScript;

    public GameObject m_tracker;
    public GameObject m_poseTracker;
    public GameObject hitzonePosition;
    public GameObject floorObj;

    private List<BackgroundDataNoDepth> originalFrames;
    private List<BackgroundDataNoDepth> scaledFrames;
    private Queue<BackgroundDataNoDepth> poses;
    public List<GameObject> poseObjects;
    private int curFrame = 0;
    private int frameLimit;
    private FramesHandler m_framesHandler;

    //private bool replayOn = false;

    [SerializeField]
    public int fps = 30;
    public int poseFpsOffset = 15;
    public int firstPoseFrame;
    private float timer;
    private float timePerFrame;
    public int poseFrame;

    public PoseSpawnScript spawnScript;
    private int counterForPose = 0;
    private bool allTextures = false;
    private bool morePoses = true;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (poseFpsOffset > fps)
            poseFpsOffset = fps;
    }
#endif


    private void Awake()
    {
        precisionSystem = new Dictionary<ScoringType, float>();

        precisionSystem[ScoringType.Excellent] = 25;
        precisionSystem[ScoringType.Great] = 50;
        precisionSystem[ScoringType.Alright] = 80;

        scoringSystem = new Dictionary<ScoringType, float>();

        scoringSystem[ScoringType.Excellent] = 1.0f;
        scoringSystem[ScoringType.Great] = 0.8f;
        scoringSystem[ScoringType.Alright] = 0.5f;
        scoringSystem[ScoringType.Miss] = 0.0f;

    }
    private void Start()
    {
        poseFrame = InfoBetweenScenes.poseInterval * (int) fps;
        firstPoseFrame = poseFrame + poseFpsOffset - 1;
        m_framesHandler = new FramesHandler(HandlerType.LOAD);
        originalFrames = m_framesHandler.LoadAnimation(InfoBetweenScenes.AnimationFileName).ToList();
        Debug.Log("Original frames: " + originalFrames.Count);
        poses = new Queue<BackgroundDataNoDepth>();

        timePerFrame = 1f / fps;
        
    }

    public void BeginPlayback()
    {
        scaledFrames = jointCalibrationScript.ScaleList(originalFrames);
        //scaledFrames = originalFrames;
        RepositionTracker();
        poses.Enqueue(scaledFrames[firstPoseFrame]);
        scaledFrames.Where((item, index) => (index - poseFpsOffset) % poseFrame == poseFrame - 1 && index > firstPoseFrame).ToList().ForEach(pose => poses.Enqueue(pose));
        //poses = new Queue<BackgroundDataNoDepth>(frames.Where((item, index) => index % poseFrame == poseFrame - 1).ToList());
        frameLimit = scaledFrames.Count;
        totalScore = (float)poses.Count;
        Debug.Log(string.Format("Total frames: {0}, Total poses: {1}", scaledFrames.Count, poses.Count));
        StartCoroutine(CustomUpdate());
    }
    IEnumerator CustomUpdate()
    {
        while (true)
        {
            // Generate poses before starting the playback
            if (!allTextures)
            {
                timer += Time.deltaTime;
                if (timer >= timePerFrame)
                {
                    timer -= timePerFrame;
                    if (counterForPose == 1)
                    {
                        counterForPose = 0;
                        GeneratePoseTexture();
                    }
                    else
                    {
                        counterForPose++;
                        UpdatePose();
                    }
                }

            }

            // Playback starts
            else
            {
                timer += Time.deltaTime;
                if (timer >= timePerFrame)
                {
                    //Debug.Log(timer);
                    timer -= timePerFrame;
                    UpdatePlaybackObj();
                    if (curFrame % poseFrame == 0)
                    {
                        if (morePoses)
                        {
                            GeneratePoseObj();
                        }

                        if (curFrame >= scaledFrames.Count)
                        {
                            FinishGame();
                        }
                        else
                        {
                            CheckCoordinates();
                        }
                    }
                    CheckPoseObj();

                }
                else
                {
                    UpdateModelLerp(timer / timePerFrame);
                    MovePoses(timer / timePerFrame);
                }

            }

            yield return new WaitForEndOfFrame();
        }

    }

    private void UpdatePlaybackObj()
    {
        // TODO: Put condition later to stop it from looping
        if (true)
        {
            m_tracker.GetComponent<TrackerHandler>().updateTracker(scaledFrames[curFrame%scaledFrames.Count]);
            curFrame++;
        }
    }

    private void UpdateModelLerp(float t)
    {

        if (curFrame < frameLimit - 1)
        {
            m_tracker.GetComponent<TrackerHandler>().updateTrackerLerp(scaledFrames[curFrame], scaledFrames[curFrame + 1], t);
        }
    }

    private void UpdatePose()
    {
        if (poses.Count != 0)
        {
            m_poseTracker.GetComponent<TrackerHandler>().updateTracker(poses.Dequeue());
        }
        else
        {
            GeneratePoseObj();
            allTextures = true;
        }
    }

    private void GeneratePoseTexture()
    {
        spawnScript.CapturePose();
    }

    public void InsertToPoseObj(GameObject poseObj)
    {
        poseObjects.Add(poseObj);
    }

    public Texture2D GetTexture()
    {
        return spawnScript.RetrievePoseTexture();
    }

    public void MovePoses(float t)
    {
        foreach (GameObject pose in poseObjects.ToList())
        {
            pose.GetComponent<IndividualPose>().MoveSelf();
        }
    }

    public void CheckPoseObj()
    {
        foreach (GameObject pose in poseObjects.ToList())
        {
            if (pose.GetComponent<IndividualPose>().HasReachedFinalFrame())
            {
                pose.GetComponent<IndividualPose>().MoveSelf();
                pose.GetComponent<IndividualPose>().DisposeSelf();
                poseObjects.Remove(pose);
                Debug.Log("Pose destroyed at frame: " + curFrame);
            }
        }
    }

    public void GeneratePoseObj()
    {
        morePoses = spawnScript.SpawnPose();
        Debug.Log("Pose spawned at frame: " + curFrame);
    }

    // Reposition playback tracker to be on top of floor
    public void RepositionTracker()
    {
        // Because our frames' Y-coordinate is reversed, we seek the highest instead of lowest
        float highestY = Mathf.NegativeInfinity;
        foreach(BackgroundDataNoDepth frame in scaledFrames)
        {
            int closestBody = findClosestTrackedBody(frame);
            for (int jointNum = 0; jointNum < (int)JointId.Count; jointNum++)
            {
                if (frame.Bodies[closestBody].JointPositions3D[jointNum].Y > highestY) highestY = frame.Bodies[closestBody].JointPositions3D[jointNum].Y;
            }
        }
        if(highestY > Mathf.NegativeInfinity)
        {
            float heightDifference = highestY + m_tracker.transform.position.y;
            Vector3 newPosition = new Vector3(m_tracker.transform.position.x, floorObj.transform.position.y, m_tracker.transform.position.z);
            newPosition.y = floorObj.transform.position.y + heightDifference;
            m_tracker.transform.position = newPosition;
        }

    }

    public void CheckCoordinates()
    {
        int closestBody = findClosestTrackedBody(scaledFrames[curFrame]);

        int closestBody2 = findClosestTrackedBody(originalFrames[curFrame]);


        // Left and right wrists' positions relative to pelvis

        Vector2 localPos = new Vector2(scaledFrames[curFrame].Bodies[closestBody].JointPositions2D[(int)JointId.Pelvis].X, scaledFrames[curFrame].Bodies[closestBody].JointPositions2D[(int)JointId.Pelvis].Y);
        Vector2 leftWrist = new Vector2(
            scaledFrames[curFrame].Bodies[closestBody].JointPositions2D[(int)JointId.WristLeft].X, scaledFrames[curFrame].Bodies[closestBody].JointPositions2D[(int)JointId.WristLeft].Y);
        Vector2 rightWrist = new Vector2(scaledFrames[curFrame].Bodies[closestBody].JointPositions2D[(int)JointId.WristRight].X, scaledFrames[curFrame].Bodies[closestBody].JointPositions2D[(int)JointId.WristRight].Y);

        Vector2 leftPos = leftWrist - localPos;
        Vector2 rightPos = rightWrist - localPos;


        Vector2 localOgPos = new Vector2(originalFrames[curFrame].Bodies[closestBody].JointPositions2D[(int)JointId.Pelvis].X, originalFrames[curFrame].Bodies[closestBody].JointPositions2D[(int)JointId.Pelvis].Y);
        
        Vector2 leftOgPos = new Vector2(
            originalFrames[curFrame].Bodies[closestBody2].JointPositions2D[(int)JointId.WristLeft].X - localOgPos.x,
            originalFrames[curFrame].Bodies[closestBody2].JointPositions2D[(int)JointId.WristLeft].Y - localOgPos.y);

        Vector2 rightOgPos = new Vector2(
            originalFrames[curFrame].Bodies[closestBody2].JointPositions2D[(int)JointId.WristRight].X - localOgPos.x,
            originalFrames[curFrame].Bodies[closestBody2].JointPositions2D[(int)JointId.WristRight].Y - localOgPos.y);

        //Debug.Log("original frame's left wrist: " + leftOgPos.y + " Y, " + leftOgPos.x + " X");
        //Debug.Log("scaled frame's left wrist: " + leftPos.y + " Y, " + leftPos.x + " X");
        //Debug.Log("original frame's right wrist: " + rightOgPos.y + " Y, " + rightOgPos.x + " X");
        //Debug.Log("scaled frame's right wrist: " + rightPos.y + " Y, " + rightPos.x + " X");

        float averagePrecision = liveTrackingScript.CompareCoordinates(leftPos, rightPos);
        Debug.Log("Average precision: " + averagePrecision);
        CalculateScore(averagePrecision);

    }



    public void CalculateScore(float precision)
    {
        ScoringType currentType = ScoringType.Miss;

        foreach(KeyValuePair<ScoringType, float> score in precisionSystem)
        {
            if(currentType == ScoringType.Miss)
            {
                if (precision <= score.Value) currentType = score.Key;
            }
        }

        currentScore += scoringSystem[currentType];
        Debug.Log("Current score counter :" + currentScore);
        Debug.Log("Score type: " + currentType);
    }

    public void FinishGame()
    {
        Debug.Log("Total accuracy: " + currentScore / totalScore);
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
