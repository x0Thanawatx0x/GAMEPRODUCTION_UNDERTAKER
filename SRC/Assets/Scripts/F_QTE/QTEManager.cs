using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QTEManager : MonoBehaviour
{
    public static QTEManager Instance;

    [Header("=== UI ===")]
    public GameObject qtePanel;
    public Slider timeSlider;
    public List<TextMeshProUGUI> keyTexts; // ต้องมี 4 ช่อง

    [Header("=== Settings ===")]
    public float qteTime = 3f;
    public int keyCount = 4;

    KeyCode[] possibleKeys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };

    List<KeyCode> currentKeys = new List<KeyCode>();
    bool[] pressed;               // ⭐ สำคัญ: แยกกดทีละช่อง
    float timer;
    bool isActive;

    MonsterQTE currentMonster;

    void Awake()
    {
        Instance = this;
        qtePanel.SetActive(false);
    }

    void Update()
    {
        if (!isActive) return;

        // ===== เวลา =====
        timer -= Time.deltaTime;
        timeSlider.value = timer;

        if (timer <= 0)
        {
            FailQTE();
            return;
        }

        // ===== ตรวจการกดปุ่ม =====
        for (int i = 0; i < currentKeys.Count; i++)
        {
            if (pressed[i]) continue;

            if (Input.GetKeyDown(currentKeys[i]))
            {
                pressed[i] = true;
                keyTexts[i].color = Color.green;
                break; // ❗ สำคัญ: กดได้ทีละช่อง
            }
        }

        // ===== เช็กว่าครบหรือยัง =====
        bool allPressed = true;
        for (int i = 0; i < pressed.Length; i++)
        {
            if (!pressed[i])
            {
                allPressed = false;
                break;
            }
        }

        if (allPressed)
        {
            SuccessQTE();
        }
    }

    // ================= START QTE =================
    public void StartQTE(MonsterQTE monster)
    {
        if (isActive) return;

        isActive = true;
        currentMonster = monster;

        // 🔒 ล็อกการเดิน
        PlayerController2D.canMove = false;

        qtePanel.SetActive(true);

        currentKeys.Clear();
        pressed = new bool[keyCount];

        // 🎲 สุ่มปุ่ม (ซ้ำได้)
        for (int i = 0; i < keyCount; i++)
        {
            currentKeys.Add(possibleKeys[Random.Range(0, possibleKeys.Length)]);
        }

        // 🖥 แสดง UI
        for (int i = 0; i < keyTexts.Count; i++)
        {
            keyTexts[i].text = currentKeys[i].ToString();
            keyTexts[i].color = Color.white;
        }

        timer = qteTime;
        timeSlider.maxValue = qteTime;
        timeSlider.value = qteTime;
    }

    // ================= END STATES =================
    void SuccessQTE()
    {
        isActive = false;
        qtePanel.SetActive(false);
        PlayerController2D.canMove = true;

        currentMonster?.OnQTESuccess();
    }

    void FailQTE()
    {
        isActive = false;
        qtePanel.SetActive(false);
        PlayerController2D.canMove = true;

        currentMonster?.OnQTEFail();
    }
}
