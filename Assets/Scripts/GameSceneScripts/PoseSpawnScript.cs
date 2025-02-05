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
    public GameObject self;

    void Start()
    {
        renderTexture = new RenderTexture(500, 500, 24); 
        poseCamera.targetTexture = renderTexture;
    }

    public void CapturePose()
    {
        Texture2D capturedTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.BGRA32, false);

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
            poseObj.GetComponent<IndividualPose>().SetFrame(mainScript.poseFpsOffset + mainScript.fps);
            poseObj.transform.SetParent(self.transform, true);
            poseObj.GetComponent<IndividualPose>().SetPositions(transform.position, new Vector3(mainScript.hitzonePosition.transform.position.x, transform.position.y, transform.position.z));
            poseObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            poseObj.GetComponent<IndividualPose>().ApplyImageSelf(posesTexture.Dequeue());
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
