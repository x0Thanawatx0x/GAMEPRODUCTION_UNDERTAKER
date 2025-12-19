using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ShadowController : MonoBehaviour
{
    [Header("=== Free Movement ===")]
    public float moveSpeed = 5f;

    [Header("=== Radius Limit ===")]
    public Transform centerTarget;
    public float maxDistance = 5f;

    [Header("=== Ignore Collision Tags ===")]
    public string playerTag = "Player";
    public string wallTag = "SpriteWall";

    private Rigidbody2D rb;
    private Collider2D shadowCollider;
    private Vector2 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        shadowCollider = GetComponent<Collider2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Start()
    {
        IgnoreCollisionWithTag(playerTag);
        IgnoreCollisionWithTag(wallTag);
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        rb.velocity = input.normalized * moveSpeed;
        ClampDistance();
    }

    void ClampDistance()
    {
        if (centerTarget == null) return;

        Vector2 center = centerTarget.position;
        Vector2 nextPos = rb.position + rb.velocity * Time.fixedDeltaTime;
        float dist = Vector2.Distance(center, nextPos);

        if (dist > maxDistance)
        {
            Vector2 dir = (nextPos - center).normalized;
            rb.position = center + dir * maxDistance;
            rb.velocity = Vector2.zero; // หยุดความเร็วเมื่อชนขอบรัศมี
        }
    }

    void IgnoreCollisionWithTag(string tagName)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tagName);
        foreach (GameObject obj in targets)
        {
            Collider2D col = obj.GetComponent<Collider2D>();
            if (col != null) Physics2D.IgnoreCollision(shadowCollider, col, true);
        }
    }
}