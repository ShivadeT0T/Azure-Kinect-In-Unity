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
    private readonly int FrameLimit;
    private int FrameCount = 0;
    private bool LastFrameReached = false;

    public BackgroundDataNoDepth[] FramesArray;
    public ConcurrentQueue<BackgroundDataNoDepth> FramesProcessor;
    public List<BackgroundDataNoDepth> FramesList;

    public FramesHandler(int frameLimit, HandlerType handlerType)
    {
        FrameLimit = frameLimit;

        switch (handlerType)
        {
            case HandlerType.SAVE:
                FramesArray = new BackgroundDataNoDepth[FrameLimit];
                FramesProcessor = new ConcurrentQueue<BackgroundDataNoDepth>();
                break;
            case HandlerType.BOTH:
                FramesArray = new BackgroundDataNoDepth[FrameLimit];
                FramesProcessor = new ConcurrentQueue<BackgroundDataNoDepth>();
                FramesList = new List<BackgroundDataNoDepth>();
                break;
            default:
                break;
        }
    }

    public FramesHandler(HandlerType handlerType)
    {
        switch (handlerType)
        {
            case HandlerType.LOAD:
                FramesList = new List<BackgroundDataNoDepth>();
                break;
            default:
                break;
        }
    }

    public void ProcessFrame(BackgroundDataNoDepth frame)
    {
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

}