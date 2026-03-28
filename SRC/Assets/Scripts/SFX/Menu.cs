using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("Buttons")]
    public Button continueButton;

    void Start()
    {
        // เช็คว่ามีเซฟไหม ถ้าไม่มีปุ่ม Continue จะกดไม่ได้
        if (continueButton != null)
            continueButton.interactable = PlayerPrefs.HasKey("SavedScene");
    }

    // --- ปุ่มเริ่มเกมใหม่ ---
    public void NewGame()
    {
        // ลบค่าตำแหน่งที่เคยเซฟไว้ทั้งหมด
        PlayerPrefs.DeleteKey("SavedScene");
        PlayerPrefs.DeleteKey("PlayerX");
        PlayerPrefs.DeleteKey("PlayerY");
        PlayerPrefs.DeleteKey("PlayerZ");
        PlayerPrefs.Save();

        // โหลดฉากเริ่มเกม (เปลี่ยนชื่อ SampleScene เป็นชื่อฉากของคุณ)
        SceneManager.LoadScene("CutScene");
    }

    // --- ปุ่มเล่นต่อ ---
    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("SavedScene"))
        {
            string sceneToLoad = PlayerPrefs.GetString("SavedScene");
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    // --- ปุ่มออกจากเกม ---
    public void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }
}