using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject levelPanel;

    private bool isPanelOpen = false;

    void Start()
    {
        // เริ่มเกมมาให้ปิด Panel ไว้ก่อน
        if (levelPanel != null) levelPanel.SetActive(false);
    }

    void Update()
    {
        // 🔹 ระบบกด F1 เพื่อเปิด/ปิด (กดจากที่ไหนก็ได้ในฉาก)
        if (Input.GetKeyDown(KeyCode.F1))
        {
            TogglePanel();
        }

        // 🔹 ถ้าหน้าต่างเปิดอยู่ แล้วกด Esc ให้ปิดหน้าต่างได้ด้วย
        if (isPanelOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }
    }

    // ฟังก์ชันสลับสถานะ เปิด/ปิด
    public void TogglePanel()
    {
        if (!isPanelOpen) OpenPanel();
        else ClosePanel();
    }

    public void OpenPanel()
    {
        isPanelOpen = true;
        if (levelPanel != null) levelPanel.SetActive(true);

        // 🧊 หยุดเวลาเกมเพื่อให้เลือกด่านได้โดยไม่โดนทำร้าย
        Time.timeScale = 0f;

        // 🖱️ แสดงเมาส์เพื่อให้กดปุ่มได้
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ClosePanel()
    {
        isPanelOpen = false;
        if (levelPanel != null) levelPanel.SetActive(false);

        // ⏳ เดินเวลาเกมต่อตามปกติ
        Time.timeScale = 1f;

        // 🖱️ ซ่อนเมาส์ (ถ้าเกมคุณเป็นแนวที่ต้องซ่อนเมาส์ตอนเล่น ให้เอาคอมเมนต์ออก)
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
    }

    // 🔥 ฟังก์ชันสำหรับปุ่มย้ายฉาก 6 ปุ่ม (ลากใส่ On Click ใน Inspector)
    public void LoadLevel(string sceneName)
    {
        // คืนค่าเวลาก่อนย้ายฉากเสมอ เพื่อไม่ให้ฉากถัดไปค้าง (TimeScale 0)
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}