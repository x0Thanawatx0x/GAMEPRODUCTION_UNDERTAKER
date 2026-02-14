using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public GameObject upgradePanel;

    void Start()
    {
        upgradePanel.SetActive(false);
    }

    public void ShowUpgradePanel()
    {
        upgradePanel.SetActive(true);
        Time.timeScale = 0f; // หยุดเกม
    }

    public void HideUpgradePanel()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1f; // เล่นต่อ
    }

    // ===== ตัวอย่างอัปเกรด =====

    public void UpgradeSpeed()
    {
        PlayerController2D player = FindObjectOfType<PlayerController2D>();
        player.SendMessage("AddSpeed", 1f);
        HideUpgradePanel();
    }

    public void UpgradeJump()
    {
        PlayerController2D player = FindObjectOfType<PlayerController2D>();
        player.SendMessage("AddJump", 2f);
        HideUpgradePanel();
    }

    public void UpgradeCooldown()
    {
        PlayerController2D player = FindObjectOfType<PlayerController2D>();
        player.SendMessage("ReduceShadowCooldown", 1f);
        HideUpgradePanel();
    }
}
