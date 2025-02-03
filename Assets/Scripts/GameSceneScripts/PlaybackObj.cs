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
    public GameObject hitzonePosition;

    private List<BackgroundDataNoDepth> frames;
    private Queue<BackgroundDataNoDepth> poses;
    public List<GameObject> poseObjects;
    private int curFrame = 0;
    private int frameLimit;
    private FramesHandler m_framesHandler;

    private bool replayOn = false;

    [SerializeField]
    public int fps = 30;
    public int poseFpsOffset = 15;
    private float timer;
    private float timePerFrame;
    private int poseFrame;

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

    void Start()
    {
        poseFrame = InfoBetweenScenes.poseInterval * (int) fps;
        m_framesHandler = new FramesHandler(HandlerType.LOAD);
        frames = m_framesHandler.LoadAnimation(InfoBetweenScenes.AnimationFileName).ToList();
        poses = new Queue<BackgroundDataNoDepth>(frames.Where((item, index) => index % poseFrame == poseFrame - 1).ToList());
        frameLimit = frames.Count;

        timePerFrame = 1f / fps;

    }
    void Update()
    {
        // Generate poses before starting the playback
        if (!allTextures)
        {
            timer += Time.deltaTime;
            if(timer >= timePerFrame)
            {
                timer -= timePerFrame;
                if (counterForPose == 1)
                {
                    counterForPose = 0;
                    UpdatePose();
                }
                else
                {
                    counterForPose++;
                    GeneratePoseTexture();
                }
            }

        }
        else
        {
            replayOn = true;
        }

        // Playback starts
        if (replayOn)
        {
            timer += Time.deltaTime;
            if (timer >= timePerFrame)
            {
                //Debug.Log(timer);
                timer -= timePerFrame;
                UpdatePlaybackObj();
                if (curFrame % poseFpsOffset == 0)
                {
                    if(morePoses) GeneratePoseObj();
                }
                MovePoses(1);
            }
            else
            {
                UpdateModelLerp(timer / timePerFrame);
                MovePoses(timer / timePerFrame);
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

    private void UpdatePose()
    {
        if (poses.Count != 0)
        {
            m_poseTracker.GetComponent<TrackerHandler>().updateTracker(poses.Dequeue());
        }
        else
        {
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
        if (poseObjects.Count != 0)
        {
            foreach (GameObject pose in poseObjects)
            {
                pose.GetComponent<IndividualPose>().MoveSelf(t);
            }
        }
    }

    public void CheckPoseObj()
    {
        // TODO: Logic for checking if poseObj reached final frame and remove and destroy it if so
        foreach (GameObject pose in poseObjects)
        {
            if (pose.GetComponent<IndividualPose>().HasReachedFinalFrame())
            {
                pose.GetComponent<IndividualPose>().DisposeSelf();
                poseObjects.Remove(pose);
            }
        }
    }

    public void GeneratePoseObj()
    {
        morePoses = spawnScript.SpawnPose();
    }
}
