using UnityEngine;

public class TrapRespawn : MonoBehaviour
{
    public string playerTag = "Player";

    [Header("Original Spawn")]
    public Transform originalSpawnPoint;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Something entered trap: " + other.name);

        if (!other.CompareTag(playerTag)) return;

        Debug.Log("Player hit trap!");

        PlayerLifeManager lifeManager = other.GetComponent<PlayerLifeManager>();

        if (lifeManager != null)
        {
            Debug.Log("Counting trap and resetting ghost");
            lifeManager.CountTrap();
            lifeManager.ResetGhost();
        }

        // ⭐ Respawn Ghost Orbs
        Debug.Log("Respawning all Ghost Orbs...");
        GhostOrbRespawn.RespawnAll();

        Vector3 respawnPos;

        if (PlayerPrefs.HasKey("PlayerX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerX");
            float y = PlayerPrefs.GetFloat("PlayerY");
            float z = PlayerPrefs.GetFloat("PlayerZ");

            respawnPos = new Vector3(x, y, z);
            Debug.Log("Respawn from PlayerPrefs: " + respawnPos);
        }
        else
        {
            respawnPos = originalSpawnPoint.position;
            Debug.Log("Respawn from original spawn point: " + respawnPos);
        }

        other.transform.position = respawnPos;

        if (lifeManager != null)
        {
            lifeManager.ResetTrapCountLock();
        }
    }
}