using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("=== Player Movement ===")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float runSpeed = 7f;
    [SerializeField] float jumpForce = 10f;

    [Header("=== Gravity Settings ===")]
    [SerializeField] float normalGravity = 3f;
    [SerializeField] float shadowModeGravity = 0.3f;

    [Header("=== Ground & Wall Check ===")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;
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

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool wasGrounded;
    private bool isTouchingWall;
    private bool controllingShadow;
    private GameObject shadowInstance;
    private float lastShadowTime = -100f;

    // ⭐ Jump Count
    public int maxJumpCount = 1; // ปรับจาก Inspector หรือ WorldSetting
    private int currentJumpCount;

    bool canSpawnShadow => Time.time >= lastShadowTime + shadowCooldown;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = normalGravity;

        // ⭐ รับค่า maxJumpCount จาก WorldSetting
        if (WorldSetting.Instance != null)
            maxJumpCount = WorldSetting.Instance.maxJumpCount;

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
        moveInput = Input.GetAxisRaw("Horizontal");

        // Ground & Wall Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        Collider2D wallCollider = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, groundLayer);
        isTouchingWall = (wallCollider != null && wallCollider.CompareTag("ClimbWALL"));

        // ⭐ รีเซ็ต Jump Count เฉพาะตอนผู้เล่นลงพื้น
        if (isGrounded && !wasGrounded)
        {
            currentJumpCount = maxJumpCount;
        }
        wasGrounded = isGrounded;

        // ⭐ Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded && currentJumpCount > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f); // reset y velocity
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                currentJumpCount--;
            }
            else if (!isGrounded && currentJumpCount > 0) // Double jump
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                currentJumpCount--;
            }
            else if (isTouchingWall) // Wall Jump (ไม่ลด jumpCount)
            {
                PerformWallJump();
            }
        }

        if (moveInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);

        // Shadow
        if (Input.GetKeyDown(KeyCode.F) && canSpawnShadow)
            EnterShadowMode();

        if (Input.GetKeyUp(KeyCode.F))
            ExitShadowMode();

        if (radiusUI != null)
            radiusUI.position = transform.position;
    }

    void FixedUpdate()
    {
        if (!controllingShadow)
        {
            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void PerformWallJump()
    {
        float jumpDir = -transform.localScale.x;
        rb.velocity = new Vector2(wallJumpForce.x * jumpDir, wallJumpForce.y);
        transform.localScale = new Vector3(jumpDir, 1, 1);
    }

    void EnterShadowMode()
    {
        if (shadowInstance != null) return;

        rb.velocity = Vector2.zero;
        shadowInstance = Instantiate(shadowPrefab, shadowSpawnPoint.position, Quaternion.identity);

        ShadowController shadow = shadowInstance.GetComponent<ShadowController>();
        shadow.centerTarget = transform;
        shadow.maxDistance = shadowMaxDistance;

        rb.gravityScale = shadowModeGravity;

        if (radiusUI != null) radiusUI.gameObject.SetActive(true);

        SetGhostOrbVisible(false);

        controllingShadow = true;
    }

    void ExitShadowMode()
    {
        if (shadowInstance == null) return;

        rb.velocity = Vector2.zero;
        transform.position = shadowInstance.transform.position;
        rb.gravityScale = normalGravity;

        Destroy(shadowInstance);
        lastShadowTime = Time.time;

        if (radiusUI != null) radiusUI.gameObject.SetActive(false);

        SetGhostOrbVisible(true);

        controllingShadow = false;
    }

    void SetGhostOrbVisible(bool visible)
    {
        GameObject[] orbs = GameObject.FindGameObjectsWithTag("GhostOrb");
        foreach (GameObject orb in orbs)
        {
            SpriteRenderer sr = orb.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.enabled = visible;
        }
    }
}
