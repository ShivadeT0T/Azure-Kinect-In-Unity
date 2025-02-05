using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class IndividualPose : MonoBehaviour
{
    
    private int finalFrame;
    private int frameCount = 1;
    private Vector3 initialPos;
    private Vector3 finalPos;
    public Image imageTexture;

    public void SetFrame(int final)
    {
        finalFrame = final;
    }
    public void MoveSelf(float t)
    {
        //Debug.Log(frameCount);
        //Debug.Log(finalFrame);
        Debug.Log("t value: " + t);
        float division = (float)frameCount/finalFrame;
        float time = division * t;
        Debug.Log(time);
        transform.position = Vector3.Lerp(initialPos, finalPos, time);
    }

    public bool HasReachedFinalFrame()
    {
        //Debug.Log(frameCount);
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

    public void ApplyImageSelf(Texture2D texture)
    {
        texture.Apply();
        imageTexture.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(1.0f, 1.0f));

    }

    public void SetPositions(Vector3 init, Vector3 final)
    {
        initialPos = init;
        finalPos = final;
    }
}