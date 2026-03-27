using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject upgradePanel;

    public Button option1Button;
    public Button option2Button;
    public Button option3Button;

    public TextMeshProUGUI option1Text;
    public TextMeshProUGUI option2Text;
    public TextMeshProUGUI option3Text;

    [Header("Player Stats")]
    public PlayerStats playerStats;

    [Header("Upgrade Values")]
    public float speedUpgradeAmount = 1f;
    public float jumpUpgradeAmount = 2f;
    public float cooldownReduceAmount = 1f;
    public float cloneRangeUpgradeAmount = 1f;
    public float attackSpeedReduceAmount = 0.5f;

    public Action OnUpgradeComplete;

    enum UpgradeType
    {
        Speed,
        Jump,
        Cooldown,
        CloneRange,
        AttackSpeed,
        DoubleJump // 🔥 เพิ่ม
    }

    UpgradeType[] currentOptions = new UpgradeType[3];

    [Header("Scene Upgrades (Override Random)")]
    public bool useFixedUpgrades = false;

    [SerializeField]
    UpgradeType[] fixedUpgrades = new UpgradeType[3];

    bool isShowing = false;

    void Start()
    {
        upgradePanel.SetActive(false);
    }

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
        if (useFixedUpgrades && fixedUpgrades.Length >= 3)
        {
            for (int i = 0; i < 3; i++)
                currentOptions[i] = fixedUpgrades[i];
        }
        else
        {
            for (int i = 0; i < 3; i++)
                currentOptions[i] = (UpgradeType)UnityEngine.Random.Range(
                    0, Enum.GetValues(typeof(UpgradeType)).Length);
        }

        option1Text.text = GetUpgradeName(currentOptions[0]);
        option2Text.text = GetUpgradeName(currentOptions[1]);
        option3Text.text = GetUpgradeName(currentOptions[2]);

        option1Button.onClick.RemoveAllListeners();
        option2Button.onClick.RemoveAllListeners();
        option3Button.onClick.RemoveAllListeners();

        option1Button.onClick.AddListener(() => ApplyUpgrade(currentOptions[0]));
        option2Button.onClick.AddListener(() => ApplyUpgrade(currentOptions[1]));
        option3Button.onClick.AddListener(() => ApplyUpgrade(currentOptions[2]));
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
            case UpgradeType.DoubleJump: return "Unlock Double Jump"; // 🔥 เพิ่ม
        }
        return "";
    }

    void ApplyUpgrade(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Speed:
                playerStats.runSpeed += speedUpgradeAmount;
                break;

            case UpgradeType.Jump:
                playerStats.jumpForce += jumpUpgradeAmount;
                break;

            case UpgradeType.Cooldown:
                playerStats.bodySwapCooldown -= cooldownReduceAmount;
                if (playerStats.bodySwapCooldown < 1f)
                    playerStats.bodySwapCooldown = 1f;
                break;

            case UpgradeType.CloneRange:
                playerStats.cloneRange += cloneRangeUpgradeAmount;
                break;

            case UpgradeType.AttackSpeed:
                playerStats.attackChargeTime -= attackSpeedReduceAmount;
                if (playerStats.attackChargeTime < 0.5f)
                    playerStats.attackChargeTime = 0.5f;
                break;

            case UpgradeType.DoubleJump: // 🔥 เพิ่ม
                playerStats.canDoubleJump = true;
                break;
        }

        HideUpgradePanel();
        OnUpgradeComplete?.Invoke();
    }
}