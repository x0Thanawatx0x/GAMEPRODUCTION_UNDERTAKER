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
}