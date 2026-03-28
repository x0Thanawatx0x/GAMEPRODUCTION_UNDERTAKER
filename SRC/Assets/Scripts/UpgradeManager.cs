using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class UpgradeManager : MonoBehaviour
{
    [Header("UI Text Elements")]
    public GameObject upgradePanel;
    public Button option1Button, option2Button, option3Button;
    public TextMeshProUGUI option1Text, option2Text, option3Text;
    public TextMeshProUGUI option1CostText, option2CostText, option3CostText;

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

    [Header("References")]
    public PlayerStats playerStats;
    public PlayerLifeManager playerLife;

    [Header("Upgrade Values Settings")]
    public float speedUpgradeAmount = 1f;
    public float jumpUpgradeAmount = 2f;
    public float cooldownReduceAmount = 1f;
    public float cloneRangeUpgradeAmount = 1f;
    public float attackSpeedReduceAmount = 0.5f;

    [Header("Upgrade Cost Settings")]
    public int speedCost = 100;
    public int jumpCost = 100;
    public int cooldownCost = 150;
    public int cloneRangeCost = 120;
    public int attackSpeedCost = 200;
    public int doubleJumpCost = 500;

    public Action OnUpgradeComplete;

    enum UpgradeType { Speed, Jump, Cooldown, CloneRange, AttackSpeed, DoubleJump }

    [Header("Fixed Upgrade Settings")]
    public bool useFixedUpgrades = true; // 🔹 ติ๊กถูกถ้าไม่ต้องการสุ่ม
    [SerializeField] UpgradeType option1Type = UpgradeType.Speed; // 🔹 กำหนดใบที่ 1
    [SerializeField] UpgradeType option2Type = UpgradeType.Jump;  // 🔹 กำหนดใบที่ 2
    [SerializeField] UpgradeType option3Type = UpgradeType.Cooldown; // 🔹 กำหนดใบที่ 3

    UpgradeType[] currentOptions = new UpgradeType[3];
    int[] currentCosts = new int[3];
    bool isShowing = false;

    void Start() { upgradePanel.SetActive(false); }

    public void ShowUpgradePanel()
    {
        if (isShowing) return;
        isShowing = true;
        upgradePanel.SetActive(true);
        Time.timeScale = 0f;
        GenerateUpgrades();

        StopAllCoroutines();
        StartCoroutine(AutoRefreshRoutine());
    }

    public void HideUpgradePanel()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
        isShowing = false;
        StopAllCoroutines();
    }

    public void CloseAndProceed()
    {
        if (playerStats != null) playerStats.Save();
        HideUpgradePanel();
        OnUpgradeComplete?.Invoke();
    }

    IEnumerator AutoRefreshRoutine()
    {
        while (isShowing)
        {
            RefreshButtonInteractable();
            yield return new WaitForSecondsRealtime(2f);
        }
    }

    public void RefreshButtonInteractable()
    {
        if (playerLife == null || !upgradePanel.activeSelf) return;
        option1Button.interactable = (playerLife.money >= currentCosts[0]);
        option2Button.interactable = (playerLife.money >= currentCosts[1]);
        option3Button.interactable = (playerLife.money >= currentCosts[2]);
    }

    void GenerateUpgrades()
    {
        if (useFixedUpgrades)
        {
            // 🔹 ดึงค่าจากที่เรากำหนดไว้ใน Inspector
            currentOptions[0] = option1Type;
            currentOptions[1] = option2Type;
            currentOptions[2] = option3Type;
        }
        else
        {
            // 🎲 ถ้าไม่ติ๊ก Fixed ก็จะสุ่มเหมือนเดิม
            for (int i = 0; i < 3; i++)
            {
                currentOptions[i] = (UpgradeType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(UpgradeType)).Length);
            }
        }

        // คำนวณราคาและอัปเดต UI เหมือนเดิม
        for (int i = 0; i < 3; i++)
        {
            currentCosts[i] = GetCost(currentOptions[i]);
        }

        option1Text.text = GetUpgradeName(currentOptions[0]);
        option2Text.text = GetUpgradeName(currentOptions[1]);
        option3Text.text = GetUpgradeName(currentOptions[2]);

        option1CostText.text = "Cost: " + currentCosts[0];
        option2CostText.text = "Cost: " + currentCosts[1];
        option3CostText.text = "Cost: " + currentCosts[2];

        UpdateCardVisuals(card1Visual, currentOptions[0]);
        UpdateCardVisuals(card2Visual, currentOptions[1]);
        UpdateCardVisuals(card3Visual, currentOptions[2]);

        RefreshButtonInteractable();
    }

    public void SelectOption1() { ApplyUpgrade(0); }
    public void SelectOption2() { ApplyUpgrade(1); }
    public void SelectOption3() { ApplyUpgrade(2); }

    void ApplyUpgrade(int index)
    {
        UpgradeType type = currentOptions[index];
        int cost = currentCosts[index];

        if (playerLife != null && playerLife.money >= cost)
        {
            playerLife.money -= cost;
            playerLife.UpdateMoneyUI_Public();
        }
        else return;

        switch (type)
        {
            case UpgradeType.Speed: playerStats.speedLevel++; playerStats.runSpeed += speedUpgradeAmount; break;
            case UpgradeType.Jump: playerStats.jumpLevel++; playerStats.jumpForce += jumpUpgradeAmount; break;
            case UpgradeType.Cooldown: playerStats.cooldownLevel++; playerStats.bodySwapCooldown -= cooldownReduceAmount; break;
            case UpgradeType.CloneRange: playerStats.rangeLevel++; playerStats.cloneRange += cloneRangeUpgradeAmount; break;
            case UpgradeType.AttackSpeed: playerStats.attackChargeLevel++; playerStats.attackChargeTime -= attackSpeedReduceAmount; break;
            case UpgradeType.DoubleJump: playerStats.canDoubleJump = true; break;
        }

        playerStats.Save();
        CloseAndProceed();
    }

    int GetCost(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Speed: return speedCost;
            case UpgradeType.Jump: return jumpCost;
            case UpgradeType.Cooldown: return cooldownCost;
            case UpgradeType.CloneRange: return cloneRangeCost;
            case UpgradeType.AttackSpeed: return attackSpeedCost;
            case UpgradeType.DoubleJump: return doubleJumpCost;
            default: return 0;
        }
    }

    string GetUpgradeName(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Speed: return "Speed UP";
            case UpgradeType.Jump: return "Jump UP";
            case UpgradeType.Cooldown: return "Skill CD Down";
            case UpgradeType.CloneRange: return "Range UP";
            case UpgradeType.AttackSpeed: return "Attack Speed UP";
            case UpgradeType.DoubleJump: return "Double Jump";
            default: return "";
        }
    }

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
}