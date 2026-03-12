using System.Collections;
using UnityEngine;

public class GhostOrbManager : MonoBehaviour
{
    [Header("=== Pickup Settings ===")]
    [SerializeField] float pickupRadius = 0.8f;
    [SerializeField] KeyCode pickupKey = KeyCode.E;
    [SerializeField] float pickupCooldown = 0.2f;

    [Header("=== Fade Settings ===")]
    [SerializeField] float fadeDuration = 0.5f;

    [Header("=== UI ===")]
    public GameObject pickupUI;

    bool isPicking;
    PlayerLifeManager lifeManager;

    void Start()
    {
        lifeManager = FindObjectOfType<PlayerLifeManager>();

        if (pickupUI != null)
            pickupUI.SetActive(false);
    }

    void Update()
    {
        CheckNearbyOrb();

        if (Input.GetKeyDown(pickupKey) && !isPicking)
        {
            TryPickupOrb();
        }

        // 🔒 ล็อก UI ไม่ให้ Flip ตาม Player
        if (pickupUI != null)
        {
            float direction = Mathf.Sign(transform.localScale.x);

            pickupUI.transform.localScale = new Vector3(
                   6f * direction,
    6f,
    1f
            );
        }
    
    }

    void CheckNearbyOrb()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRadius);

        bool foundOrb = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("GhostOrb"))
            {
                foundOrb = true;
                break;
            }
        }

        if (pickupUI != null)
            pickupUI.SetActive(foundOrb);
    }

    void TryPickupOrb()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRadius);

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("GhostOrb")) continue;

            StartCoroutine(FadeAndDestroy(hit.gameObject));

            if (lifeManager != null)
                lifeManager.AddGhost(1);

            StartCoroutine(PickupCooldown());
            break;
        }
    }

    IEnumerator PickupCooldown()
    {
        isPicking = true;
        yield return new WaitForSeconds(pickupCooldown);
        isPicking = false;
    }

    IEnumerator FadeAndDestroy(GameObject orb)
    {
        SpriteRenderer sr = orb.GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            Destroy(orb);
            yield break;
        }

        float time = 0f;
        Color startColor = sr.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);

            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        Destroy(orb);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}