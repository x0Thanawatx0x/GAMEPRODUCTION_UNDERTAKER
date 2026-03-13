using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Game/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    public float runSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Abilities")]
    public bool canDoubleJump = false;

    [Header("Skill")]
    public float bodySwapCooldown = 5f;
}