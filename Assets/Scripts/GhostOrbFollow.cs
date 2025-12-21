using System.Collections.Generic;
using UnityEngine;

public class GhostOrbManager : MonoBehaviour
{
    public float orbitRadius = 1.5f;
    public float rotateSpeed = 120f;

    private List<Transform> orbs = new List<Transform>();
    private float angleOffset;

    void Update()
    {
        if (orbs.Count == 0) return;

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

            orbs[i].position = Vector2.Lerp(
                orbs[i].position,
                targetPos,
                8f * Time.deltaTime
            );
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

    // ⭐ สำคัญ
    public int GetOrbCount()
    {
        return orbs.Count;
    }
}
