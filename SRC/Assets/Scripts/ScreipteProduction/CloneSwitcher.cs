using UnityEngine;

public class CloneSwitcher : MonoBehaviour
{
    [Header("=== Clone Settings ===")]
    public GameObject clonePrefab;

    [Header("=== Switch Key ===")]
    public KeyCode switchKey = KeyCode.J;

    [Header("=== Clone Range UI ===")]
    public GameObject rangeUI;
    public float cloneRange = 5f;

    private GameObject currentClone;

    private PlayerControllerMain playerController;
    private bool isControllingClone = false;

    void Start()
    {
        playerController = GetComponent<PlayerControllerMain>();

        if (rangeUI != null)
        {
            rangeUI.SetActive(false);
            rangeUI.transform.SetParent(transform);
            rangeUI.transform.localPosition = Vector3.zero;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            TrySpawnClone();
            ShowRangeUI(true);
        }

        if (Input.GetKey(switchKey) && currentClone != null)
        {
            SwitchToClone();
            UpdateRangeUI();
        }

        if (Input.GetKeyUp(switchKey))
        {
            ShowRangeUI(false);

            if (currentClone != null)
                ReturnToPlayer();
        }
    }

    void TrySpawnClone()
    {
        if (currentClone != null) return;

        if (!playerController.CanUseClone())
        {
            Debug.Log("<color=red>ยังติดคูลดาวน์!</color>");
            return;
        }

        currentClone = Instantiate(
            clonePrefab,
            transform.position,
            transform.rotation
        );

        IgnoreCloneCollision(currentClone);
    }

    void SwitchToClone()
    {
        if (isControllingClone) return;

        playerController.EnableControl(false);

        CloneController cloneController =
            currentClone.GetComponent<CloneController>();

        if (cloneController != null)
            cloneController.EnableControl(true);

        isControllingClone = true;
    }

    void ReturnToPlayer()
    {
        transform.position = currentClone.transform.position;

        playerController.EnableControl(true);

        Destroy(currentClone);

        playerController.StartCloneCooldown();

        isControllingClone = false;
    }

    // 💥🔥 ฟังก์ชันใหม่ (สำคัญมาก)
    public void ForceCancelClone()
    {
        if (currentClone == null) return;

        playerController.EnableControl(true);

        Destroy(currentClone);

        playerController.StartCloneCooldown();

        isControllingClone = false;

        Debug.Log("<color=yellow>Clone ถูกยกเลิกโดย Trap!</color>");
    }

    // =========================
    // RANGE UI
    // =========================

    void ShowRangeUI(bool show)
    {
        if (rangeUI == null) return;

        rangeUI.SetActive(show);

        if (show)
        {
            rangeUI.transform.SetParent(transform);
            rangeUI.transform.localPosition = Vector3.zero;
            UpdateRangeUI();
        }
    }

    void UpdateRangeUI()
    {
        if (rangeUI == null) return;

        float size = cloneRange * 2f;

        rangeUI.transform.localScale = new Vector3(size, size, 1f);
    }

    // =========================
    // IGNORE COLLISION
    // =========================

    void IgnoreCloneCollision(GameObject clone)
    {
        Collider2D cloneCol = clone.GetComponent<Collider2D>();

        if (cloneCol == null) return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            Collider2D col = p.GetComponent<Collider2D>();

            if (col != null)
                Physics2D.IgnoreCollision(cloneCol, col, true);
        }

        GameObject[] walls = GameObject.FindGameObjectsWithTag("SpriteWall");

        foreach (GameObject w in walls)
        {
            Collider2D col = w.GetComponent<Collider2D>();

            if (col != null)
                Physics2D.IgnoreCollision(cloneCol, col, true);
        }
    }
}