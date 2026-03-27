using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject upgradePanel;
    public Button option1Button, option2Button, option3Button;
    public TextMeshProUGUI option1Text, option2Text, option3Text;

    // 🔥 เปลี่ยน Reference เป็น CardVisualUI (ฉบับตัดระบบหมุน)
    [Header("Card Visual Components")]
    public CardVisualUI card1Visual;
    public CardVisualUI card2Visual;
    public CardVisualUI card3Visual;

    [Header("Card Sprites (6 Types)")]
    public Sprite speedSprite;
    public Sprite jumpSprite;
    public Sprite cooldownSprite;
    public Sprite cloneRangeSprite;
    public Sprite attackSpeedSprite;
    public Sprite doubleJumpSprite;

    [Header("Player Stats")]
    public PlayerStats playerStats;

    [Header("Upgrade Values")]
    public float speedUpgradeAmount = 1f;
    public float jumpUpgradeAmount = 2f;
    public float cooldownReduceAmount = 1f;
    public float cloneRangeUpgradeAmount = 1f;
    public float attackSpeedReduceAmount = 0.5f;

    public Action OnUpgradeComplete;

    enum UpgradeType { Speed, Jump, Cooldown, CloneRange, AttackSpeed, DoubleJump }
    UpgradeType[] currentOptions = new UpgradeType[3];

    [Header("Scene Upgrades")]
    public bool useFixedUpgrades = false;
    [SerializeField] UpgradeType[] fixedUpgrades = new UpgradeType[3];

    bool isShowing = false;

    void Start() { upgradePanel.SetActive(false); }

    public void ShowUpgradePanel()
    {
        if (isShowing) return;
        isShowing = true;
        upgradePanel.SetActive(true);
        Time.timeScale = 0f;
        GenerateUpgrades();
    }

    public void HideUpgradePanel()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
        isShowing = false;
    }

    void GenerateUpgrades()
    {
        // 1. Logic การสุ่ม
        if (useFixedUpgrades && fixedUpgrades.Length >= 3)
        {
            for (int i = 0; i < 3; i++) currentOptions[i] = fixedUpgrades[i];
        }
        else
        {
            for (int i = 0; i < 3; i++)
                currentOptions[i] = (UpgradeType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(UpgradeType)).Length);
        }

        // 2. อัปเดตข้อความ
        option1Text.text = GetUpgradeName(currentOptions[0]);
        option2Text.text = GetUpgradeName(currentOptions[1]);
        option3Text.text = GetUpgradeName(currentOptions[2]);

        // 🔥 3. ส่งรูป Sprite ไปอัปเดตภาพหน้าการ์ดตรงๆ ทันที ( No Flip)
        UpdateCardVisuals(card1Visual, currentOptions[0]);
        UpdateCardVisuals(card2Visual, currentOptions[1]);
        UpdateCardVisuals(card3Visual, currentOptions[2]);

        // 4. Setup ปุ่ม (เขียนให้ครบทั้ง 3 ปุ่ม)
        option1Button.onClick.RemoveAllListeners();
        option2Button.onClick.RemoveAllListeners();
        option3Button.onClick.RemoveAllListeners();

        option1Button.onClick.AddListener(() => ApplyUpgrade(currentOptions[0]));
        option2Button.onClick.AddListener(() => ApplyUpgrade(currentOptions[1]));
        option3Button.onClick.AddListener(() => ApplyUpgrade(currentOptions[2]));
    }

    // ฟังก์ชันสำหรับลากใส่ปุ่มใน Inspector
    public void SelectOption1() { ApplyUpgrade(currentOptions[0]); }
    public void SelectOption2() { ApplyUpgrade(currentOptions[1]); }
    public void SelectOption3() { ApplyUpgrade(currentOptions[2]); }

    // เปลี่ยนชื่อ Parameter เป็น CardVisualUI
    void UpdateCardVisuals(CardVisualUI cardVisual, UpgradeType type)
    {
        if (cardVisual == null) return;
        Sprite s = null;
        switch (type)
        {
            case UpgradeType.Speed: s = speedSprite; break;
            case UpgradeType.Jump: s = jumpSprite; break;
            case UpgradeType.Cooldown: s = cooldownSprite; break;
            case UpgradeType.CloneRange: s = cloneRangeSprite; break;
            case UpgradeType.AttackSpeed: s = attackSpeedSprite; break;
            case UpgradeType.DoubleJump: s = doubleJumpSprite; break;
        }
        cardVisual.SetCardVisual(s);
    }

    string GetUpgradeName(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Speed: return "Increase Speed";
            case UpgradeType.Jump: return "Increase Jump";
            case UpgradeType.Cooldown: return "Reduce Shadow Cooldown";
            case UpgradeType.CloneRange: return "Increase Clone Range";
            case UpgradeType.AttackSpeed: return "Faster Exorcise";
            case UpgradeType.DoubleJump: return "Unlock Double Jump";
        }
        return "";
    }

    void ApplyUpgrade(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Speed: playerStats.runSpeed += speedUpgradeAmount; break;
            case UpgradeType.Jump: playerStats.jumpForce += jumpUpgradeAmount; break;
            case UpgradeType.Cooldown:
                playerStats.bodySwapCooldown -= cooldownReduceAmount;
                if (playerStats.bodySwapCooldown < 1f) playerStats.bodySwapCooldown = 1f;
                break;
            case UpgradeType.CloneRange: playerStats.cloneRange += cloneRangeUpgradeAmount; break;
            case UpgradeType.AttackSpeed:
                playerStats.attackChargeTime -= attackSpeedReduceAmount;
                if (playerStats.attackChargeTime < 0.5f) playerStats.attackChargeTime = 0.5f;
                break;
            case UpgradeType.DoubleJump: playerStats.canDoubleJump = true; break;
        }
        HideUpgradePanel();
        OnUpgradeComplete?.Invoke();
    }
}