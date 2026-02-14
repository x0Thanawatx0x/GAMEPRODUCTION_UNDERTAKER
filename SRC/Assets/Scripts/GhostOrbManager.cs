using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostOrbManager : MonoBehaviour
{
    [Header("=== Orbit Settings ===")]
    public float orbitRadius = 1.5f;
    public float rotateSpeed = 120f;
    public float returnSpeed = 5f;

    [Header("=== Pickup Settings ===")]
    [SerializeField] float pickupRadius = 0.8f;
    [SerializeField] KeyCode pickupKey = KeyCode.E;
    [SerializeField] float pickupCooldown = 0.2f;
    [SerializeField] float facingDotThreshold = 0.3f;

    List<Transform> orbs = new List<Transform>();
    Dictionary<Transform, Vector3> returningOrbs = new Dictionary<Transform, Vector3>();

    bool isPicking;
    PlayerLifeManager lifeManager;   // 🔗 UI Manager

    void Start()
    {
        lifeManager = FindObjectOfType<PlayerLifeManager>();
    }

    void Update()
    {
        // ===== กด E เพื่อเก็บ =====
        if (Input.GetKeyDown(pickupKey) && !isPicking)
        {
            TryPickupOrb();
        }

        // ===== Orb โคจรรอบผู้เล่น =====
        if (orbs.Count > 0)
        {
            float angleStep = 360f / orbs.Count;
            float angleOffset = Time.time * rotateSpeed;

            for (int i = 0; i < orbs.Count; i++)
            {
                float angle = angleOffset + angleStep * i;
                float rad = angle * Mathf.Deg2Rad;

                Vector3 targetPos = transform.position + new Vector3(
                    Mathf.Cos(rad),
                    Mathf.Sin(rad),
                    0f
                ) * orbitRadius;

                if (!returningOrbs.ContainsKey(orbs[i]))
                {
                    orbs[i].position = Vector2.Lerp(
                        orbs[i].position,
                        targetPos,
                        8f * Time.deltaTime
                    );
                }
            }
        }

        // ===== Orb บินกลับบ้าน =====
        List<Transform> finished = new List<Transform>();
        foreach (var pair in returningOrbs)
        {
            Transform orb = pair.Key;
            Vector3 home = pair.Value;

            orb.position = Vector3.MoveTowards(orb.position, home, returnSpeed * Time.deltaTime);

            if (Vector3.Distance(orb.position, home) < 0.01f)
                finished.Add(orb);
        }

        foreach (Transform orb in finished)
        {
            returningOrbs.Remove(orb);
            orbs.Remove(orb);

            orb.tag = "GhostOrb";

            if (orb.GetComponent<Collider2D>() == null)
            {
                CircleCollider2D col = orb.gameObject.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
            }
        }
    }

    // ================= PICKUP =================

    void TryPickupOrb()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRadius);

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("GhostOrb")) continue;

            Vector2 toOrb = (hit.transform.position - transform.position).normalized;
            Vector2 facingDir = new Vector2(Mathf.Sign(transform.localScale.x), 0f);

            float dot = Vector2.Dot(facingDir, toOrb);
            if (dot < facingDotThreshold) continue;

            AddOrb(hit.gameObject);
            StartCoroutine(PickupCooldown());
            break;
        }
    }

    IEnumerator PickupCooldown()
    {
        isPicking = true;
        yield return new WaitForSeconds(pickupCooldown);
        isPicking = false;
    }

    void AddOrb(GameObject orb)
    {
        Transform t = orb.transform;
        if (orbs.Contains(t)) return;

        orbs.Add(t);
        orb.tag = "Untagged";
        Destroy(orb.GetComponent<Collider2D>());

        // 🔔 เพิ่มจำนวนวิญญาณใน UI
        if (lifeManager != null)
            lifeManager.AddGhost(1);
    }

    // ================= PUBLIC =================

    public int GetOrbCount()
    {
        return orbs.Count;
    }

    public List<Transform> GetOrbitingOrbs()
    {
        return orbs;
    }

    public void ReturnOrbHome(Transform orb, Vector3 homePosition)
    {
        if (!returningOrbs.ContainsKey(orb))
            returningOrbs.Add(orb, homePosition);
    }

    public void ClearOrbs()
    {
        orbs.Clear();
        returningOrbs.Clear();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
