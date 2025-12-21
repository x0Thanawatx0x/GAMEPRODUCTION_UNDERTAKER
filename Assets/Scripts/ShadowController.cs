using UnityEngine;
using System.Collections.Generic;

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

    [Header("=== Platform Control ===")]
    public string platformTag = "Platform";

    private Rigidbody2D rb;
    private Collider2D shadowCollider;
    private Vector2 input;

    // ✅ เก็บ Platform ไว้ล่วงหน้า
    private List<GameObject> platforms = new List<GameObject>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        shadowCollider = GetComponent<Collider2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // 🔥 เก็บ Platform ตอนยัง Active อยู่
        GameObject[] found = GameObject.FindGameObjectsWithTag(platformTag);
        platforms.AddRange(found);
    }

    void Start()
    {
        IgnoreCollisionWithTag(playerTag);
        IgnoreCollisionWithTag(wallTag);

        // ใช้ร่างเงา → ซ่อน Platform
        SetPlatformActive(false);
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
            rb.velocity = Vector2.zero;
        }
    }

    void IgnoreCollisionWithTag(string tagName)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tagName);
        foreach (GameObject obj in targets)
        {
            Collider2D col = obj.GetComponent<Collider2D>();
            if (col != null)
                Physics2D.IgnoreCollision(shadowCollider, col, true);
        }
    }

    // ✅ เปิด–ปิดจาก list ที่เก็บไว้
    void SetPlatformActive(bool isActive)
    {
        foreach (GameObject p in platforms)
        {
            if (p != null)
                p.SetActive(isActive);
        }
    }

    void OnDisable()
    {
        // เลิกใช้ร่างเงา → Platform กลับมา
        SetPlatformActive(true);
    }

    void OnDestroy()
    {
        SetPlatformActive(true);
    }
}
