using UnityEngine;

public class AdvancedCameraFollow : MonoBehaviour
{
    public Transform player;

    [Header("=== Follow ===")]
    public Vector2 offset = new Vector2(0, 2f);
    public float smoothTimeX = 0.15f;
    public float smoothTimeY = 0.2f;

    private float velocityX;
    private float velocityY;

    [Header("=== Look Ahead ===")]
    public float lookAheadDistance = 2f;
    public float lookAheadSmooth = 5f;
    private float currentLookAhead;

    [Header("=== Dead Zone ===")]
    public Vector2 deadZone = new Vector2(1f, 1f);

    [Header("=== Clamp (Limit Camera) ===")]
    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    void LateUpdate()
    {
        if (player == null) return;

        float inputX = Input.GetAxisRaw("Horizontal");

        // 🎯 Look Ahead
        float targetLookAhead = inputX * lookAheadDistance;
        currentLookAhead = Mathf.Lerp(currentLookAhead, targetLookAhead, Time.deltaTime * lookAheadSmooth);

        // 🎯 Target Position
        Vector3 targetPos = new Vector3(
            player.position.x + offset.x + currentLookAhead,
            player.position.y + offset.y,
            transform.position.z
        );

        // 🧠 Dead Zone
        Vector3 delta = targetPos - transform.position;

        if (Mathf.Abs(delta.x) < deadZone.x) targetPos.x = transform.position.x;
        if (Mathf.Abs(delta.y) < deadZone.y) targetPos.y = transform.position.y;

        // 🎯 Smooth Follow
        float newX = Mathf.SmoothDamp(transform.position.x, targetPos.x, ref velocityX, smoothTimeX);
        float newY = Mathf.SmoothDamp(transform.position.y, targetPos.y, ref velocityY, smoothTimeY);

        Vector3 finalPos = new Vector3(newX, newY, transform.position.z);

        // 🔒 Clamp
        if (useBounds)
        {
            finalPos.x = Mathf.Clamp(finalPos.x, minBounds.x, maxBounds.x);
            finalPos.y = Mathf.Clamp(finalPos.y, minBounds.y, maxBounds.y);
        }

        transform.position = finalPos;
    }
}