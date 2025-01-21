using UnityEngine;
using UnityEngine.UI;

public class CameraLiveFeed : MonoBehaviour
{
    public RawImage liveFeed;

    public void updateLiveFeed(BackgroundData frame)
    {
        Texture2D texture = new Texture2D(frame.DepthImageWidth, frame.DepthImageWidth, TextureFormat.ARGB32, false);

        texture.LoadRawTextureData(frame.DepthImage);
        texture.Apply();

        liveFeed.texture = texture;
    }
}
