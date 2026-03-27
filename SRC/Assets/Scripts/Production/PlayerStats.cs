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

    [Header("Clone")]
    public float cloneRange = 5f;

    // 🔥 เพิ่มใหม่
    [Header("Combat")]
    public float attackChargeTime = 3f;

    public void Save()
    {
        PlayerPrefs.SetFloat("runSpeed", runSpeed);
        PlayerPrefs.SetFloat("jumpForce", jumpForce);
        PlayerPrefs.SetInt("canDoubleJump", canDoubleJump ? 1 : 0);
        PlayerPrefs.SetFloat("bodySwapCooldown", bodySwapCooldown);
        PlayerPrefs.SetFloat("cloneRange", cloneRange);

        // 🔥 เพิ่ม
        PlayerPrefs.SetFloat("attackChargeTime", attackChargeTime);

        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("runSpeed")) runSpeed = PlayerPrefs.GetFloat("runSpeed");
        if (PlayerPrefs.HasKey("jumpForce")) jumpForce = PlayerPrefs.GetFloat("jumpForce");
        if (PlayerPrefs.HasKey("canDoubleJump")) canDoubleJump = PlayerPrefs.GetInt("canDoubleJump") == 1;
        if (PlayerPrefs.HasKey("bodySwapCooldown")) bodySwapCooldown = PlayerPrefs.GetFloat("bodySwapCooldown");
        if (PlayerPrefs.HasKey("cloneRange")) cloneRange = PlayerPrefs.GetFloat("cloneRange");

        // 🔥 เพิ่ม
        if (PlayerPrefs.HasKey("attackChargeTime"))
            attackChargeTime = PlayerPrefs.GetFloat("attackChargeTime");
    }
}