using UnityEngine;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    public GameObject statusPanel;
    public PlayerStats stats;

    [Header("UI Text Elements")]
    public TextMeshProUGUI runSpeedText;
    public TextMeshProUGUI jumpForceText;
    public TextMeshProUGUI doubleJumpText;
    public TextMeshProUGUI cooldownText;

    // 🔥 เพิ่มส่วนที่ขาดไป
    public TextMeshProUGUI cloneRangeText;
    public TextMeshProUGUI attackChargeText;

    bool isOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleStatus();
        }
    }

    void ToggleStatus()
    {
        isOpen = !isOpen;
        statusPanel.SetActive(isOpen);

        if (isOpen)
        {
            Time.timeScale = 0; // หยุดเกมชั่วขณะ
            UpdateUI();
        }
        else
        {
            Time.timeScale = 1; // เล่นเกมต่อ
        }
    }

    void UpdateUI()
    {
        // ตรวจสอบเผื่อลืมลาก ScriptableObject มาใส่ใน Inspector
        if (stats == null) return;

        runSpeedText.text = "Run Speed : " + stats.runSpeed.ToString("F1");
        jumpForceText.text = "Jump Force : " + stats.jumpForce.ToString("F1");
        doubleJumpText.text = "Double Jump : " + (stats.canDoubleJump ? "Yes" : "No");
        cooldownText.text = "Body Swap CD : " + stats.bodySwapCooldown.ToString("F1") + "s";

        // 🔥 แสดงค่าใหม่ที่เพิ่มเข้ามา
        if (cloneRangeText != null)
            cloneRangeText.text = "Clone Range : " + stats.cloneRange.ToString("F1");

        if (attackChargeText != null)
            attackChargeText.text = "Attack Charge : " + stats.attackChargeTime.ToString("F1") + "s";
    }
}