using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerLifeManager : MonoBehaviour
{
    [Header("Time")]
    public float playTime;

    [Header("Trap Count")]
    public int trapCount = 0;
    private bool canCountTrap = true;

    [Header("Ghost Count")]
    public int ghostCount = 0;

    [Header("Money")]
    public int money = 0;
    public int moneyPerGhost = 10;

    [Header("Animation")]
    public Animator playerAnimator;

    [Header("UI")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI trapCountText;
    public TextMeshProUGUI ghostCountText;
    public TextMeshProUGUI moneyText;

    [Header("Ghost UI Images")]
    public Image[] ghostUIImages;
    public Sprite ghostEmptySprite;
    public Sprite ghostFullSprite;

    [Header("Upgrade Reference")]
    public UpgradeManager upgradeManager; // 🔥 ลาก Object UpgradeManager มาใส่ในช่องนี้

    void Start()
    {
        playTime = 0f;
        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>();

        UpdateTrapUI();
        UpdateGhostUI();
        UpdateMoneyUI();
    }

    void Update()
    {
        playTime += Time.deltaTime;
        UpdateTimeUI();
    }

    void UpdateTimeUI()
    {
        int minutes = Mathf.FloorToInt(playTime / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);
        timeText.text = $"Time : {minutes:00}:{seconds:00}";
    }

    void UpdateTrapUI()
    {
        trapCountText.text = $"DEAD: {trapCount}";
    }

    public void CountTrap()
    {
        if (!canCountTrap) return;
        trapCount++;
        UpdateTrapUI();
        canCountTrap = false;
    }

    public void ResetTrapCountLock()
    {
        canCountTrap = true;
    }

    void UpdateGhostUI()
    {
        ghostCountText.text = $"Ghost : {ghostCount}";
        if (ghostUIImages != null && ghostUIImages.Length > 0)
        {
            for (int i = 0; i < ghostUIImages.Length; i++)
            {
                if (i < ghostCount)
                    ghostUIImages[i].sprite = ghostFullSprite;
                else
                    ghostUIImages[i].sprite = ghostEmptySprite;
            }
        }
    }

    public void UpdateMoneyUI_Public()
    {
        UpdateMoneyUI();
    }

    public void AddGhost(int amount = 1)
    {
        ghostCount += amount;
        UpdateGhostUI();
    }

    public void ResetGhost()
    {
        ghostCount = 0;
        UpdateGhostUI();
    }

    public int GetGhost()
    {
        return ghostCount;
    }

    void UpdateMoneyUI()
    {
        moneyText.text = $"Money : {money}";
    }

    public void ConvertGhostToMoney()
    {
        int earned = ghostCount * moneyPerGhost;
        money += earned;
        ghostCount = 0;

        UpdateMoneyUI();
        UpdateGhostUI();

        // 🔥 สั่งให้ UpgradeManager เช็คเงินใหม่ทันทีที่เงินเข้ากระเป๋า
        if (upgradeManager != null)
        {
            upgradeManager.RefreshButtonInteractable();
        }
    }

    public void PlayPrayAnimation(string animationName)
    {
        if (playerAnimator != null)
            playerAnimator.Play(animationName, 0, 0f);
    }
}