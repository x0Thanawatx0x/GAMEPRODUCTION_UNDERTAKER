using System.Collections;
using UnityEngine;

public class GhostOrbManager : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] float fadeDuration = 0.5f;

    PlayerLifeManager lifeManager;

    void Start()
    {
        lifeManager = FindObjectOfType<PlayerLifeManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจสอบ parent ใด ๆ ว่าเป็น GhostOrb
        GhostOrbRespawn orbRespawn = other.GetComponentInParent<GhostOrbRespawn>();
        if (orbRespawn == null) return; // ไม่ใช่ orb

        // เพิ่ม Ghost แค่ครั้งเดียว
        if (lifeManager != null)
            lifeManager.AddGhost(1);

        StartCoroutine(FadeAndCollect(orbRespawn.gameObject));
        Debug.Log("Orb collected: " + orbRespawn.gameObject.name);
    }

    IEnumerator FadeAndCollect(GameObject orb)
    {
        SpriteRenderer sr = orb.GetComponent<SpriteRenderer>();
        GhostOrbRespawn respawn = orb.GetComponent<GhostOrbRespawn>();

        if (sr == null)
            yield break;

        float time = 0f;
        Color startColor = sr.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        if (respawn != null)
            respawn.Collect();
        else
            orb.SetActive(false);
    }
}