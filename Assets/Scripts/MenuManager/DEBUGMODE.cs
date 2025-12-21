using UnityEngine;

public class DEBUGMODE : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;

    [Header("Setting")]
    public bool pauseGame = true; // true = หยุดเวลาเกม

    private bool isOpen = false;

    // ลำดับปุ่มที่จะตรวจสอบ (W W S S A D A D)
    private KeyCode[] comboSequence = new KeyCode[]
    {
        KeyCode.W, KeyCode.W, KeyCode.S, KeyCode.S,
        KeyCode.A, KeyCode.D, KeyCode.A, KeyCode.D,
        
    };

    private int comboIndex = 0; // เก็บตำแหน่งปัจจุบันในลำดับ

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        CheckComboInput();
    }

    void CheckComboInput()
    {
        if (comboIndex >= comboSequence.Length) return;

        if (Input.GetKeyDown(comboSequence[comboIndex]))
        {
            comboIndex++;

            // ครบลำดับ → toggle pause
            if (comboIndex == comboSequence.Length)
            {
                TogglePause();
                comboIndex = 0; // รีเซ็ตลำดับ
            }
        }
        else if (Input.anyKeyDown)
        {
            // ถ้ากดผิดลำดับ → รีเซ็ต
            if (Input.GetKeyDown(comboSequence[comboIndex]) == false)
                comboIndex = 0;
        }
    }

    public void TogglePause()
    {
        isOpen = !isOpen;

        if (pausePanel != null)
            pausePanel.SetActive(isOpen);

        if (pauseGame)
        {
            Time.timeScale = isOpen ? 0f : 1f;
        }
    }

    // ใช้กับปุ่ม Resume
    public void Resume()
    {
        isOpen = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);
        Time.timeScale = 1f;
        comboIndex = 0; // รีเซ็ตลำดับด้วย
    }
}
