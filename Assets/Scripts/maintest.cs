using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// Temporary name. Basically a modified version of "main.cs" to test out loading from JSON file
public class maintest : MonoBehaviour
{
    public GameObject m_tracker;
    public IReadOnlyCollection<BackgroundDataNoDepth> frames;
    public FramesHandler m_framesHandler;


    private void Awake()
    {
        Application.targetFrameRate = 30;
    }
    void Start()
    {
        m_framesHandler = new FramesHandler(HandlerType.LOAD);
        try
        {
            frames = m_framesHandler.LoadAnimation(InfoBetweenScenes.AnimationFileName);
            List<AnimationFile> files = FileManager.LoadFilesInfo().ToList();
            foreach (AnimationFile file in files)
            {
                Debug.Log($"{file.Name} : {file.CreationTime.ToString()}");
            }
            StartCoroutine(Waiter());
        } catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    IEnumerator Waiter()
    {
        if (frames.Any())
        {
            //Debug.Log("Inside frames loop");
            foreach(BackgroundDataNoDepth frame in frames)
            {
                m_tracker.GetComponent<TrackerHandler>().updateTracker(frame);
                //Debug.Log("Inside foreach");
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            Debug.Log("Problem with list");
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("Shutting down");
    }
}
