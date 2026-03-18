using UnityEngine;

public class CameraIdleZoomStop : MonoBehaviour
{
    public Transform player;

    [Header("Follow")]
    public float followSpeed = 5f;
    public Vector3 offset = new Vector3(0, 2, -10);

    [Header("Zoom")]
    public float zoomInSize = 3f;
    public float zoomOutSize = 9f;
    public float zoomSpeed = 3f;

    [Header("Idle")]
    public float idleTime = 5f;

    private Camera cam;
    private float idleTimer = 0f;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        bool isMoving = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0;

        if (isMoving)
        {
            // รีเซ็ตเวลา idle
            idleTimer = 0f;

            // กล้องตามผู้เล่น
            Vector3 targetPos = player.position + offset;

            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * followSpeed
            );

            // Zoom In
            cam.orthographicSize = Mathf.Lerp(
                cam.orthographicSize,
                zoomInSize,
                Time.deltaTime * zoomSpeed
            );
        }
        else
        {
            // นับเวลา idle
            idleTimer += Time.deltaTime;

            // ถ้ายืนนิ่งครบ 5 วิ
            if (idleTimer >= idleTime)
            {
                // กล้องกลับไปตำแหน่งกลาง
                Vector3 idlePos = new Vector3(0, 0, -10);

                transform.position = Vector3.Lerp(
                    transform.position,
                    idlePos,
                    Time.deltaTime * followSpeed
                );

                // Zoom Out
                cam.orthographicSize = Mathf.Lerp(
                    cam.orthographicSize,
                    zoomOutSize,
                    Time.deltaTime * zoomSpeed
                );
            }
        }
    }
}