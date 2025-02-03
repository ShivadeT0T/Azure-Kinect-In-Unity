using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoseSpawnScript : MonoBehaviour
{

    public Camera poseCamera;
    public Queue<Texture2D> posesTexture = new Queue<Texture2D>();

    private RenderTexture renderTexture;
    public GameObject pose;
    public PlaybackObj mainScript;

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

    public bool SpawnPose()
    {
        if (posesTexture.Count != 0)
        {
            GameObject poseObj = Instantiate(pose, transform.position, transform.rotation);
            Texture2D texture = posesTexture.Dequeue();
            texture.Apply();
            poseObj.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(1.0f, 1.0f));
            mainScript.poseObjects.Add(poseObj);
            return true;
        }

        Debug.Log("NO MORE POSES TO SPAWN!");
        return false;
    }

    public Texture2D RetrievePoseTexture()
    {
        if (posesTexture.Count != 0)
        {
            return posesTexture.Dequeue();
        }
        return null;
    }
}
