using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class CloneController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("=== Clone Range Limit ===")]   // ✅ เพิ่มใหม่
    [SerializeField] float cloneRange = 5f; // ✅ เพิ่มใหม่

    private Rigidbody2D rb;
    private Animator anim;
    private string lastAnim = "";
    private Vector2 input;
    private bool canControl = false;

    private bool facingRight = true;

    Vector3 spawnPosition; // ✅ เพิ่มใหม่

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Start() // ✅ เพิ่มใหม่
    {
        spawnPosition = transform.position;
    }

    void Update()
    {
        if (!canControl) return;

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        HandleAnimation();
    }

    void FixedUpdate()
    {
        if (!canControl) return;

        rb.velocity = input.normalized * moveSpeed;

        LimitMovementRange(); // ✅ เพิ่มใหม่
    }

    void LimitMovementRange() // ✅ เพิ่มใหม่
    {
        Vector2 offset = transform.position - spawnPosition;

        if (offset.magnitude > cloneRange)
        {
            offset = offset.normalized * cloneRange;
            rb.position = (Vector2)spawnPosition + offset;
        }
    }

    void HandleAnimation()
    {
        if (anim == null) return;

        float x = input.x;
        float y = input.y;

        bool isRight = x > 0;
        bool isLeft = x < 0;
        bool isUp = y > 0;
        bool isDown = y < 0;

        if (isRight) facingRight = true;
        if (isLeft) facingRight = false;

        string newAnim = "";

        if (isUp)
        {
            newAnim = facingRight ? "FlyUpRight" : "FlyUpLeft";
        }
        else if (isDown)
        {
            newAnim = facingRight ? "FlyDownRight" : "FlyDownLeft";
        }
        else if (isRight || isLeft)
        {
            newAnim = facingRight ? "FlyUpRight" : "FlyUpLeft";
        }

        if (newAnim != "")
        {
            anim.SetBool("FlyUpRight", false);
            anim.SetBool("FlyUpLeft", false);
            anim.SetBool("FlyDownRight", false);
            anim.SetBool("FlyDownLeft", false);

            anim.SetBool(newAnim, true);
            lastAnim = newAnim;
        }
    }

    public void EnableControl(bool value)
    {
        canControl = value;

        if (!value)
            rb.velocity = Vector2.zero;
    }
}