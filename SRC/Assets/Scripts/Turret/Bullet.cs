using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float force = 10f;
    public float lifeTime = 5f;

    [Header("Curve Settings")]
    public AnimationCurve curve;     // เส้นโค้ง
    public float curveStrength = 3f; // ความแรงของโค้ง

    [Header("Trap Settings")]
    public string playerTag = "Player";
    public string spawnTag = "PlayerSpawn";

    [Header("GhostOrb Original Positions")]
    public List<Transform> ghostOrbOriginals = new List<Transform>();

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
        // ยิงกระสุนไปข้างหน้า
        rb.velocity = transform.right * force;

        // หา Spawn Point
        GameObject spawnObj = GameObject.FindGameObjectWithTag(spawnTag);
        if (spawnObj != null)
            spawnPoint = spawnObj.transform;

        // เก็บตำแหน่ง GhostOrb เดิม
        GameObject[] orbs = GameObject.FindGameObjectsWithTag("GhostOrb");
        ghostOrbOriginals.Clear();

        foreach (GameObject orb in orbs)
        {
            GameObject temp = new GameObject("OrbOriginalPos");
            temp.transform.position = orb.transform.position;
            ghostOrbOriginals.Add(temp.transform);
        }

        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        // เพิ่มแรงโค้งให้กระสุน
        timer += Time.fixedDeltaTime;

        float curveValue = curve.Evaluate(timer);
        Vector2 curveForce = Vector2.up * curveValue * curveStrength;

        rb.AddForce(curveForce);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        // ===== โดนผู้เล่น → Trap =====
        if (other.CompareTag(playerTag))
        {
            hasTriggered = true;

            PlayerLifeManager lifeManager = other.GetComponent<PlayerLifeManager>();
            if (lifeManager != null)
                lifeManager.CountTrap();

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

            GhostOrbManager ghostManager = other.GetComponent<GhostOrbManager>();
            if (ghostManager != null && ghostManager.GetOrbCount() > 0)
            {
                List<Transform> playerOrbs =
                    new List<Transform>(ghostManager.GetOrbitingOrbs());

                for (int i = 0; i < playerOrbs.Count && i < ghostOrbOriginals.Count; i++)
                {
                    ghostManager.ReturnOrbHome(
                        playerOrbs[i],
                        ghostOrbOriginals[i].position
                    );
                }
            }

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
