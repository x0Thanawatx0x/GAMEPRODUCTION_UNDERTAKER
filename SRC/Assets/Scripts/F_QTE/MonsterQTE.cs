using UnityEngine;

public class MonsterQTE : MonoBehaviour
{
    [Header("=== QTE Settings ===")]
    public int requiredSuccessCount = 3;
    int currentSuccessCount = 0;

    [Header("=== Result Prefab ===")]
    public GameObject resultPrefab;
    public int spawnCount = 1;
    public float spawnSpacing = 0.5f;

    [Header("=== Knockback Settings ===")]
    public float knockbackForce = 8f;
    public float disableColliderTime = 0.5f;

    CircleCollider2D circleCollider;
    bool isFinished = false;

    CannonTurretStraight turret;   // 🔫 อ้างอิงป้อมปืน

    void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        turret = FindObjectOfType<CannonTurretStraight>(); // หา turret ในฉาก
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isFinished) return;

        if (other.CompareTag("Player"))
        {
            // 🔥 สั่งป้อมหยุดยิง
            if (turret != null)
                turret.isPlayerInQTE = true;

            QTEManager.Instance?.StartQTE(this);
        }
    }

    // ================== QTE CALLBACK ==================

    public void OnQTESuccess()
    {
        if (isFinished) return;

        currentSuccessCount++;
        KnockbackPlayer();

        // 🔫 QTE จบ → ยิงต่อ
        if (turret != null)
            turret.isPlayerInQTE = false;

        if (currentSuccessCount >= requiredSuccessCount)
        {
            TransformToResult();
        }
    }

    public void OnQTEFail()
    {
        if (isFinished) return;

        KnockbackPlayer();

        // 🔫 QTE จบ → ยิงต่อ
        if (turret != null)
            turret.isPlayerInQTE = false;
    }

    // ================== LOGIC ==================

    void TransformToResult()
    {
        if (isFinished) return;
        isFinished = true;

        if (resultPrefab != null)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 offset = new Vector3(
                    (i - (spawnCount - 1) / 2f) * spawnSpacing,
                    0f,
                    0f
                );

                Instantiate(resultPrefab, transform.position + offset, transform.rotation);
            }
        }

        Destroy(gameObject);
    }

    void KnockbackPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        Vector2 dir = (player.transform.position - transform.position).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);

        if (circleCollider != null)
        {
            circleCollider.enabled = false;
            Invoke(nameof(EnableCollider), disableColliderTime);
        }
    }

    void EnableCollider()
    {
        if (!isFinished && circleCollider != null)
        {
            circleCollider.enabled = true;
        }
    }
}
