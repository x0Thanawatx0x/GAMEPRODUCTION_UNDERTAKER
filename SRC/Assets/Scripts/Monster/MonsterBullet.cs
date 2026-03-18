using UnityEngine;

public class MonsterBullet : MonoBehaviour
{
    public string playerTag = "Player";
    public Vector3 warpPosition;   // ใส่ค่าตำแหน่งเอง
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.position = warpPosition;
        }
        else
        {
            other.transform.position = warpPosition;
        }

        Destroy(gameObject);
    }
}