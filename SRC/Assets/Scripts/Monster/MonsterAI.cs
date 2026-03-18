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

    private float timer;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
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

    void SetState(State newState)
    {
        currentState = newState;

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

    // ⭐ Debug Detect Range
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