using UnityEngine;

public class ScoreText : MonoBehaviour
{
    public float lifeTime = 1.5f;
    public float moveSpeed = 2.5f;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position = transform.position + (Vector3.up * moveSpeed) * Time.deltaTime;
    }
}
