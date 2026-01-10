using UnityEngine;

public class WorldSetting : MonoBehaviour
{
    [Header("Jump Setting")]
    [Tooltip("จำนวนครั้งที่ผู้เล่นสามารถกระโดดได้ในฉากนี้")]
    public int maxJumpCount = 1;

    public static WorldSetting Instance;

    void Awake()
    {   
        // ให้มีแค่ตัวเดียวต่อฉาก
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
