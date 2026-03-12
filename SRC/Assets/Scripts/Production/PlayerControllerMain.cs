using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class PlayerControllerMain : MonoBehaviour
{
    public bool canMove = true;

    [Header("=== Movement ===")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float jumpForce = 10f;

    [Header("=== Ground Check ===")]
    [SerializeField] Transform groundRayOrigin;
    [SerializeField] float groundRayLength = 0.3f;
    [SerializeField] LayerMask groundLayer;

    [Header("=== Wall Check ===")]
    [SerializeField] Transform wallCheck;
    [SerializeField] float wallCheckDistance = 0.3f;
    [SerializeField] LayerMask wallLayer;

    [Header("=== Wall Movement ===")]
    [SerializeField] float wallSlideSpeed = 2f;
    [SerializeField] float wallJumpForceX = 8f;
    [SerializeField] float wallJumpForceY = 10f;

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

    [Header("=== Clone Range & Cooldown ===")]
    [SerializeField] float cooldownDuration = 5f;

    [Header("=== Clone Cooldown UI (Radial) ===")]
    [SerializeField] Image cloneCooldownCircle;

    private float lastCloneTime = -999f;

    public bool CanUseClone()
    {
        return Time.time >= lastCloneTime + cooldownDuration;
    }

    public void StartCloneCooldown()
    {
        lastCloneTime = Time.time;
    }

    Rigidbody2D rb;
    AudioSource audioSource;

    Vector3 originalScale;

    float moveInput;
    bool isGrounded;
    bool wasGrounded;
    bool isTouchingWall;
    bool isFacingRight = true;

    int wallSide;
    ParticleSystem.EmissionModule smokeEmission;

    // 🔒 ล็อค animation (เพิ่มใหม่)
    bool lockAnimation = false;

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

    void UpdateCloneCooldownUI()
    {
        if (cloneCooldownCircle == null) return;

        float elapsed = Time.time - lastCloneTime;
        float value = Mathf.Clamp01(elapsed / cooldownDuration);
        cloneCooldownCircle.fillAmount = value;
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        if (isTouchingWall && !isGrounded && rb.velocity.y <= 0)
            rb.velocity = new Vector2(0, -wallSlideSpeed);
        else
            rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y);
    }

    public void EnableControl(bool value)
    {
        canMove = value;
        if (!value) rb.velocity = Vector2.zero;
    }

    void HandleAnimation()
    {
        // 🔒 ถ้ากำลัง Pray ให้หยุดระบบ animation อื่น
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

    // ⭐ ฟังก์ชันเล่น Pray (เพิ่มใหม่)
    public void PlayPray(float duration)
    {
        StartCoroutine(PrayRoutine(duration));
    }

    IEnumerator PrayRoutine(float duration)
    {
        lockAnimation = true;
        canMove = false;

        animator.Play("Pray", 0, 0f);

        yield return new WaitForSeconds(duration);

        lockAnimation = false;
        canMove = true;
    }

    void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            else if (isTouchingWall && wallSide != 0)
            {
                float jumpDir = -wallSide;
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(jumpDir * wallJumpForceX, wallJumpForceY), ForceMode2D.Impulse);
                isFacingRight = jumpDir > 0;
            }
        }
    }

    void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundRayOrigin.position, Vector2.down, groundRayLength, groundLayer);
        isGrounded = hit.collider != null;
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

    public void PlayFootstep()
    {
        if (!isGrounded || footstepClip == null) return;
        audioSource.PlayOneShot(footstepClip, footstepVolume);
    }
}