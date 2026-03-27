using UnityEngine;

public class CloneSwitcher : MonoBehaviour
{
    [Header("Clone")]
    public GameObject clonePrefab;
    public KeyCode switchKey = KeyCode.J;

    [Header("Stats")]
    [SerializeField] PlayerStats playerStats;

    // 🔥 ================= แก้ตรงนี้ =================
    [Header("Range Visual")]
    [SerializeField] Transform rangeVisual;
    [SerializeField] float baseRange = 5f;

    Vector3 originalScale;
    // ==============================================

    GameObject currentClone;
    PlayerControllerMain playerController;

    bool isControllingClone = false;

    void Start()
    {
        playerController = GetComponent<PlayerControllerMain>();

        if (rangeVisual != null)
        {
            originalScale = rangeVisual.localScale;

            // ✅ เริ่มเกม = ซ่อนก่อน
            rangeVisual.gameObject.SetActive(false);
        }

        baseRange = playerStats.cloneRange;
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
            TrySpawnClone();

        if (Input.GetKey(switchKey) && currentClone != null)
        {
            SwitchToClone();
            CheckRange();
        }

        if (Input.GetKeyUp(switchKey))
            ReturnToPlayer();

        // 🔥 อัปเดต scale + การแสดงผล
        UpdateRangeVisual();
    }

    void TrySpawnClone()
    {
        if (currentClone != null) return;
        if (!playerController.CanUseClone()) return;

        currentClone = Instantiate(clonePrefab, transform.position, transform.rotation);
        SetupCloneCollisions(currentClone);
    }

    void SetupCloneCollisions(GameObject clone)
    {
        Collider2D cloneCollider = clone.GetComponent<Collider2D>();
        if (cloneCollider == null) return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            Collider2D pCol = p.GetComponent<Collider2D>();
            if (pCol != null)
                Physics2D.IgnoreCollision(cloneCollider, pCol, true);
        }

        GameObject[] walls = GameObject.FindGameObjectsWithTag("SpriteWall");
        foreach (GameObject w in walls)
        {
            Collider2D wCol = w.GetComponent<Collider2D>();
            if (wCol != null)
                Physics2D.IgnoreCollision(cloneCollider, wCol, true);
        }
    }

    void SwitchToClone()
    {
        if (isControllingClone) return;

        playerController.EnableControl(false);
        playerController.SetCloneMode(true);

        CloneController clone = currentClone.GetComponent<CloneController>();
        if (clone != null)
            clone.EnableControl(true);

        isControllingClone = true;
    }

    void ReturnToPlayer()
    {
        if (currentClone == null) return;

        transform.position = currentClone.transform.position;
        Destroy(currentClone);

        playerController.EnableControl(true);
        playerController.SetCloneMode(false);
        playerController.StartCloneCooldown();

        isControllingClone = false;
    }

    void CheckRange()
    {
        if (currentClone == null) return;

        float distance = Vector2.Distance(transform.position, currentClone.transform.position);

        if (distance > playerStats.cloneRange)
        {
            Vector2 dir = (currentClone.transform.position - transform.position).normalized;

            currentClone.transform.position =
                (Vector2)transform.position + dir * playerStats.cloneRange;

            Rigidbody2D rb = currentClone.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.velocity = Vector2.zero;
        }
    }

    // 🔥 ================= ส่วนสำคัญ =================
    void UpdateRangeVisual()
    {
        if (rangeVisual == null) return;

        // ✅ แสดงเฉพาะตอนกด J ค้าง
        bool show = Input.GetKey(switchKey);

        if (rangeVisual.gameObject.activeSelf != show)
            rangeVisual.gameObject.SetActive(show);

        if (!show) return;

        float scaleMultiplier = playerStats.cloneRange / baseRange;
        Vector3 targetScale = originalScale * scaleMultiplier;

        rangeVisual.localScale = Vector3.Lerp(
            rangeVisual.localScale,
            targetScale,
            Time.deltaTime * 5f
        );

        CircleCollider2D col = rangeVisual.GetComponent<CircleCollider2D>();
        if (col != null)
        {
            col.radius = playerStats.cloneRange;
        }
    }
    // ============================================

    public void ForceCancelClone()
    {
        if (currentClone == null) return;

        playerController.EnableControl(true);
        playerController.SetCloneMode(false);

        Destroy(currentClone);
        playerController.StartCloneCooldown();

        isControllingClone = false;
    }
}