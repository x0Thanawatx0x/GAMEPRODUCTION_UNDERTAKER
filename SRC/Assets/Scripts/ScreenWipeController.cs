using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenWipeController : MonoBehaviour
{
    public RectTransform wipeImage;
    public float wipeDuration = 0.5f;

    float screenWidth;

    void Start()
    {
        // ใช้ width ของ Canvas แทน Screen จะเป๊ะกว่า
        Canvas canvas = GetComponentInParent<Canvas>();
        screenWidth = canvas.GetComponent<RectTransform>().rect.width;

        HideInstant(); // ซ่อนทันทีตอนเริ่มเกม
    }

    public void WipeToBlack(System.Action onComplete = null)
    {
        StopAllCoroutines();
        StartCoroutine(WipeRoutine(0, screenWidth, onComplete));
    }

    public void WipeFromBlack(System.Action onComplete = null)
    {
        StopAllCoroutines();
        StartCoroutine(WipeRoutine(screenWidth, 0, onComplete));
    }

    IEnumerator WipeRoutine(float from, float to, System.Action onComplete)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / wipeDuration;
            float x = Mathf.Lerp(from, to, t);
            wipeImage.sizeDelta = new Vector2(x, wipeImage.sizeDelta.y);
            yield return null;
        }

        wipeImage.sizeDelta = new Vector2(to, wipeImage.sizeDelta.y);
        onComplete?.Invoke();
    }

    void HideInstant()
    {
        // ซ่อนจริงๆ ไม่เหลือพื้นที่
        wipeImage.sizeDelta = new Vector2(0, wipeImage.sizeDelta.y);
    }
}
