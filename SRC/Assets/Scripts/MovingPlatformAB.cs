using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatformAB : MonoBehaviour
{
    [Header("=== Reference Points ===")]
    public Transform pointA;
    public Transform pointB;

    [Header("=== Movement Settings ===")]
    public float moveSpeed = 2f;
    public bool startFromA = true;

    private Transform targetPoint;
    private Vector3 lastPosition;
    private Transform passenger;

    private SpriteRenderer sr;
    private Collider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        targetPoint = startFromA ? pointB : pointA;
        transform.position = startFromA ? pointA.position : pointB.position;

        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        MovePlatform();
        MovePassenger();
        lastPosition = transform.position;
    }

    void MovePlatform()
    {
        if (pointA == null || pointB == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPoint.position,
            moveSpeed * Time.fixedDeltaTime
        );

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.01f)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
        }
    }

    void MovePassenger()
    {
        if (passenger == null) return;

        Vector3 delta = transform.position - lastPosition;
        passenger.position += delta;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            passenger = collision.transform;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (passenger == collision.transform)
                passenger = null;
        }
    }

    // ⭐ ซ่อน / แสดง Platform ทั้งภาพ + Collider
    public void SetPlatformActive(bool active)
    {
        if (sr != null) sr.enabled = active;
        if (col != null) col.enabled = active;
    }

    void OnDrawGizmos()
    {
        if (pointA == null || pointB == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pointA.position, pointB.position);
        Gizmos.DrawSphere(pointA.position, 0.1f);
        Gizmos.DrawSphere(pointB.position, 0.1f);
    }
}
