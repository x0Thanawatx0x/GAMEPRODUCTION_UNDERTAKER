using UnityEngine;

public class MonsterBullet : MonoBehaviour
{
    public string playerTag = "Player";
    public string warpTag = "WarpPoint"; // Tag ของ Warp Point
    public float lifeTime = 5f;

    [Header("=== Hit Layers (Ground + Wall) ===")]
    public LayerMask hitLayers;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ถ้าโดน Ground หรือ Wall → ทำลายกระสุน
        if (((1 << other.gameObject.layer) & hitLayers) != 0)
        {
            Debug.Log($"Bullet hit {other.name} (layer {other.gameObject.layer}) → destroyed");
            Destroy(gameObject);
            return;
        }

        // ถ้าโดน Player
        if (!other.CompareTag(playerTag)) return;

        // หา Warp Point ตาม Tag
        GameObject warpObj = GameObject.FindWithTag(warpTag);
        if (warpObj == null)
        {
            Debug.LogWarning("Warp Point not found in Scene!");
            return;
        }

        Vector3 warpPosition = warpObj.transform.position;
        Debug.Log($"Player hit by bullet! Warping to {warpPosition}");

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.position = warpPosition;
        }
        else
        {
            other.transform.position = warpPosition;
        }

        Destroy(gameObject);
        Debug.Log("Bullet destroyed after hitting player");
    }
}