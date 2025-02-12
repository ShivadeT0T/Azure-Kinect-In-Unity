using NUnit.Framework.Api;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PlaybackObj : MonoBehaviour
{
    public JointCalibration jointCalibrationScript;

    public GameObject m_tracker;
    public GameObject m_poseTracker;
    public GameObject hitzonePosition;

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
        poses.Enqueue(scaledFrames[firstPoseFrame]);
        scaledFrames.Where((item, index) => (index - poseFpsOffset) % poseFrame == poseFrame - 1 && index > firstPoseFrame).ToList().ForEach(pose => poses.Enqueue(pose));
        //poses = new Queue<BackgroundDataNoDepth>(frames.Where((item, index) => index % poseFrame == poseFrame - 1).ToList());
        frameLimit = scaledFrames.Count;
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
        // TODO: Logic for checking if poseObj reached final frame and remove and destroy it if so
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
}
