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

    [Header("=== Ground Check ===")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;

    [Header("=== Shadow Settings ===")]
    [SerializeField] GameObject shadowPrefab;
    [SerializeField] Transform shadowSpawnPoint;
    [SerializeField] float shadowMaxDistance = 5f;

    [Header("=== Shadow Radius UI (World Space) ===")]
    [SerializeField] RectTransform radiusUI;
    [SerializeField] float uiScale = 100f;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool controllingShadow;
    private GameObject shadowInstance;

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
        if (Input.GetKeyDown(KeyCode.F))
            EnterShadowMode();

        if (Input.GetKeyUp(KeyCode.F))
            ExitShadowMode();

        if (!controllingShadow)
            HandleMovement();

        // ให้ UI วงกลมตามผู้เล่นตลอด
        if (radiusUI != null)
            radiusUI.position = transform.position;
    }

    void FixedUpdate()
    {
        if (!controllingShadow)
            ApplyMovement();
        else
            rb.velocity = new Vector2(0, rb.velocity.y);
    }

    void HandleMovement()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        if (moveInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
    }

    void ApplyMovement()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    // ================= Shadow Mode =================
    void EnterShadowMode()
    {
        if (shadowInstance != null) return;

        shadowInstance = Instantiate(
            shadowPrefab,
            shadowSpawnPoint.position,
            Quaternion.identity
        );

        // ⭐ ผูกค่ารัศมี + ศูนย์กลางให้ร่างโคลน
        ShadowController shadow = shadowInstance.GetComponent<ShadowController>();
        shadow.centerTarget = transform;
        shadow.maxDistance = shadowMaxDistance;

        rb.gravityScale = shadowModeGravity;

        if (radiusUI != null)
            radiusUI.gameObject.SetActive(true);

        controllingShadow = true;
    }

    void ExitShadowMode()
    {
        if (shadowInstance == null) return;

        rb.velocity = Vector2.zero;
        transform.position = shadowInstance.transform.position;
        rb.gravityScale = normalGravity;

        Destroy(shadowInstance);

        if (radiusUI != null)
            radiusUI.gameObject.SetActive(false);

        controllingShadow = false;
    }
}
