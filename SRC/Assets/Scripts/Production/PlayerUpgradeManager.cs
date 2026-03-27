using UnityEngine;

public class PlayerUpgradeManager : MonoBehaviour
{
    public PlayerStats stats;

    public void UpgradeSpeed()
    {
        stats.runSpeed += 1f;
        Debug.Log("Speed Upgraded");
    }

    public void UpgradeJump()
    {
        stats.jumpForce += 2f;
        Debug.Log("Jump Upgraded");
    }

    public void UnlockDoubleJump()
    {
        stats.canDoubleJump = true;
        Debug.Log("Double Jump Unlocked");
    }

    public void ReduceCooldown()
    {
        stats.bodySwapCooldown -= 0.5f;

        if (stats.bodySwapCooldown < 1f)
            stats.bodySwapCooldown = 1f;

        Debug.Log("Cooldown Reduced");
    }

    // 🔥 ================= เพิ่มใหม่ =================
    [Header("Clone Upgrade")]
    public float cloneRangeUpgradeAmount = 5f;
    public float maxCloneRange = 100f;

    public void UpgradeCloneRange()
    {
        stats.cloneRange += cloneRangeUpgradeAmount;

        // กันเกิน max
        if (stats.cloneRange > maxCloneRange)
            stats.cloneRange = maxCloneRange;

        Debug.Log("Clone Range Upgraded: " + stats.cloneRange);
    }
    // ==============================================
}