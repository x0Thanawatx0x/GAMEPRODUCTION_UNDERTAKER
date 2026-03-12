using UnityEngine;

public class CloneSwitcher : MonoBehaviour
{
    [Header("=== Clone Settings ===")]
    public GameObject clonePrefab;

    [Header("=== Switch Key ===")]
    public KeyCode switchKey = KeyCode.J;

    private GameObject currentClone;

    private PlayerControllerMain playerController;
    private bool isControllingClone = false;

    void Start()
    {
        playerController = GetComponent<PlayerControllerMain>();
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            TrySpawnClone();
        }

        if (Input.GetKey(switchKey) && currentClone != null)
        {
            SwitchToClone();
        }

        if (Input.GetKeyUp(switchKey) && currentClone != null)
        {
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

        // 🔥 ทำให้ Clone ทะลุ Player และ SpriteWall
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

    void IgnoreCloneCollision(GameObject clone)
    {
        Collider2D cloneCol = clone.GetComponent<Collider2D>();

        if (cloneCol == null) return;

        // หา Player ทั้งหมด
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            Collider2D col = p.GetComponent<Collider2D>();

            if (col != null)
                Physics2D.IgnoreCollision(cloneCol, col, true);
        }

        // หา SpriteWall ทั้งหมด
        GameObject[] walls = GameObject.FindGameObjectsWithTag("SpriteWall");

        foreach (GameObject w in walls)
        {
            Collider2D col = w.GetComponent<Collider2D>();

            if (col != null)
                Physics2D.IgnoreCollision(cloneCol, col, true);
        }
    }
}