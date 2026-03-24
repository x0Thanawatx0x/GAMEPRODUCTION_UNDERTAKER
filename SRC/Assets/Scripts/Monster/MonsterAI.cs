using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    public enum State
    {
        Idle,
        Charge,
        Attack,
        Die
    }

    [SerializeField] private State currentState = State.Idle;

    [Header("Target")]
    public Transform player;

    [Header("Attack")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    [Header("Detection")]
    public float detectRange = 6f;

    [Header("Timing")]
    public float chargeTime = 1f;

    [Header("Audio")] // ⭐ เพิ่มส่วนของเสียง
    public AudioSource audioSource;
    public AudioClip idleSound;
    public AudioClip chargeSound;
    public AudioClip attackSound;
    public AudioClip dieSound;

    private float timer;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        // ถ้าไม่ได้ลาก AudioSource มาใส่เอง ให้มันพยายามหาในเครื่อง
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        SetState(State.Idle);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                CheckPlayer();
                break;

            case State.Charge:
                Charge();
                break;

            case State.Attack:
                break;
        }
    }

    void CheckPlayer()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

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
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Vector2 dir = (player.position - firePoint.position).normalized;
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = dir * bulletSpeed;
        }

        Invoke(nameof(BackToIdle), 0.5f);
    }

    void BackToIdle()
    {
        SetState(State.Idle);
    }

    // ⭐ ปรับปรุงฟังก์ชัน SetState ให้เล่นเสียงตาม State
    void SetState(State newState)
    {
        currentState = newState;

        // 1. จัดการเรื่องเสียง
        PlayStateSound(newState);

        // 2. จัดการเรื่อง Animation
        if (anim == null) return;

        switch (newState)
        {
            case State.Idle:
                anim.Play("MIdle");
                break;

            case State.Charge:
                anim.Play("MCharge");
                break;

            case State.Attack:
                anim.Play("MAttack");
                break;

            case State.Die:
                anim.Play("DieM");
                break;
        }
    }

    // ⭐ ฟังก์ชันสำหรับเลือกเล่นเสียง
    void PlayStateSound(State state)
    {
        if (audioSource == null) return;

        // หยุดเสียงเดิมก่อน (ถ้าต้องการให้เสียงใหม่ขัดจังหวะเสียงเก่าได้เลย)
        audioSource.Stop();

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
            audioSource.clip = clipToPlay;
            // ถ้าเป็น Idle อาจจะให้ Loop (ติ๊กถูกใน AudioSource Component จะง่ายกว่า)
            audioSource.loop = (state == State.Idle);
            audioSource.Play();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}