using UnityEngine;

public class MonsterQTE : MonoBehaviour
{
    [Header("=== QTE Settings ===")]
    public int requiredSuccessCount = 3;   // ต้องผ่านกี่ครั้ง
    int currentSuccessCount = 0;

    [Header("=== Result Prefab ===")]
    public GameObject resultPrefab;        // prefab หลังชนะ

    bool playerInside;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            TryStartQTE();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    void TryStartQTE()
    {
        // เรียกได้ทุกครั้งที่เข้า
        if (QTEManager.Instance != null)
        {
            QTEManager.Instance.StartQTE(this);
        }
    }

    // ================== QTE CALLBACK ==================

    public void OnQTESuccess()
    {
        currentSuccessCount++;

        Debug.Log($"QTE Success {currentSuccessCount}/{requiredSuccessCount}");

        if (currentSuccessCount >= requiredSuccessCount)
        {
            TransformToResult();
        }
        else
        {
            // ถ้ายังไม่ครบ → ออกแล้วเข้าใหม่ จะขึ้นอีก
        }
    }

    public void OnQTEFail()
    {
        Debug.Log("QTE Failed");
        // จะให้รีเซ็ต count หรือไม่ก็ได้
        // currentSuccessCount = 0;
    }

    void TransformToResult()
    {
        if (resultPrefab != null)
        {
            Instantiate(resultPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject); // ลบมอนสเตอร์ตัวเดิม
    }
}
