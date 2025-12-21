using UnityEngine;

public class TrapRespawn : MonoBehaviour
{
    public string playerTag = "Player";

    [Header("Original Spawn")]
    public Transform originalSpawnPoint;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

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
    }
}
