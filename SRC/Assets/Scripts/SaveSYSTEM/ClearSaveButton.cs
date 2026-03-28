using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearSaveButton : MonoBehaviour
{
    [Header("Player Stats Reference")]
    public PlayerStats playerStats; // ลากไฟล์ PlayerStats ใส่ตรงนี้ใน Inspector

    public void ClearSave()
    {
        // 1. ลบข้อมูลตำแหน่งใน PlayerPrefs
        PlayerPrefs.DeleteKey("PlayerX");
        PlayerPrefs.DeleteKey("PlayerY");
        PlayerPrefs.DeleteKey("PlayerZ");
        PlayerPrefs.DeleteKey("SavedScene");

        // 2. รีเซ็ตค่าใน PlayerStats ให้เป็น 0 (เพื่อให้ UI ช่องๆ หายไป)
        if (playerStats != null)
        {
            // รีเซ็ตเลเวลทั้งหมดเป็น 0
            playerStats.speedLevel = 0;
            playerStats.jumpLevel = 0;
            playerStats.cooldownLevel = 0;
            playerStats.rangeLevel = 0;
            playerStats.attackChargeLevel = 0;

            // รีเซ็ตความสามารถพิเศษ (ถ้ามี)
            playerStats.canDoubleJump = false;

            // สั่ง Save ทับลงไปเพื่อให้ค่าที่รีเซ็ตถูกบันทึกลงเครื่องจริงๆ
            playerStats.Save();
        }

        // 3. บันทึกการลบ
        PlayerPrefs.Save();

        Debug.Log("Save data cleared and levels reset!");

        // 4. (แนะนำ) รีโหลดฉากปัจจุบันเพื่อให้ตัวละครกลับไปจุดเริ่มต้น และ UI อัปเดต
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}