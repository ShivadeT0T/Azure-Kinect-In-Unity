using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PoseSpawnScript : MonoBehaviour
{

    public Camera poseCamera;
    public Queue<Texture2D> posesTexture = new Queue<Texture2D>();

    private RenderTexture renderTexture;
    public GameObject pose;

    void Start()
    {
        renderTexture = new RenderTexture(500, 500, 0);
        poseCamera.targetTexture = renderTexture;
    }

    public void CapturePose()
    {
        Texture2D capturedTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.R8, false);

        RenderTexture.active = renderTexture;

        capturedTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);

        capturedTexture.Apply(capturedTexture);

        posesTexture.Enqueue(capturedTexture);

        RenderTexture.active = null;
    }

    public void SpawnPose()
    {
        if (posesTexture.Count != 0)
        {
            Instantiate(pose, transform.position, transform.rotation);
        }
    }
}
