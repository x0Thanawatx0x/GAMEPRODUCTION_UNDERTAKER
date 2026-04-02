using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlideTransition : MonoBehaviour
{
    public RectTransform panel;
    public float duration = 0.5f;

    Vector2 startPos;
    Vector2 endPos;

    void Awake()
    {
        float width = Screen.width;

        // ปิดจอ (ขวา → กลาง)
        startPos = new Vector2(width, 0);
        endPos = Vector2.zero;

        if (panel != null)
            panel.anchoredPosition = startPos;
    }
    void Start()
    {
        SlideIn();
    }

    // 👉 ใช้ตอน "ออกฉาก"
    public void SlideAndLoad(string sceneName)
    {
        StartCoroutine(SlideOut(sceneName));
    }

    IEnumerator SlideOut(string sceneName)
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            panel.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        panel.anchoredPosition = endPos;

        SceneManager.LoadScene(sceneName);
    }

    // 👉 ใช้ตอน "เข้าฉากใหม่"
    public void SlideIn()
    {
        StartCoroutine(SlideInRoutine());
    }

    IEnumerator SlideInRoutine()
    {
        float width = Screen.width;

        Vector2 from = Vector2.zero;          // เริ่มปิดจอ
        Vector2 to = new Vector2(-width, 0);  // เลื่อนไปซ้าย (เปิดจอ)

        panel.anchoredPosition = from;

        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            panel.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }

        panel.anchoredPosition = to;
    }
}