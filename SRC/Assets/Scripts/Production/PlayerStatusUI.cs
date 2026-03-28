using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    public GameObject statusPanel;
    public PlayerStats stats;

    [System.Serializable]
    public class StatSlotGroup
    {
        public List<Image> slots; // ลาก Image ช่องๆ มาใส่ใน Inspector
        public Color activeColor = Color.white;
        public Color inactiveColor = new Color(1, 1, 1, 0.2f);
    }

    [Header("UI Slot Groups")]
    public StatSlotGroup speedGroup;
    public StatSlotGroup jumpGroup;
    public StatSlotGroup cooldownGroup;
    public StatSlotGroup rangeGroup;
    public StatSlotGroup attackChargeGroup;

    [Header("Other UI")]
    public TextMeshProUGUI doubleJumpText;

    bool isOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) ToggleStatus();
    }

    void ToggleStatus()
    {
        isOpen = !isOpen;
        statusPanel.SetActive(isOpen);
        Time.timeScale = isOpen ? 0 : 1;
        if (isOpen) UpdateUI();
    }

    void UpdateUI()
    {
        if (stats == null) return;

        // อัปเดตสีของช่อง Image ตามเลเวลปัจจุบัน
        UpdateSlots(speedGroup, stats.speedLevel);
        UpdateSlots(jumpGroup, stats.jumpLevel);
        UpdateSlots(cooldownGroup, stats.cooldownLevel);
        UpdateSlots(rangeGroup, stats.rangeLevel);
        UpdateSlots(attackChargeGroup, stats.attackChargeLevel);

        if (doubleJumpText != null)
            doubleJumpText.text = "Double Jump : " + (stats.canDoubleJump ? "YES" : "NO");
    }

    void UpdateSlots(StatSlotGroup group, int currentLevel)
    {
        for (int i = 0; i < group.slots.Count; i++)
        {
            group.slots[i].color = (i < currentLevel) ? group.activeColor : group.inactiveColor;
        }
    }
}