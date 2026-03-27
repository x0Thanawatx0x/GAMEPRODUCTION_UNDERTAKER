using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class AutoSaveTrigger : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text saveText;

    [Header("Setting")]
    public float finishDelay = 1.5f;

    [Header("Player Stats")]
    public PlayerStats playerStats; // 🔹 ต้องลาก Inspector

    private bool hasSaved = false;

    void Start()
    {
        if (saveText != null)
            saveText.gameObject.SetActive(false);
    }

    public IEnumerator SaveProcess(Transform player)
    {
        if (saveText != null)
        {
            saveText.gameObject.SetActive(true);
            saveText.text = "Saving...";
        }

        yield return new WaitForSeconds(0.5f);

        // ✅ Save Scene
        PlayerPrefs.SetString("SavedScene", SceneManager.GetActiveScene().name);

        // ✅ Save Position
        PlayerPrefs.SetFloat("PlayerX", player.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.position.y);
        PlayerPrefs.SetFloat("PlayerZ", player.position.z);

        // ✅ Save PlayerStats
        if (playerStats != null)
            playerStats.Save();

        PlayerPrefs.Save();

        if (saveText != null)
            saveText.text = "Finish";

        yield return new WaitForSeconds(finishDelay);

        if (saveText != null)
            saveText.gameObject.SetActive(false);
    }

    // 🔹 ฟังก์ชัน Load ใช้ตอนเริ่ม Scene
    public void LoadPlayer(Transform player)
    {
        if (PlayerPrefs.HasKey("PlayerX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerX");
            float y = PlayerPrefs.GetFloat("PlayerY");
            float z = PlayerPrefs.GetFloat("PlayerZ");
            player.position = new Vector3(x, y, z);
        }

        if (playerStats != null)
            playerStats.Load();
    }
}