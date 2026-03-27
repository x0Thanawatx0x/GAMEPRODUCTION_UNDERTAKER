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
    public Image[] ghostUIImages; // ใส่ 5 รูปใน Inspector
    public Sprite ghostEmptySprite; // รูปวิญญาณยังไม่ได้เก็บ
    public Sprite ghostFullSprite;  // รูปวิญญาณเก็บแล้ว

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

    // ===== TIME =====
    void UpdateTimeUI()
    {
        int minutes = Mathf.FloorToInt(playTime / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);
        timeText.text = $"Time : {minutes:00}:{seconds:00}";
    }

    // ===== TRAP =====
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

    // ===== GHOST =====
    void UpdateGhostUI()
    {
        // อัปเดตข้อความ Ghost Count
        ghostCountText.text = $"Ghost : {ghostCount}";

        // อัปเดตรูปวิญญาณใน UI
        if (ghostUIImages != null && ghostUIImages.Length > 0)
        {
            for (int i = 0; i < ghostUIImages.Length; i++)
            {
                if (i < ghostCount)
                    ghostUIImages[i].sprite = ghostFullSprite; // เปลี่ยนเป็นรูปเก็บแล้ว
                else
                    ghostUIImages[i].sprite = ghostEmptySprite; // ยังไม่ได้เก็บ
            }
        }
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

    // ===== MONEY =====
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
    }

    // ===== PLAY PRAY ANIMATION =====
    public void PlayPrayAnimation(string animationName)
    {
        if (playerAnimator != null)
        {
            playerAnimator.Play(animationName, 0, 0f);
        }
    }
}