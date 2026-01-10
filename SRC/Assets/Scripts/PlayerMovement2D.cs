using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    // ⭐ QTE : คุมการเคลื่อนไหวจากภายนอก
    public static bool canMove = true;

    [Header("=== Input Keys (Inspector Editable) ===")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode dashKey = KeyCode.F;
    [SerializeField] KeyCode shadowKey = KeyCode.LeftShift;

    [Header("=== Player Movement ===")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float jumpForce = 10f;

    [Header("=== Dash Settings ===")]
    [SerializeField] float dashSpeed = 15f;
    [SerializeField] float dashDuration = 0.15f;
    [SerializeField] float dashCooldown = 0.5f;

    [Header("=== Gravity Settings ===")]
    [SerializeField] float normalGravity = 3f;
    [SerializeField] float wallSlideGravity = 0.5f;

    [Header("=== Wall Slide Settings ===")]
    [SerializeField] float wallSlideSpeed = 2f;

    [Header("=== Ground Raycast Settings ===")]
    [SerializeField] Transform groundRayOrigin;
    [SerializeField] float groundRayLength = 0.3f;
    [SerializeField] LayerMask groundLayer;

    [Header("=== Wall Check ===")]
    [SerializeField] Transform wallCheck;
    [SerializeField] float wallCheckRadius = 0.2f;

    [Header("=== Wall Jump Settings ===")]
    [SerializeField] Vector2 wallJumpForce = new Vector2(8f, 12f);

    [Header("=== Shadow Settings ===")]
    [SerializeField] GameObject shadowPrefab;
    [SerializeField] Transform shadowSpawnPoint;
    [SerializeField] float shadowMaxDistance = 5f;
    [SerializeField] float shadowCooldown = 5f;

    [Header("=== Shadow Radius UI (World Space) ===")]
    [SerializeField] RectTransform radiusUI;
    [SerializeField] float uiScale = 100f;

    Rigidbody2D rb;
    Vector3 originalScale;

    float moveInput;
    bool isGrounded;
    bool wasGrounded;
    bool isTouchingWall;
    bool isWallSliding;

    public int maxJumpCount = 1;
    int currentJumpCount;

    // ===== Dash =====
    bool isDashing;
    float dashTimer;
    float lastDashTime = -100f;

    // ===== Shadow =====
    bool controllingShadow;
    GameObject shadowInstance;
    float lastShadowTime = -100f;
    bool canSpawnShadow => Time.time >= lastShadowTime + shadowCooldown;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = normalGravity;
        originalScale = transform.localScale;

        currentJumpCount = maxJumpCount;

        if (radiusUI != null)
        {
            float diameter = shadowMaxDistance * 2f * uiScale;
            radiusUI.sizeDelta = new Vector2(diameter, diameter);
            radiusUI.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // ⭐ QTE : ถ้าห้ามขยับ → หยุด input ทั้งหมด
        if (!canMove)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        // ===== Ground Raycast =====
        RaycastHit2D groundHit = Physics2D.Raycast(
            groundRayOrigin.position,
            Vector2.down,
            groundRayLength,
            groundLayer
        );

        isGrounded = groundHit.collider != null;

        if (isGrounded && !wasGrounded)
        {
            currentJumpCount = maxJumpCount;
        }
        wasGrounded = isGrounded;

        // ===== Wall Check =====
        Collider2D wallCollider = Physics2D.OverlapCircle(
            wallCheck.position,
            wallCheckRadius,
            groundLayer
        );

        isTouchingWall = wallCollider != null && wallCollider.CompareTag("ClimbWALL");

        // ===== Wall Slide =====
        isWallSliding = isTouchingWall && !isGrounded && moveInput != 0;
        rb.gravityScale = isWallSliding ? wallSlideGravity : normalGravity;

        // ===== Jump =====
        if (Input.GetKeyDown(jumpKey))
        {
            if (currentJumpCount > 0)
            {
                Jump();
                currentJumpCount--;
            }
            else if (isWallSliding)
            {
                WallJump();
            }
        }

        // ===== Dash =====
        if (Input.GetKeyDown(dashKey) && CanDash())
            StartDash();

        // ===== Flip =====
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(moveInput) * Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }

        // ===== Shadow Mode =====
        if (Input.GetKey(shadowKey) && canSpawnShadow && !controllingShadow)
            EnterShadowMode();

        if (!Input.GetKey(shadowKey) && controllingShadow)
            ExitShadowMode();

        if (radiusUI != null)
            radiusUI.position = transform.position;
    }

    void FixedUpdate()
    {
        // ⭐ QTE : ไม่ให้ขยับ
        if (!canMove)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        // ===== Dash =====
        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;
            float dashDir = Mathf.Sign(transform.localScale.x);
            rb.velocity = new Vector2(dashDir * dashSpeed, 0f);

            if (dashTimer <= 0)
                EndDash();

            return;
        }

        // ===== Normal Movement =====
        if (!controllingShadow)
        {
            float targetY = rb.velocity.y;

            if (isWallSliding)
                targetY = Mathf.Lerp(rb.velocity.y, -wallSlideSpeed, 5f * Time.fixedDeltaTime);

            rb.velocity = new Vector2(moveInput * walkSpeed, targetY);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    // ================= FUNCTIONS =================

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void WallJump()
    {
        float dir = -Mathf.Sign(transform.localScale.x);
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(wallJumpForce.x * dir, wallJumpForce.y), ForceMode2D.Impulse);

        transform.localScale = new Vector3(
            dir * Mathf.Abs(originalScale.x),
            originalScale.y,
            originalScale.z
        );
    }

    bool CanDash()
    {
        return !isDashing && !controllingShadow && Time.time >= lastDashTime + dashCooldown;
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        lastDashTime = Time.time;
        rb.gravityScale = 0f;
    }

    void EndDash()
    {
        isDashing = false;
        rb.gravityScale = normalGravity;
    }

    void EnterShadowMode()
    {
        shadowInstance = Instantiate(shadowPrefab, shadowSpawnPoint.position, Quaternion.identity);
        ShadowController shadow = shadowInstance.GetComponent<ShadowController>();
        shadow.centerTarget = transform;
        shadow.maxDistance = shadowMaxDistance;

        if (radiusUI != null) radiusUI.gameObject.SetActive(true);
        controllingShadow = true;
    }

    void ExitShadowMode()
    {
        rb.velocity = Vector2.zero;
        transform.position = shadowInstance.transform.position;

        Destroy(shadowInstance);
        lastShadowTime = Time.time;

        if (radiusUI != null) radiusUI.gameObject.SetActive(false);
        controllingShadow = false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundRayOrigin != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                groundRayOrigin.position,
                groundRayOrigin.position + Vector3.down * groundRayLength
            );
        }
    }
}
