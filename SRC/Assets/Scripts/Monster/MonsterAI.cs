using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MonsterAI : MonoBehaviour
{
    public enum State { Idle, Charge, Attack, Die }
    [SerializeField] private State currentState = State.Idle;

    [Header("Target")]
    public Transform player;

    [Header("Attack")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    [Header("Detection")]
    public float detectRange = 6f;
    public float stopAttackRange = 2f;

    [Header("Timing")]
    public float chargeTime = 1f;
    [SerializeField] PlayerStats playerStats;

    [Header("Ghost Orb Spawn")]
    public GameObject ghostOrbPrefab;
    public Vector2 orbSpawnOffset = new Vector2(0f, 1f);

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip idleSound;
    public AudioClip chargeSound;
    public AudioClip attackSound;
    public AudioClip dieSound;
    [Range(0, 1)] public float maxVolume = 1f;

    [Header("E Prompt")]
    public GameObject ePrompt;

    [Header("UI")]
    public Slider chargeSlider;

    private float timer;
    private Animator anim;
    private bool playerInRange = false;
    private float holdETimer = 0f;
    private SpriteRenderer spriteRenderer;
    public float fadeSpeed = 1f;
    private bool playerAttackTriggered = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetState(State.Idle);

        if (chargeSlider != null)
        {
            chargeSlider.maxValue = playerStats.attackChargeTime;
            chargeSlider.value = 0f;
            chargeSlider.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        UpdateSoundVolume();

        if (ePrompt != null)
            ePrompt.SetActive(playerInRange);

        if (playerInRange && currentState != State.Die)
        {
            if (Input.GetKey(KeyCode.E))
            {
                holdETimer += Time.deltaTime;

                if (chargeSlider != null)
                {
                    chargeSlider.gameObject.SetActive(true);
                    chargeSlider.value = holdETimer;
                }

                if (!playerAttackTriggered && player != null)
                {
                    PlayerControllerMain playerController = player.GetComponent<PlayerControllerMain>();
                    if (playerController != null)
                    {
                        playerController.PlayAttackAnimation();
                        playerAttackTriggered = true;
                    }
                }

                if (holdETimer >= playerStats.attackChargeTime)
                {
                    StartCoroutine(PlayDieAnimationThenFreeze()); // 🔥 เปลี่ยนตรงนี้
                    holdETimer = 0f;

                    if (chargeSlider != null)
                        chargeSlider.gameObject.SetActive(false);

                    playerAttackTriggered = false;
                }
            }
            else
            {
                holdETimer = 0f;
                playerAttackTriggered = false;

                if (chargeSlider != null)
                {
                    chargeSlider.value = 0f;
                    chargeSlider.gameObject.SetActive(false);
                }
            }
        }

        if (player == null || currentState == State.Die) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist < stopAttackRange && currentState != State.Die)
        {
            if (currentState != State.Idle) SetState(State.Idle);
            return;
        }

        switch (currentState)
        {
            case State.Idle: CheckPlayer(dist); break;
            case State.Charge: Charge(); break;
        }
    }

    void UpdateSoundVolume()
    {
        if (player == null || audioSource == null || currentState == State.Die) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= detectRange)
        {
            float volumeLevel = 1f - (dist / detectRange);
            audioSource.volume = Mathf.Clamp01(volumeLevel * maxVolume);
        }
        else
        {
            audioSource.volume = 0f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    void CheckPlayer(float dist)
    {
        if (dist < detectRange)
        {
            timer = chargeTime;
            SetState(State.Charge);
        }
    }

    void Charge()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SetState(State.Attack);
            Shoot();
        }
    }

    void Shoot()
    {
        if (player == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Vector2 dir = (player.position - firePoint.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = dir * bulletSpeed;

        Invoke(nameof(BackToIdle), 0.5f);
    }

    void BackToIdle()
    {
        if (currentState != State.Die)
            SetState(State.Idle);
    }

    void SetState(State newState)
    {
        currentState = newState;
        PlayStateSound(newState);

        if (anim == null) return;

        switch (newState)
        {
            case State.Idle: anim.Play("MIdle"); break;
            case State.Charge: anim.Play("MCharge"); break;
            case State.Attack: anim.Play("MAttack"); break;
            case State.Die: anim.Play("DieM", 0, 0f); break; // 🔥 บังคับเริ่มต้นที่เฟรมแรก
        }
    }

    void PlayStateSound(State state)
    {
        if (audioSource == null) return;

        AudioClip clipToPlay = null;

        switch (state)
        {
            case State.Idle: clipToPlay = idleSound; break;
            case State.Charge: clipToPlay = chargeSound; break;
            case State.Attack: clipToPlay = attackSound; break;
            case State.Die: clipToPlay = dieSound; break;
        }

        if (clipToPlay != null)
        {
            if (audioSource.clip == clipToPlay && audioSource.isPlaying) return;

            audioSource.clip = clipToPlay;
            audioSource.loop = (state == State.Idle);
            audioSource.Play();
        }
    }

    // 🔥 ตัวใหม่: เล่นแล้ว "ค้างเฟรมสุดท้าย"
    IEnumerator PlayDieAnimationThenFreeze()
    {
        SetState(State.Die);

        if (player != null)
        {
            PlayerControllerMain playerController = player.GetComponent<PlayerControllerMain>();
            if (playerController != null)
            {
                playerController.PlayFinishAttackAnimation();
            }
        }

        // 🔥 รอให้ Animator เข้า state ก่อน (สำคัญ)
        yield return null;

        // 🔥 ดึงเวลาจริงของ animation
        float animLength = anim.GetCurrentAnimatorStateInfo(0).length;

        // 🔥 รอให้ animation เล่นจนจบ
        yield return new WaitForSeconds(animLength);

        // 🔥 เริ่มจางหาย
        float alpha = 1f;

        while (alpha > 0f)
        {
            alpha -= fadeSpeed * Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        // 🔥 spawn ของ
        SpawnGhostOrbs();

        // 🔥 ลบทิ้ง
        Destroy(gameObject);
    }
    void SpawnGhostOrbs()
    {
        if (ghostOrbPrefab == null) return;

        if (Random.value < 0.5f)
        {
            Vector2 offset = new Vector2(
                Random.Range(-orbSpawnOffset.x, orbSpawnOffset.x),
                Random.Range(0, orbSpawnOffset.y)
            );

            Instantiate(ghostOrbPrefab, (Vector2)transform.position + offset, Quaternion.identity);
        }
    }
}