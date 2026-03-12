using UnityEngine;
using UnityEngine.SceneManagement;

public class TrapRespawn : MonoBehaviour
{
    public string playerTag = "Player";

    [Header("Original Spawn")]
    public Transform originalSpawnPoint;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // ================= UI Manager =================
        PlayerLifeManager lifeManager = other.GetComponent<PlayerLifeManager>();

        if (lifeManager != null)
        {
            lifeManager.CountTrap();   // นับจำนวนครั้งที่ตก Trap
            lifeManager.ResetGhost();  // รีเซ็ต Ghost = 0
        }

        // ================= Respawn Position =================
        Vector3 respawnPos;

        if (PlayerPrefs.HasKey("PlayerX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerX");
            float y = PlayerPrefs.GetFloat("PlayerY");
            float z = PlayerPrefs.GetFloat("PlayerZ");

            respawnPos = new Vector3(x, y, z);
        }
        else
        {
            if (originalSpawnPoint == null)
            {
                Debug.LogError("Original Spawn Point not assigned!");
                return;
            }

            respawnPos = originalSpawnPoint.position;
        }

        // บันทึกตำแหน่ง respawn ชั่วคราว
        PlayerPrefs.SetFloat("PlayerX", respawnPos.x);
        PlayerPrefs.SetFloat("PlayerY", respawnPos.y);
        PlayerPrefs.SetFloat("PlayerZ", respawnPos.z);

        PlayerPrefs.Save();

        // ================= Reload Scene =================
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}