using UnityEngine;
using TMPro;

public class PlayerLifeManager : MonoBehaviour
{
    [Header("Time")]
    public float playTime;

    [Header("Trap Count")]
    public int trapCount = 0;
    private bool canCountTrap = true;   // 🔒 ตัวกันนับรัว

    [Header("UI")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI trapCountText;

    void Start()
    {
        playTime = 0f;
        UpdateTrapUI();
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
        trapCountText.text = $"Trap Hit : {trapCount}";
    }

    // เรียกจาก Trap
    public void CountTrap()
    {
        if (!canCountTrap) return;

        trapCount++;
        UpdateTrapUI();

        canCountTrap = false; // 🔒 ล็อกไม่ให้นับซ้ำ
    }

    // เรียกหลังจาก respawn เสร็จ
    public void ResetTrapCountLock()
    {
        canCountTrap = true;
    }
}
