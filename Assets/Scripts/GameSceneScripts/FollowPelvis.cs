using UnityEngine;

public class FollowPelvis : MonoBehaviour
{
    public Transform pelvis;
    public Vector3 offset;

    private void Update()
    {
        transform.position = pelvis.position + offset;
    }
}
