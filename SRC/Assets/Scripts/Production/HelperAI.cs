using UnityEngine;
using TMPro;
using System.Collections;

public class HelperAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Follow")]
    public Vector3 offset = new Vector3(1.5f, 1f, 0f);
    public float smoothTime = 0.2f;

    [Header("Floating")]
    public float floatAmplitude = 0.3f;
    public float floatFrequency = 2f;

    [Header("Dialogue")]
    public GameObject dialogueBubble;
    public TextMeshProUGUI dialogueText;
    public float showTime = 2f;

    [Header("Danger Detection (Tag)")]
    public float detectRadius = 3f;

    [Tooltip("ใส่ Tag เช่น trap, enemy, boss")]
    public string[] dangerTags;

    [TextArea]
    public string[] dangerMessages;

    [Header("Cooldown")]
    public float talkCooldown = 3f;

    private Vector3 velocity = Vector3.zero;
    private float floatTimer = 0f;
    private float lastTalkTime = -999f;
    private Coroutine talkRoutine;

    private float currentOffsetX;

    void Update()
    {
        if (player == null) return;

        HandleFollow();
        HandleDangerCheck();
    }

    // ================= FOLLOW =================
    void HandleFollow()
    {
        float dir = Mathf.Sign(player.localScale.x);

        // อยู่ด้านหลังผู้เล่น
        float targetX = -dir * Mathf.Abs(offset.x);

        // สลับฝั่งแบบนุ่ม
        currentOffsetX = Mathf.Lerp(currentOffsetX, targetX, Time.deltaTime * 5f);

        Vector3 dynamicOffset = new Vector3(currentOffsetX, offset.y, 0f);
        Vector3 targetPos = player.position + dynamicOffset;

        // ลอยขึ้นลง
        floatTimer += Time.deltaTime;
        targetPos.y += Mathf.Sin(floatTimer * floatFrequency) * floatAmplitude;

        // เคลื่อนที่นุ่ม
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );

        // หันตามผู้เล่น
        if (dir > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    // ================= DANGER CHECK (ใช้ Tag) =================
    void HandleDangerCheck()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            player.position,
            detectRadius
        );

        bool foundDanger = false;

        foreach (var hit in hits)
        {
            foreach (var tag in dangerTags)
            {
                if (hit.CompareTag(tag))
                {
                    foundDanger = true;
                    break;
                }
            }

            if (foundDanger) break;
        }

        if (foundDanger)
        {
            if (Time.time >= lastTalkTime + talkCooldown)
            {
                SayRandom();
                lastTalkTime = Time.time;
            }
        }
    }

    void SayRandom()
    {
        if (dangerMessages == null || dangerMessages.Length == 0) return;

        int index = Random.Range(0, dangerMessages.Length);
        Say(dangerMessages[index]);
    }

    // ================= TALK =================
    public void Say(string message)
    {
        if (talkRoutine != null)
            StopCoroutine(talkRoutine);

        talkRoutine = StartCoroutine(TalkRoutine(message));
    }

    IEnumerator TalkRoutine(string message)
    {
        if (dialogueBubble == null || dialogueText == null) yield break;

        dialogueBubble.SetActive(true);
        dialogueText.text = message;

        yield return new WaitForSeconds(showTime);

        dialogueBubble.SetActive(false);
    }

    // ================= GIZMOS =================
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (player != null)
            Gizmos.DrawWireSphere(player.position, detectRadius);
    }
}