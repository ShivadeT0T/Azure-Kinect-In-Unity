using System;
using System.Runtime.Serialization;

// Class which contains all data sent from background thread to main thread.
[Serializable]
public class BackgroundDataNoDepth : ISerializable
{
    // Timestamp of current data
    public float TimestampInMs { get; set; }

    // Number of detected bodies.
    public ulong NumOfBodies { get; set; }

    // List of all bodies in current frame, each body is list of Body objects.
    public Body[] Bodies { get; set; }

    public BackgroundDataNoDepth(int maxBodiesCount = 20, int maxJointsSize = 100)
    {

        Bodies = new Body[maxBodiesCount];
        for (int i = 0; i < maxBodiesCount; i++)
        {
            Bodies[i] = new Body(maxJointsSize);
        }
    }

    public BackgroundDataNoDepth(SerializationInfo info, StreamingContext context)
    {
        TimestampInMs = (float)info.GetValue("TimestampInMs", typeof(float));
        NumOfBodies = (ulong)info.GetValue("NumOfBodies", typeof(ulong));
        Bodies = (Body[])info.GetValue("Bodies", typeof(Body[]));
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        // Writing only relevant data to serialized stream, without the placeholder data
        // (the real depthimage size is not maxdepthimagesize, but smaller).
        info.AddValue("TimestampInMs", TimestampInMs, typeof(float));
        info.AddValue("NumOfBodies", NumOfBodies, typeof(ulong));
        Body[] ValidBodies = new Body[NumOfBodies];
        for (int i = 0; i < (int)NumOfBodies; i++)
        {
            ValidBodies[i] = Bodies[i];
        }
        info.AddValue("Bodies", ValidBodies, typeof(Body[]));
    }

    public static BackgroundDataNoDepth DeepCopy(BackgroundDataNoDepth copyFromData)
    {
        BackgroundDataNoDepth copiedData = new BackgroundDataNoDepth();
        ulong numOfBodies = copyFromData.NumOfBodies;
        copiedData.NumOfBodies = copyFromData.NumOfBodies;

        copiedData.TimestampInMs = copyFromData.TimestampInMs;

        for (int i = 0; i < (int)numOfBodies; i++)
        {
            copiedData.Bodies[i] = Body.DeepCopy(copyFromData.Bodies[i]);
        }

        return copiedData;
    }
}

