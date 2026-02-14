using UnityEngine;

public class DoorSceneTrigger : MonoBehaviour
{
    public int requiredOrbAmount = 3;   // ต้องมีอย่างน้อยกี่วิญญาณถึงจะผ่าน
    public UpgradeManager upgradeManager; // ลากใส่ใน Inspector

    void OnTriggerEnter2D(Collider2D other)
    {
        GhostOrbManager orbManager = other.GetComponent<GhostOrbManager>();
        PlayerLifeManager lifeManager = other.GetComponent<PlayerLifeManager>();

        if (orbManager == null || lifeManager == null) return;

        if (orbManager.GetOrbCount() >= requiredOrbAmount)
        {
            // 🔥 แปลง Ghost → Money
            lifeManager.ConvertGhostToMoney();

            // ล้าง orb รอบตัวผู้เล่น
            orbManager.ClearOrbs();

            // 🃏 เปิด Panel การ์ด
            if (upgradeManager != null)
                upgradeManager.ShowUpgradePanel();

            Debug.Log("ผ่านละ! แปลงวิญญาณเป็นเงินแล้ว + เปิดการ์ด");
        }
        else
        {
            Debug.Log("วิญญาณยังไม่พอ");
        }
    }
}
