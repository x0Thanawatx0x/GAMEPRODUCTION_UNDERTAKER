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
    [SerializeField] Transform wallCheck;
    [SerializeField] float wallCheckRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;

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
    private bool isTouchingWall;
    private bool controllingShadow;
    private GameObject shadowInstance;
    private float lastShadowTime = -100f;

    // เช็คว่าพร้อมใช้ร่างโคลนหรือยัง
    bool canSpawnShadow => Time.time >= lastShadowTime + shadowCooldown;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = normalGravity;

        if (radiusUI != null)
        {
            float diameter = shadowMaxDistance * 2f * uiScale;
            radiusUI.sizeDelta = new Vector2(diameter, diameter);
            radiusUI.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // ระบบ Shadow Clone พร้อม Cooldown
        if (Input.GetKeyDown(KeyCode.F) && canSpawnShadow)
            EnterShadowMode();

        if (Input.GetKeyUp(KeyCode.F))
            ExitShadowMode();

        // ถ้าไม่ได้ควบคุมร่างโคลน ให้เดิน/กระโดดได้ปกติ
        if (!controllingShadow)
        {
            HandleInputs();
        }

        if (radiusUI != null) radiusUI.position = transform.position;
    }

    void FixedUpdate()
    {
        if (!controllingShadow)
        {
            ApplyMovement();
        }
        else
        {
            // หยุดตัวผู้เล่นไว้กลางอากาศขณะคุมร่างโคลน
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void HandleInputs()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // เช็คพื้นและกำแพง
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        Collider2D wallCollider = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, groundLayer);
        isTouchingWall = (wallCollider != null && wallCollider.CompareTag("ClimbWALL"));

        // การกระโดด
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (isTouchingWall)
            {
                PerformWallJump();
            }
        }

        // กลับหน้าตัวละคร
        if (moveInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
    }

    void ApplyMovement()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    void PerformWallJump()
    {
        // ทิศทางกระโดดจะตรงข้ามกับที่หันหน้าอยู่
        float jumpDir = -transform.localScale.x;
        rb.velocity = new Vector2(wallJumpForce.x * jumpDir, wallJumpForce.y);

        // หันหน้าตัวละครไปตามทางที่กระโดด
        transform.localScale = new Vector3(jumpDir, 1, 1);
    }

    // ================= Shadow Mode Logic =================

    void EnterShadowMode()
    {
        if (shadowInstance != null) return;

        // ⭐ จุดสำคัญ: ล้างความเร็วเก่า (เช่น แรงส่งจากการกระโดด) ทันทีที่กด F
        rb.velocity = Vector2.zero;

        shadowInstance = Instantiate(shadowPrefab, shadowSpawnPoint.position, Quaternion.identity);

        ShadowController shadow = shadowInstance.GetComponent<ShadowController>();
        shadow.centerTarget = transform;
        shadow.maxDistance = shadowMaxDistance;

        // เปลี่ยนเป็นแรงดึงดูดต่ำ (ทำให้ตัวละครค่อยๆ ลอยลง หรือค้างอยู่เกือบตำแหน่งเดิม)
        rb.gravityScale = shadowModeGravity;

        if (radiusUI != null) radiusUI.gameObject.SetActive(true);
        controllingShadow = true;
    }

    void ExitShadowMode()
    {
        if (shadowInstance == null) return;

        // วาร์ปตัวผู้เล่นไปที่ร่างโคลน
        rb.velocity = Vector2.zero;
        transform.position = shadowInstance.transform.position;
        rb.gravityScale = normalGravity;

        Destroy(shadowInstance);

        // เริ่มนับ Cooldown
        lastShadowTime = Time.time;

        if (radiusUI != null) radiusUI.gameObject.SetActive(false);
        controllingShadow = false;
    }
}