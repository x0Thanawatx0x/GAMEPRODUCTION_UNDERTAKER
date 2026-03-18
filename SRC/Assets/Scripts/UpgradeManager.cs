using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    enum UpgradeType
    {
        Speed,
        Jump,
        Cooldown
    }

    UpgradeType[] currentOptions = new UpgradeType[3];

    void Start()
    {
        upgradePanel.SetActive(false);
    }

    public void ShowUpgradePanel()
    {
        upgradePanel.SetActive(true);
        Time.timeScale = 0f;

        GenerateUpgrades();
    }

    public void HideUpgradePanel()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void GenerateUpgrades()
    {
        for (int i = 0; i < 3; i++)
        {
            currentOptions[i] = (UpgradeType)Random.Range(0, System.Enum.GetValues(typeof(UpgradeType)).Length);
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
            case UpgradeType.Speed:
                return "Increase Speed";
            case UpgradeType.Jump:
                return "Increase Jump";
            case UpgradeType.Cooldown:
                return "Reduce Shadow Cooldown";
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
        }

        HideUpgradePanel();
    }
}