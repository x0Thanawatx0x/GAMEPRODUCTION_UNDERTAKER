using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class AutoSaveTrigger : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text saveText;

    [Header("Setting")]
    public float finishDelay = 1.5f;

    [Header("Player Stats")]
    public PlayerStats playerStats;

    void Start()
    {
        if (saveText != null) saveText.gameObject.SetActive(false);
    }

    public IEnumerator SaveProcess(Transform player)
    {
        if (saveText != null)
        {
            saveText.gameObject.SetActive(true);
            saveText.text = "Saving...";
        }

        yield return new WaitForSeconds(0.5f);

        // บันทึกตำแหน่งและฉาก
        PlayerPrefs.SetString("SavedScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("PlayerX", player.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.position.y);
        PlayerPrefs.SetFloat("PlayerZ", player.position.z);

        // ✅ บันทึกค่าพลังและเลเวลทั้งหมด
        if (playerStats != null) playerStats.Save();

        PlayerPrefs.Save();

        if (saveText != null) saveText.text = "Finish";

        yield return new WaitForSeconds(finishDelay);
        if (saveText != null) saveText.gameObject.SetActive(false);
    }

    public void LoadPlayer(Transform player)
    {
        if (PlayerPrefs.HasKey("PlayerX"))
        {
            player.position = new Vector3(
                PlayerPrefs.GetFloat("PlayerX"),
                PlayerPrefs.GetFloat("PlayerY"),
                PlayerPrefs.GetFloat("PlayerZ")
            );
        }

        // ✅ โหลดค่าพลังและเลเวลทั้งหมด
        if (playerStats != null) playerStats.Load();
    }

    // 🔥 ฟังก์ชันใหม่สำหรับปุ่ม ClearSave
    public void ClearAllSaveData()
    {
        // 1. ลบข้อมูลทั้งหมดที่อยู่ใน PlayerPrefs (ตำแหน่ง, ฉาก, ค่าพลังที่เซฟไว้)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // 2. รีเซ็ตค่าใน PlayerStats (ScriptableObject) ให้กลับเป็นค่าเริ่มต้น
        if (playerStats != null)
        {
            // รีเซ็ตเลเวลเป็น 0
            playerStats.speedLevel = 0;
            playerStats.jumpLevel = 0;
            playerStats.cooldownLevel = 0;
            playerStats.rangeLevel = 0;
            playerStats.attackChargeLevel = 0;

            // รีเซ็ตความสามารถพิเศษ
            playerStats.canDoubleJump = false;

            // (ถ้าต้องการรีเซ็ตค่า float พื้นฐานด้วย สามารถกำหนดค่าที่นี่ได้เลย)
            // playerStats.runSpeed = 5f; 
            // playerStats.jumpForce = 10f;

            // สั่ง Save ทับข้อมูลที่ว่างเปล่าลงไปอีกที
            playerStats.Save();
        }

        Debug.Log("Save Data Cleared and Levels Reset to 0!");

        // 3. รีโหลดฉากปัจจุบันเพื่อให้ตำแหน่งตัวละครกลับไปจุดเริ่มต้นของ Map
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}