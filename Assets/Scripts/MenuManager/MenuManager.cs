using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject optionPanel;

    [Header("Buttons")]
    public Button continueButton;

    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider volumeSlider;

    [Header("Display")]
    public Toggle fullscreenToggle;

    private const string VOLUME_KEY = "MasterVolume";
    private const string SCREEN_MODE_KEY = "ScreenMode"; // 1 = Full, 0 = Window

    void Start()
    {
        if (optionPanel != null)
            optionPanel.SetActive(false);

        // 🔊 Load Volume
        float volume = PlayerPrefs.GetFloat(VOLUME_KEY, 0.75f);
        if (volumeSlider != null)
        {
            volumeSlider.value = volume;
            SetVolume(volume);
        }

        // 🖥️ Load Screen Mode
        LoadScreenMode();

        CheckContinueButton();
    }

    // ================= MENU =================

    public void NewGame()
    {
        ClearSave();
        SceneManager.LoadScene("SampleScene");
    }
    public void BackToMenu()
    {
        
        SceneManager.LoadScene("S_menu");
    }
    public void ContinueGame()
    {
        if (!PlayerPrefs.HasKey("SavedScene")) return;

        SceneManager.LoadScene(PlayerPrefs.GetString("SavedScene"));
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // ================= OPTION =================

    public void OpenOption()
    {
        optionPanel.SetActive(true);
    }

    public void CloseOption()
    {
        optionPanel.SetActive(false);
    }

    // ================= AUDIO =================

    public void SetVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20f);
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        PlayerPrefs.Save();
    }

    // ================= DISPLAY =================

    // 🔥 เรียกจาก Toggle เท่านั้น
    public void OnFullscreenToggle(bool isOn)
    {
        if (isOn)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.fullScreen = true;
            PlayerPrefs.SetInt(SCREEN_MODE_KEY, 1);
        }
        else
        {
            Screen.fullScreen = false;
            PlayerPrefs.SetInt(SCREEN_MODE_KEY, 0);
        }

        PlayerPrefs.Save();
    }

    void LoadScreenMode()
    {
        bool isFullscreen = PlayerPrefs.GetInt(SCREEN_MODE_KEY, 1) == 1;

        // ⭐ จุดสำคัญ แก้บั๊ก Toggle ค้าง
        fullscreenToggle.SetIsOnWithoutNotify(isFullscreen);

        // บังคับใช้โหมด
        if (isFullscreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
        }
    }

    // ================= SAVE =================

    void CheckContinueButton()
    {
        if (continueButton != null)
            continueButton.interactable = PlayerPrefs.HasKey("SavedScene");
    }

    void ClearSave()
    {
        PlayerPrefs.DeleteKey("SavedScene");
        PlayerPrefs.DeleteKey("PlayerX");
        PlayerPrefs.DeleteKey("PlayerY");
        PlayerPrefs.DeleteKey("PlayerZ");
        PlayerPrefs.Save();
    }
}
