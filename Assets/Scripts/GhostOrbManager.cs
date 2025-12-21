using System.Collections.Generic;
using UnityEngine;

public class GhostOrbManager : MonoBehaviour
{
    public float orbitRadius = 1.5f;
    public float rotateSpeed = 120f;
    public float returnSpeed = 5f; // ความเร็วบินกลับบ้าน

    private List<Transform> orbs = new List<Transform>();
    private float angleOffset;

    // เก็บ orb ที่กำลังบินกลับบ้าน
    private Dictionary<Transform, Vector3> returningOrbs = new Dictionary<Transform, Vector3>();

    void Update()
    {
        // ================== โคจรรอบผู้เล่น ==================
        if (orbs.Count > 0)
        {
            angleOffset += rotateSpeed * Time.deltaTime;
            float angleStep = 360f / orbs.Count;

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

        // ================== Orb "บินกลับบ้าน" ==================
        List<Transform> finished = new List<Transform>();
        foreach (var pair in returningOrbs)
        {
            Transform orb = pair.Key;
            Vector3 home = pair.Value;

            orb.position = Vector3.MoveTowards(orb.position, home, returnSpeed * Time.deltaTime);

            if (Vector3.Distance(orb.position, home) < 0.01f)
            {
                finished.Add(orb); // ถึงบ้านแล้ว
            }
        }

        // รีเซ็ต orb ที่ถึงบ้าน
        foreach (Transform orb in finished)
        {
            returningOrbs.Remove(orb);
            orbs.Remove(orb); // เลิกโคจรรอบผู้เล่น

            // คืนสถานะ orb ให้สามารถเก็บได้อีก
            orb.gameObject.tag = "GhostOrb";

            if (orb.GetComponent<Collider2D>() == null)
            {
                CircleCollider2D col = orb.gameObject.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("GhostOrb"))
        {
            AddOrb(other.gameObject);
        }
    }

    void AddOrb(GameObject orb)
    {
        Transform t = orb.transform;
        if (orbs.Contains(t)) return;

        orbs.Add(t);
        Destroy(orb.GetComponent<Collider2D>());
    }

    // ⭐ ฟังก์ชันสำคัญ
    public int GetOrbCount()
    {
        return orbs.Count;
    }

    public List<Transform> GetOrbitingOrbs()
    {
        return orbs;
    }

    // ================== ฟังก์ชันให้ orb บินกลับบ้าน ==================
    public void ReturnOrbHome(Transform orb, Vector3 homePosition)
    {
        if (!returningOrbs.ContainsKey(orb))
        {
            returningOrbs.Add(orb, homePosition);
        }
    }

    // ================== ลบ orb ทั้งหมด ==================
    public void ClearOrbs()
    {
        orbs.Clear();
        returningOrbs.Clear();
    }
}
