using UnityEngine;
using System.Collections;

public class CameraIdleZoomStop : MonoBehaviour
{
    public Transform player;

    public float normalZoom = 5f;
    public float zoomOutSize = 7f;
    public float zoomDuration = 0.5f;
    public float idleTime = 3f;

    float idleTimer;
    Camera cam;
    Coroutine zoomRoutine;
    bool isZoomedOut = false;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = normalZoom;
    }

    void Update()
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb.velocity.magnitude > 0.1f)
        {
            idleTimer = 0f;

            if (isZoomedOut)
            {
                StartZoom(normalZoom);
                isZoomedOut = false;
            }
        }
        else
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleTime && !isZoomedOut)
            {
                StartZoom(zoomOutSize);
                isZoomedOut = true;
            }
        }
    }

    void StartZoom(float targetSize)
    {
        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);

        zoomRoutine = StartCoroutine(SmoothZoom(targetSize));
    }

    IEnumerator SmoothZoom(float targetSize)
    {
        float startSize = cam.orthographicSize;
        float time = 0f;

        while (time < zoomDuration)
        {
            time += Time.deltaTime;
            cam.orthographicSize = Mathf.Lerp(
                startSize,
                targetSize,
                time / zoomDuration
            );
            yield return null;
        }

        cam.orthographicSize = targetSize; // หยุดจริง
    }
}