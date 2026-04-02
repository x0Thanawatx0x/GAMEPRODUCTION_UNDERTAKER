using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class PlayerControllerMain : MonoBehaviour
{
    public bool canMove = true;

    [Header("=== Player Stats ===")]
    [SerializeField] PlayerStats playerStats;

    [Header("=== Ground Check ===")]
    [SerializeField] Transform groundRayOrigin;
    [SerializeField] float groundRayLength = 0.4f;
    [SerializeField] LayerMask groundLayer;

    [Header("=== Wall Check ===")]
    [SerializeField] Transform wallCheck;
    [SerializeField] float wallCheckDistance = 0.3f;
    [SerializeField] LayerMask wallLayer;

    [Header("=== Wall Movement ===")]
    [SerializeField] float wallSlideSpeed = 2f;
    [SerializeField] float wallJumpForceX = 10f; // แนะนำให้ปรับเป็น 10-12 เพื่อให้ดีดตัวพ้นระยะกำแพง
    [SerializeField] float wallJumpForceY = 12f;

    [Header("=== Wall Jump Cooldown ===")]
    [SerializeField] float wallJumpCooldown = 0.5f;
    private bool canWallJump = true;

    [Header("=== Wall Gravity ===")]
    [SerializeField] float wallSlideGravity = 0.3f;
    [SerializeField] float normalGravity = 1.5f;

    [Header("=== Animation ===")]
    [SerializeField] Animator animator;

    [Header("=== Wall Smoke ===")]
    [SerializeField] ParticleSystem wallSmoke;

    [Header("=== Audio ===")]
    [SerializeField] AudioClip footstepClip;
    [SerializeField] float footstepVolume = 0.8f;

    [Header("=== Landing Sound ===")]
    [SerializeField] AudioClip landingClip;
    [SerializeField] float landingVolume = 1f;
    [SerializeField] float minLandingVelocity = -6f;

    [Header("=== Clone Cooldown UI (Radial) ===")]
    [SerializeField] Image cloneCooldownCircle;

    [Header("=== Better Jump ===")]
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;

    private float lastCloneTime = -999f;

    Rigidbody2D rb;
    AudioSource audioSource;
    Vector3 originalScale;
    float moveInput;
    bool isGrounded;
    bool wasGrounded;
    bool isTouchingWall;
    bool isFacingRight = true;
    int wallSide;
    int jumpCount = 0;
    Vector2 groundNormal = Vector2.up;
    ParticleSystem.EmissionModule smokeEmission;
    bool lockAnimation = false;
    bool isWallJumping = false;
    float wallJumpTime = 0.2f; // ปรับเวลาล็อคการควบคุมแนวนอนเล็กน้อย
    bool isCloneMode = false;

    public bool CanUseClone() => Time.time >= lastCloneTime + playerStats.bodySwapCooldown;
    public void StartCloneCooldown() => lastCloneTime = Time.time;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        originalScale = transform.localScale;

        if (wallSmoke != null)
        {
            smokeEmission = wallSmoke.emission;
            smokeEmission.rateOverTime = 0;
        }

        if (cloneCooldownCircle != null)
            cloneCooldownCircle.fillAmount = 1f;

        rb.gravityScale = normalGravity;
    }

    void Update()
    {
        if (!canMove) return;

        moveInput = Input.GetAxisRaw("Horizontal");

        GroundCheck();
        WallCheck();
        HandleJumpInput();
        HandleFlip();
        UpdateWallSmoke();
        HandleLandingSound();
        HandleAnimation();
        UpdateCloneCooldownUI();
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        float targetSpeed = moveInput * playerStats.runSpeed;

        if (isTouchingWall && !isGrounded && moveInput == wallSide)
        {
            rb.gravityScale = wallSlideGravity;
            rb.velocity = new Vector2(0, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
            isWallJumping = false;
        }
        else if (isWallJumping)
        {
            rb.gravityScale = normalGravity;
            wallJumpTime -= Time.fixedDeltaTime;
            if (wallJumpTime <= 0)
                isWallJumping = false;
        }
        else
        {
            rb.gravityScale = normalGravity;

            if (isGrounded)
            {
                Vector2 slopeDir = new Vector2(groundNormal.y, -groundNormal.x).normalized;
                Vector2 targetVelocity = slopeDir * moveInput * playerStats.runSpeed;

                if (!Input.GetKey(KeyCode.Space))
                    rb.velocity = new Vector2(targetVelocity.x, slopeDir.y * moveInput * playerStats.runSpeed);
                else
                    rb.velocity = new Vector2(targetVelocity.x, rb.velocity.y);
            }
            else
            {
                float smooth = 5f;
                float newX = Mathf.Lerp(rb.velocity.x, targetSpeed, smooth * Time.fixedDeltaTime);
                rb.velocity = new Vector2(newX, rb.velocity.y);
            }
        }

        if (rb.velocity.y < 0)
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier * 1.5f - 1) * Time.fixedDeltaTime;
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
    }

    void GroundCheck()
    {
        float extra = 0.2f;
        RaycastHit2D hitCenter = Physics2D.Raycast(groundRayOrigin.position, Vector2.down, groundRayLength + extra, groundLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(groundRayOrigin.position + Vector3.left * 0.2f, Vector2.down, groundRayLength + extra, groundLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(groundRayOrigin.position + Vector3.right * 0.2f, Vector2.down, groundRayLength + extra, groundLayer);

        RaycastHit2D hit = hitCenter.collider ? hitCenter : (hitLeft.collider ? hitLeft : hitRight);

        if (hit.collider != null)
        {
            isGrounded = true;
            groundNormal = hit.normal;
            jumpCount = 0;
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector2.up;
        }
    }

    void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(Vector2.up * playerStats.jumpForce, ForceMode2D.Impulse);
                jumpCount = 0;
            }
            // 🔥 ปรับปรุงระบบ Wall Jump: ล็อค canWallJump ทันทีที่กด
            else if (isTouchingWall && wallSide != 0 && canWallJump)
            {
                canWallJump = false; // ล็อคการกระโดดกำแพงทันที

                float jumpDir = -wallSide;
                rb.velocity = Vector2.zero; // ล้างความเร็วเก่าเพื่อให้แรงดีดทำงานได้เต็มที่

                rb.AddForce(new Vector2(jumpDir * wallJumpForceX, wallJumpForceY), ForceMode2D.Impulse);
                isFacingRight = jumpDir > 0;

                isWallJumping = true;
                wallJumpTime = 0.25f; // ล็อค moveInput ชั่วคราวเพื่อให้ตัวละครดีดตัวพ้นระยะกำแพง

                StartCoroutine(WallJumpCooldownRoutine());
            }
            else if (playerStats.canDoubleJump && jumpCount < 1)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(Vector2.up * playerStats.jumpForce, ForceMode2D.Impulse);
                jumpCount++;
            }
        }
    }

    IEnumerator WallJumpCooldownRoutine()
    {
        // canWallJump เป็น false อยู่แล้วจากการกด Space
        yield return new WaitForSeconds(wallJumpCooldown);
        canWallJump = true; // คืนสถานะให้กระโดดกำแพงได้อีกครั้งหลังผ่านไป 0.5 วินาที
    }

    void WallCheck()
    {
        wallSide = 0;
        RaycastHit2D hitRight = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, wallLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, wallLayer);

        if (hitRight.collider != null) { isTouchingWall = true; wallSide = 1; }
        else if (hitLeft.collider != null) { isTouchingWall = true; wallSide = -1; }
        else { isTouchingWall = false; }
    }

    void HandleFlip()
    {
        if (moveInput > 0) isFacingRight = true;
        else if (moveInput < 0) isFacingRight = false;

        float dir = isFacingRight ? 1 : -1;
        transform.localScale = new Vector3(dir * Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
    }

    void HandleAnimation()
    {
        if (lockAnimation) return;

        if (isTouchingWall && !isGrounded && rb.velocity.y <= 0)
        {
            animator.Play("WallGrab");
            return;
        }

        if (!isGrounded)
        {
            if (rb.velocity.y > 0) animator.Play("Jump");
            else animator.Play("Fall");
        }
        else
        {
            if (Mathf.Abs(moveInput) > 0) animator.Play("Run");
            else animator.Play("Idle");
        }
    }

    void UpdateWallSmoke()
    {
        if (wallSmoke == null) return;

        if (isTouchingWall && !isGrounded)
        {
            if (!wallSmoke.isPlaying) wallSmoke.Play();
            smokeEmission.rateOverTime = Mathf.Abs(rb.velocity.y) * 15f;
            wallSmoke.transform.localPosition = new Vector3(wallSide * 0.3f, 0, 0);
        }
        else
        {
            if (wallSmoke.isPlaying) wallSmoke.Stop();
            smokeEmission.rateOverTime = 0;
        }
    }

    void HandleLandingSound()
    {
        if (!wasGrounded && isGrounded && landingClip != null && rb.velocity.y <= minLandingVelocity)
            audioSource.PlayOneShot(landingClip, landingVolume);

        wasGrounded = isGrounded;
    }

    void UpdateCloneCooldownUI()
    {
        if (cloneCooldownCircle == null) return;
        float elapsed = Time.time - lastCloneTime;
        float value = Mathf.Clamp01(elapsed / playerStats.bodySwapCooldown);
        cloneCooldownCircle.fillAmount = value;
    }

    public void EnableControl(bool value)
    {
        canMove = value;
        if (!value) rb.velocity = Vector2.zero;
    }

    public void PlayFootstep()
    {
        if (!isGrounded || footstepClip == null) return;
        audioSource.PlayOneShot(footstepClip, footstepVolume);
    }

    public void PlayPray(float duration) => StartCoroutine(PrayRoutine(duration));

    IEnumerator PrayRoutine(float duration)
    {
        lockAnimation = true;
        canMove = false;
        animator.Play("Pray", 0, 0f);
        yield return new WaitForSeconds(duration);
        lockAnimation = false;
        canMove = true;
    }

    public void SetCloneMode(bool value)
    {
        isCloneMode = value;
        rb.gravityScale = isCloneMode ? 0.7f : 1.5f;
    }

    public void PlayAttackAnimation() => StartCoroutine(AttackRoutine());

    IEnumerator AttackRoutine()
    {
        bool wasLocked = lockAnimation;

        lockAnimation = true;
        canMove = false;

        Vector2 originalVelocity = rb.velocity;
        rb.velocity = Vector2.zero;

        // 🔥 เล่นจากเฟรมแรก
        animator.Play("Attack", 0, 0f);

        // 🔥 รอให้ animation เข้า state ก่อน (สำคัญมาก)
        yield return null;

        // 🔥 ดึงเวลาจริงของ animation
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;

        // 🔥 รอจนเล่นจบ
        yield return new WaitForSeconds(animLength);

        // 🔥 ปลดล็อค
        rb.velocity = originalVelocity;
        lockAnimation = wasLocked;
        canMove = true;
    }

    public void PlayFinishAttackAnimation()
    {
        if (animator != null && !lockAnimation) StartCoroutine(FinishAttackRoutine());
    }

    public IEnumerator FinishAttackRoutine()
    {
        lockAnimation = true;
        canMove = false;

        Vector2 originalVelocity = rb.velocity;
        rb.velocity = Vector2.zero;

        // 🔥 เล่น animation จากเฟรมแรก
        animator.Play("FinishAttack", 0, 0f);

        // 🔥 รอให้ animation เล่นจริง (ใส่เวลาจริงของคลิป เช่น 0.6f - 1f)
        yield return new WaitForSeconds(0.7f);

        // 🔥 ค้างเฟรมสุดท้าย
        animator.speed = 0f;

        // ❌ ไม่ต้องปลด lock ไม่งั้นโดน Idle ทับ
        // lockAnimation = false;
        // canMove = true;
    }
}