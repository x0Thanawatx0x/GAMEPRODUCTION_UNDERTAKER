using UnityEngine;

public class CannonTurretStraight : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Range")]
    public float detectRange = 12f;

    [Header("Weapon")]
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float fireRate = 0.8f;
    public float bulletSpeed = 10f;
    public float qteCooldown = 1f;   // เวลาพักหลัง QTE
    float qteTimer = 0f;

    [HideInInspector]
    public bool isPlayerInQTE = false; // 🔒 ล็อกยิงตอน QTE

    float fireTimer;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        // ⏳ ถ้าอยู่ในช่วงพัก QTE
        if (qteTimer > 0)
        {
            qteTimer -= Time.deltaTime;
            return;
        }

        // ❌ ถ้าอยู่ใน QTE → หยุดยิง
        if (isPlayerInQTE)
            return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= detectRange)
        {
            Aim();

            fireTimer -= Time.deltaTime;

            if (fireTimer <= 0)
            {
                Fire();
                fireTimer = fireRate;
            }
        }
    }
    void Aim()
    {
        Vector2 dir = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Fire()
    {
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.velocity = firePoint.right * bulletSpeed;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
