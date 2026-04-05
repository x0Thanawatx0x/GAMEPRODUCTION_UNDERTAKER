using UnityEngine;

public class LavaDrop : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // เช็คว่าเป็น Player
        if (!other.CompareTag("Player")) return;

        // หา WarpPoint
        GameObject warp = GameObject.FindGameObjectWithTag("WarpPoint");

        if (warp != null)
        {
            // วาร์ปไปตำแหน่งนั้น
            other.transform.position = warp.transform.position;
        }
        else
        {
            Debug.LogWarning("❌ ไม่เจอ WarpPoint!");
        }
    }
}