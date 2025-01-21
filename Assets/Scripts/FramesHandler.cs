using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public enum HandlerType
{
    SAVE,
    LOAD,
    BOTH
}
public class FramesHandler
{
    private int FrameLimit = 0;
    private int FrameCount = 0;
    private bool LastFrameReached = false;
    private bool recordingSetup = false;

    public BackgroundDataNoDepth[] FramesArray;
    public ConcurrentQueue<BackgroundDataNoDepth> FramesProcessor;
    public List<BackgroundDataNoDepth> FramesList;

    public FramesHandler(HandlerType handlerType)
    {
        switch (handlerType)
        {
            case HandlerType.SAVE:
                FramesProcessor = new ConcurrentQueue<BackgroundDataNoDepth>();
                FramesList = new List<BackgroundDataNoDepth>();
                break;
            case HandlerType.LOAD:
                FramesList = new List<BackgroundDataNoDepth>();
                break;
            default:
                break;
        }
    }

    public void SetUpForRecording(int frameLimit)
    {
        FrameLimit = frameLimit;
        FrameCount = 0;
        FramesArray = new BackgroundDataNoDepth[frameLimit];
        LastFrameReached = false;
        recordingSetup = true;
        FramesProcessor.Clear();
    }

    public bool ProcessingFrames(BackgroundData frame)
    {
        if (LastFrameReached) return false;
        ProcessFrame(frame);
        return true;
    }

    public void ProcessFrame(BackgroundData frame)
    {
        if (!recordingSetup) return;

        if (!LastFrameReached)
        {
            Debug.Log(frame.TimestampInMs);
            FramesProcessor.Enqueue(BackgroundDataNoDepth.DeepCopy(frame));
            FrameCount++;

            if(FrameCount == FrameLimit)
            {
                LastFrameReached = true;
                Debug.Log("Last frame reached");
                FramesProcessor.CopyTo(FramesArray, 0);
                recordingSetup = false;
            }
        }
    }

    public void SaveAnimation(string fileName)
    {
        if (!IsNullOrEmpty(FramesArray) && LastFrameReached)
        {
            string json = JsonConvert.SerializeObject(FramesArray, Formatting.Indented);
            FileManager.CreateJsonFile(fileName, json);
        } else
        {
            Debug.Log("Can't save animation with empty array or while the recording is still in progress.");
        }
    }
    private bool IsNullOrEmpty(Array array)
    {
        return (array == null || array.Length == 0);
    }

    public IReadOnlyCollection<BackgroundDataNoDepth> LoadAnimation(string fileName)
    {
        string animationJson = FileManager.LoadJsonFile(fileName);
        ITraceWriter traceWriter = new MemoryTraceWriter();

        try
        {
            FramesList = JsonConvert.DeserializeObject<List<BackgroundDataNoDepth>>(animationJson, 
                new JsonSerializerSettings { TraceWriter = traceWriter });

        } catch (Exception e)
        {
            Debug.Log(traceWriter);
            Debug.Log($"Couldn't deserialize object: {e.Message}");
        }

        return FramesList;
    }

    public IReadOnlyCollection<BackgroundDataNoDepth> ReturnAnimationList()
    {
        return FramesProcessor;
    }

}