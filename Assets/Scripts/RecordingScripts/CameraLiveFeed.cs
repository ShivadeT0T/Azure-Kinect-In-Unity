using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;

public class CameraLiveFeed : MonoBehaviour
{
    public UnityEngine.UI.Image liveFeedComponent;
    public GameObject imageObj;

    private int width, height;
    private byte[] colorImage;
    private bool imageAvailable = false;

    private void Update()
    {
        if (!imageAvailable) return;

        // Convert BGRA32 image bytes to texture
        Texture2D texture = new Texture2D(width, height, TextureFormat.BGRA32, false);
        texture.LoadRawTextureData(colorImage);
        texture.Apply();

        // Update raw image's texture with current frame
        liveFeedComponent.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(1.0f, 1.0f));

        // Wait for another image to be received
        imageAvailable = false;
    }
    public void UpdateLiveFeed(Image frame)
    {
        // Store image width, height, and byte array
        height = frame.HeightPixels;
        width = frame.WidthPixels;
        colorImage = frame.Memory.ToArray();

        // New image can be processed in the main thread
        imageAvailable = true;
    }

    public void EnableImage(bool value)
    {
        if (imageObj) imageObj.SetActive(value);
    }
}
