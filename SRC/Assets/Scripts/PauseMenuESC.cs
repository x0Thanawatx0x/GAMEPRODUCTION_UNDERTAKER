using UnityEngine;

public class PauseMenuESC : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;

    [Header("Setting")]
    public bool pauseGame = true; // true = หยุดเวลาเกม

    private bool isOpen = false;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isOpen = !isOpen;

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
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
