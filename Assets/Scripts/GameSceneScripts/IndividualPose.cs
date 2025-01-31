using UnityEngine;

public class IndividualPose : MonoBehaviour
{
    private int finalFrame;
    private int frameCount = 0;
    private Vector3 initialPos, finalPos;


    void Start()
    {
        var mainScript = GameObject.FindGameObjectWithTag("MainGameScript").GetComponent<PlaybackObj>();
        finalFrame = mainScript.fps;
        initialPos = transform.position;
        var hitZone = mainScript.hitzonePosition;
        finalPos = new Vector3(hitZone.transform.position.x, hitZone.transform.position.y, initialPos.z);
        mainScript.InsertToPoseObj(this.gameObject.GetComponent<IndividualPose>());
    }

    public void MoveSelf(float t)
    {
        transform.position = Vector3.Lerp(initialPos, finalPos, t == 1 ? 1 : frameCount/finalFrame * t);
    }

    public bool HasReachedFinalFrame()
    {
        if (frameCount < finalFrame)
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