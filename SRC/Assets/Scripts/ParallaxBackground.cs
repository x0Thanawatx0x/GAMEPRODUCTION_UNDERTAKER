using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxEffect = 0.5f;

    private float startPosX;
    private float length;

    void Start()
    {
        startPosX = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float distance = cameraTransform.position.x * parallaxEffect;

        transform.position = new Vector3(startPosX + distance, transform.position.y, transform.position.z);
    }
}