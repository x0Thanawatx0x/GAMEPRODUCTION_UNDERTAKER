using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Game/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    public float runSpeed = 5f;
    public float jumpForce = 10f;
    public bool canDoubleJump = false;

    [Header("Skill & Combat")]
    public float bodySwapCooldown = 5f;
    public float cloneRange = 5f;
    public float attackChargeTime = 3f;

    [Header("Stat Levels (For UI Slots)")]
    public int speedLevel = 0;
    public int jumpLevel = 0;
    public int cooldownLevel = 0;
    public int rangeLevel = 0;
    public int attackChargeLevel = 0;

    public void Save()
    {
        // บันทึกค่าพลังพื้นฐาน
        PlayerPrefs.SetFloat("runSpeed", runSpeed);
        PlayerPrefs.SetFloat("jumpForce", jumpForce);
        PlayerPrefs.SetInt("canDoubleJump", canDoubleJump ? 1 : 0);
        PlayerPrefs.SetFloat("bodySwapCooldown", bodySwapCooldown);
        PlayerPrefs.SetFloat("cloneRange", cloneRange);
        PlayerPrefs.SetFloat("attackChargeTime", attackChargeTime);

        // บันทึกเลเวล (สำหรับวาดช่อง UI)
        PlayerPrefs.SetInt("speedLevel", speedLevel);
        PlayerPrefs.SetInt("jumpLevel", jumpLevel);
        PlayerPrefs.SetInt("cooldownLevel", cooldownLevel);
        PlayerPrefs.SetInt("rangeLevel", rangeLevel);
        PlayerPrefs.SetInt("attackChargeLevel", attackChargeLevel);

        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("runSpeed")) runSpeed = PlayerPrefs.GetFloat("runSpeed");
        if (PlayerPrefs.HasKey("jumpForce")) jumpForce = PlayerPrefs.GetFloat("jumpForce");
        canDoubleJump = PlayerPrefs.GetInt("canDoubleJump", 0) == 1;
        if (PlayerPrefs.HasKey("bodySwapCooldown")) bodySwapCooldown = PlayerPrefs.GetFloat("bodySwapCooldown");
        if (PlayerPrefs.HasKey("cloneRange")) cloneRange = PlayerPrefs.GetFloat("cloneRange");
        if (PlayerPrefs.HasKey("attackChargeTime")) attackChargeTime = PlayerPrefs.GetFloat("attackChargeTime");

        // โหลดเลเวล
        speedLevel = PlayerPrefs.GetInt("speedLevel", 0);
        jumpLevel = PlayerPrefs.GetInt("jumpLevel", 0);
        cooldownLevel = PlayerPrefs.GetInt("cooldownLevel", 0);
        rangeLevel = PlayerPrefs.GetInt("rangeLevel", 0);
        attackChargeLevel = PlayerPrefs.GetInt("attackChargeLevel", 0);
    }
}