using UnityEngine;

public class BulletRespawn : MonoBehaviour
{
    public string playerTag = "Player";
    Transform originalSpawnPoint;

    void Start()
    {
        GameObject spawn = GameObject.Find("PlayerSpawn");

        if (spawn != null)
        {
            originalSpawnPoint = spawn.transform;
        }
        else
        {
            Debug.LogError("PlayerSpawn not found!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (originalSpawnPoint == null) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.position = originalSpawnPoint.position;
        }
        else
        {
            other.transform.position = originalSpawnPoint.position;
        }

        Destroy(gameObject);
    }
}