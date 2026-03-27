using UnityEngine;
using UnityEngine.UI;

// เปลี่ยนชื่อคลาสเพื่อให้เหมาะสมกับหน้าที่ใหม่ (ตัดคำว่า Flip ออก)
public class CardVisualUI : MonoBehaviour
{
    [Header("UI References")]
    // ลากคอมโพเนนต์ Image ของการ์ดใบนี้มาใส่
    public Image cardImage;

    void Awake()
    {
        // ถ้าลืมลากใส่ ให้พยายามหา Image ในตัวมันเอง
        if (cardImage == null) cardImage = GetComponent<Image>();
    }

    // 🔥 ฟังก์ชันสำหรับเปลี่ยนรูปภาพหน้าการ์ดตรงๆ (No animation, No rotation)
    public void SetCardVisual(Sprite newFrontSprite)
    {
        if (cardImage != null && newFrontSprite != null)
        {
            cardImage.sprite = newFrontSprite;
        }

        // 🔒 บังคับให้การ์ดหน้าตรงและขนาดปกติเสมอ (กันค่าค้างจากอนิเมชันเก่า)
        transform.localRotation = Quaternion.identity; // Rotation 0,0,0
        transform.localScale = Vector3.one;           // Scale 1,1,1
    }
}