using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using System.Collections;

public class replayRecording : MonoBehaviour
{
    public GameObject m_tracker;

    private IReadOnlyCollection<BackgroundDataNoDepth> frames;
    private bool ReplayOn = false;

    public void SetUpReplay(IReadOnlyCollection<BackgroundDataNoDepth> animation)
    {
        frames = animation;
        ReplayOn = true;
        StartCoroutine(StartReplay());
    }

    private IEnumerator StartReplay()
    {
        while (ReplayOn)
        {
            if (frames.Any())
            {
                //Debug.Log("Inside frames loop");
                foreach (BackgroundDataNoDepth frame in frames)
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
    }

    public void StopReplay()
    {
        ReplayOn = false;
    }
}
