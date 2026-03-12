using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float force = 10f;
    public float lifeTime = 5f;

    [Header("Curve Settings")]
    public AnimationCurve curve;
    public float curveStrength = 3f;

    [Header("Trap Settings")]
    public string playerTag = "Player";
    public string spawnTag = "PlayerSpawn";

    private Rigidbody2D rb;
    private Transform spawnPoint;
    private bool hasTriggered = false;
    private float timer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // ยิงกระสุน
        rb.velocity = transform.right * force;

        // หา Spawn Point
        GameObject spawnObj = GameObject.FindGameObjectWithTag(spawnTag);
        if (spawnObj != null)
            spawnPoint = spawnObj.transform;

        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        // เพิ่มแรงโค้ง
        timer += Time.fixedDeltaTime;

        float curveValue = curve.Evaluate(timer);
        Vector2 curveForce = Vector2.up * curveValue * curveStrength;

        rb.AddForce(curveForce);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        // ===== โดนผู้เล่น =====
        if (other.CompareTag(playerTag))
        {
            hasTriggered = true;

            PlayerLifeManager lifeManager = other.GetComponent<PlayerLifeManager>();

            if (lifeManager != null)
            {
                lifeManager.CountTrap();
                lifeManager.ResetGhost(); // 🔥 รีเซ็ตวิญญาณ
            }

            Vector3 respawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
            other.transform.position = respawnPos;

            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = Vector2.zero;
                playerRb.angularVelocity = 0f;
            }

            if (lifeManager != null)
                lifeManager.ResetTrapCountLock();

            Destroy(gameObject);
            return;
        }

        // ===== โดนพื้น =====
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}