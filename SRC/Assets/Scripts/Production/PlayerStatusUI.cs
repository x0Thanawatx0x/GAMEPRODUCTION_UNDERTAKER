using UnityEngine;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    public GameObject statusPanel;

    public PlayerStats stats;

    public TextMeshProUGUI runSpeedText;
    public TextMeshProUGUI jumpForceText;
    public TextMeshProUGUI doubleJumpText;
    public TextMeshProUGUI cooldownText;

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
            Time.timeScale = 0;
            UpdateUI();
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    void UpdateUI()
    {
        runSpeedText.text = "Run Speed : " + stats.runSpeed;
        jumpForceText.text = "Jump Force : " + stats.jumpForce;
        doubleJumpText.text = "Double Jump : " + stats.canDoubleJump;
        cooldownText.text = "Body Swap Cooldown : " + stats.bodySwapCooldown;
    }
}