using UnityEngine;
using System.Collections;

public class CameraTransitionTrigger : MonoBehaviour
{
    public ScreenWipeController screenWipe;
    public Transform cameraTargetPos;   // จุดใหม่ของกล้อง
    public float cameraMoveDuration = 0.5f;

    bool isUsed = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isUsed) return;
        if (!other.CompareTag("Player")) return;

        isUsed = true;

        screenWipe.WipeToBlack(() =>
        {
            StartCoroutine(MoveCamera());
        });
    }

    IEnumerator MoveCamera()
    {
        Camera cam = Camera.main;
        Vector3 startPos = cam.transform.position;
        Vector3 targetPos = new Vector3(
            cameraTargetPos.position.x,
            cameraTargetPos.position.y,
            startPos.z
        );

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / cameraMoveDuration;
            cam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        screenWipe.WipeFromBlack();
    }
}
