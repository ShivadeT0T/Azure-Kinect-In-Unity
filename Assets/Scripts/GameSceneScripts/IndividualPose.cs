using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class IndividualPose : MonoBehaviour
{
    
    private int finalFrame;
    private int frameCount = 1;
    private Vector3 initialPos, finalPos;
    private PlaybackObj mainScript;


    void Start()
    {
        mainScript = GameObject.FindGameObjectWithTag("MainGameScript").GetComponent<PlaybackObj>();
        finalFrame = mainScript.fps + mainScript.poseFpsOffset;
        initialPos = transform.position;
        var hitZone = mainScript.hitzonePosition;
        finalPos = new Vector3(hitZone.transform.position.x, hitZone.transform.position.y, initialPos.z);
    }
    public void MoveSelf(float t)
    {
        Debug.Log(finalFrame);
        transform.position = Vector3.Lerp(initialPos, finalPos, frameCount/finalFrame * t);
    }

    public bool HasReachedFinalFrame()
    {
        if (frameCount <= finalFrame)
        {
            frameCount++;
            return false;
        }

        return true;
    }

    public void DisposeSelf()
    {
        Destroy(this.gameObject);
    }
}