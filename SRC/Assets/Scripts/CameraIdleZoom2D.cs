using UnityEngine;
using System.Collections;

public class CameraIdleZoomStop : MonoBehaviour
{
    public Transform player;

    [Header("Follow")]
    public Vector2 cameraOffset = new Vector2(0, 2f);
    public float smoothTimeX = 0.15f;
    public float smoothTimeY = 0.4f;

    [Header("Zoom Size")]
    public float zoomInSize = 5f;
    public float zoomOutSize = 6.5f;

    [Header("Zoom Speed")]
    public float zoomDuration = 0.4f;

    [Header("Zoom Out Delay")]
    public float zoomOutDelay = 1.5f;   // เวลาที่ต้องหยุดก่อนซูมออก

    float idleTimer;
    Camera cam;
    Coroutine zoomRoutine;
    bool isZoomedOut = false;

    float velocityX;
    float velocityY;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = zoomOutSize;
    }

    void LateUpdate()
    {
        FollowPlayer();
        HandleZoom();
    }

    void FollowPlayer()
    {
        if (player == null) return;

        float targetX = player.position.x + cameraOffset.x;
        float targetY = player.position.y + cameraOffset.y;

        float newX = Mathf.SmoothDamp(transform.position.x, targetX, ref velocityX, smoothTimeX);
        float newY = Mathf.SmoothDamp(transform.position.y, targetY, ref velocityY, smoothTimeY);

        transform.position = new Vector3(newX, newY, transform.position.z);
    }

    void HandleZoom()
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            idleTimer = 0f;

            if (isZoomedOut)
            {
                Debug.Log("Camera Zoom IN");
                StartZoom(zoomInSize);
                isZoomedOut = false;
            }
        }
        else
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= zoomOutDelay && !isZoomedOut)
            {
                Debug.Log("Camera Zoom OUT");
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

        cam.orthographicSize = targetSize;
    }
}