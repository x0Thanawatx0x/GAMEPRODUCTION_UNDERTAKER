using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    public Image cutsceneImage;
    public Sprite[] cutsceneSprites;
    

    private int currentIndex = 0;

    void Start()
    {
        cutsceneImage.sprite = cutsceneSprites[currentIndex];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            NextScene();
        }
    }

    void NextScene()
    {
        currentIndex++;

        if (currentIndex < cutsceneSprites.Length)
        {
            cutsceneImage.sprite = cutsceneSprites[currentIndex];
        }
        else
        {
            // ไป Scene ใหม่
            SceneManager.LoadScene("M1"); // เปลี่ยนชื่อ Scene ตามที่ใช้
        }
    }
}