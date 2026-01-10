using UnityEngine;

public class MonsterQTE : MonoBehaviour
{
    [Header("=== QTE Settings ===")]
    public int requiredSuccessCount = 3;
    int currentSuccessCount = 0;

    [Header("=== Result Prefab ===")]
    public GameObject resultPrefab;
    public int spawnCount = 1;              // ⭐ จำนวน prefab ที่จะออก
    public float spawnSpacing = 0.5f;       // ⭐ ระยะห่างระหว่าง prefab

    [Header("=== Knockback Settings ===")]
    public float knockbackForce = 8f;
    public float disableColliderTime = 0.5f;

    CircleCollider2D circleCollider;
    bool isFinished = false;   // 🔒 ตัวล็อกสำคัญมาก

    void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isFinished) return;

        if (other.CompareTag("Player"))
        {
            QTEManager.Instance?.StartQTE(this);
        }
    }

    // ================== QTE CALLBACK ==================

    public void OnQTESuccess()
    {
        if (isFinished) return;

        currentSuccessCount++;
        KnockbackPlayer();

        if (currentSuccessCount >= requiredSuccessCount)
        {
            TransformToResult();
        }
    }

    public void OnQTEFail()
    {
        if (isFinished) return;

        KnockbackPlayer();
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

                Instantiate(
                    resultPrefab,
                    transform.position + offset,
                    transform.rotation
                );
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
