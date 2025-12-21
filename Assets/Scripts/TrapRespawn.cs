using System.Collections.Generic;
using UnityEngine;

public class TrapRespawn : MonoBehaviour
{
    public string playerTag = "Player";

    [Header("Original Spawn")]
    public Transform originalSpawnPoint;

    [Header("GhostOrb Original Positions")]
    public List<Transform> ghostOrbOriginals = new List<Transform>();

    void Start()
    {
        // เก็บตำแหน่ง original ของ GhostOrb ทั้งหมดใน Scene
        GameObject[] orbs = GameObject.FindGameObjectsWithTag("GhostOrb");
        ghostOrbOriginals.Clear();
        foreach (GameObject orb in orbs)
        {
            GameObject temp = new GameObject("OrbOriginalPos");
            temp.transform.position = orb.transform.position;
            ghostOrbOriginals.Add(temp.transform);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // ================== Respawn Player ==================
        Vector3 respawnPos;

        if (PlayerPrefs.HasKey("PlayerX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerX");
            float y = PlayerPrefs.GetFloat("PlayerY");
            float z = PlayerPrefs.GetFloat("PlayerZ");
            respawnPos = new Vector3(x, y, z);
        }
        else
        {
            if (originalSpawnPoint == null)
            {
                Debug.LogError("Original Spawn Point not assigned!");
                return;
            }
            respawnPos = originalSpawnPoint.position;
        }

        other.transform.position = respawnPos;

        // ================== GhostOrb "บินกลับบ้าน" ==================
        GhostOrbManager ghostManager = other.GetComponent<GhostOrbManager>();
        if (ghostManager != null && ghostManager.GetOrbCount() > 0)
        {
            List<Transform> playerOrbs = new List<Transform>(ghostManager.GetOrbitingOrbs());

            for (int i = 0; i < playerOrbs.Count && i < ghostOrbOriginals.Count; i++)
            {
                ghostManager.ReturnOrbHome(playerOrbs[i], ghostOrbOriginals[i].position);
            }
        }
    }
}
