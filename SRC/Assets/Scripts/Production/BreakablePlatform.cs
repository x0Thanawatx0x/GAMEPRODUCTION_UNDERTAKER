using UnityEngine;
using System.Collections;

public class BreakablePlatform : MonoBehaviour
{
    [Header("Respawn")]
    public float respawnTime = 2f;

    [Header("Animation")]
    public Animator animator;
    public string breakAnimationName = "Break";

    private Collider2D col;
    private SpriteRenderer sr;
    private bool isBroken = false;

    void Start()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBroken) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(BreakRoutine());
        }
    }

    IEnumerator BreakRoutine()
    {
        isBroken = true;

        // 🔥 เล่น Animation แตก
        if (animator != null)
        {
            animator.Play(breakAnimationName, 0, 0f);
        }

        // 🔥 รอ 1 เฟรมให้ state อัปเดต
        yield return null;

        // 🔥 ดึงความยาว animation จริง
        float animLength = 0.5f;

        if (animator != null)
        {
            animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        }

        // 🔥 รอจนเล่นจบ
        yield return new WaitForSeconds(animLength);

        // 🔥 ปิด collider + sprite
        col.enabled = false;
        sr.enabled = false;

        // 🔥 รอ respawn
        yield return new WaitForSeconds(respawnTime);

        // 🔥 เปิดกลับ
        col.enabled = true;
        sr.enabled = true;

        isBroken = false;

        // =========================
        // 🔥 แก้ตรงนี้ (สำคัญมาก)
        // =========================
        if (animator != null)
        {
            animator.Rebind();          // รีเซ็ต animator ทั้งหมด
            animator.Update(0f);        // บังคับอัปเดตทันที
            animator.Play("Idle", 0, 0f); // เล่น Idle จากเฟรมแรก
        }
    }
}