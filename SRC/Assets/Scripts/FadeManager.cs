using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    public Image fadeImage;
    public float fadeDuration = 0.5f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public IEnumerator FadeOut()
    {
        if (fadeImage == null)
        {
            Debug.LogError("❌ fadeImage is NULL!");
            yield break;
        }

        float t = 0;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime; // 🔥 แก้ค้างตรงนี้
            float a = Mathf.Lerp(0, 1, t / fadeDuration);
            fadeImage.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }

        fadeImage.color = new Color(c.r, c.g, c.b, 1);
    }

    public IEnumerator FadeIn()
    {
        if (fadeImage == null)
        {
            Debug.LogError("❌ fadeImage is NULL!");
            yield break;
        }

        float t = 0;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime; // 🔥 แก้ค้างตรงนี้
            float a = Mathf.Lerp(1, 0, t / fadeDuration);
            fadeImage.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }

        fadeImage.color = new Color(c.r, c.g, c.b, 0);
    }
}