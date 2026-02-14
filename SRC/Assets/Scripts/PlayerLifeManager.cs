using UnityEngine;
using TMPro;

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
    public int moneyPerGhost = 10;   // 1 วิญญาณ = กี่เงิน

    [Header("UI")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI trapCountText;
    public TextMeshProUGUI ghostCountText;
    public TextMeshProUGUI moneyText;

    void Start()
    {
        playTime = 0f;
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
        trapCountText.text = $"Trap Hit : {trapCount}";
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
        ghostCountText.text = $"Ghost : {ghostCount}";
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

    // ===== MONEY =====
    void UpdateMoneyUI()
    {
        moneyText.text = $"Money : {money}";
    }

    // 🔥 ฟังก์ชันแปลง Ghost → Money
    public void ConvertGhostToMoney()
    {
        int earned = ghostCount * moneyPerGhost;
        money += earned;
        ghostCount = 0;

        UpdateMoneyUI();
        UpdateGhostUI();
    }
}
    