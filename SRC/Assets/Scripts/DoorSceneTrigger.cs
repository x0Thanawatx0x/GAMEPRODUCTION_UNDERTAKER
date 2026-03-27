using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSceneTrigger : MonoBehaviour
{
    [Header("Ghost Requirement")]
    public int requiredGhostAmount = 3;

    [Header("Upgrade Panel")]
    public UpgradeManager upgradeManager;

    [Header("Pray Animation")]
    public float prayDuration = 2f;

    [Header("Next Scene")]
    public string nextSceneName;

    [Header("Auto Save")]
    public AutoSaveTrigger autoSaveTrigger;

    bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        PlayerLifeManager lifeManager = other.GetComponent<PlayerLifeManager>();
        PlayerControllerMain player = other.GetComponent<PlayerControllerMain>();

        if (lifeManager == null || player == null) return;

        if (lifeManager.GetGhost() >= requiredGhostAmount)
        {
            triggered = true;
            StartCoroutine(PraySequence(player, lifeManager));
        }
        else
        {
            Debug.Log("วิญญาณยังไม่พอ");
        }
    }

    IEnumerator PraySequence(PlayerControllerMain player, PlayerLifeManager lifeManager)
    {
        // ▶ เล่น Pray animation
        player.PlayPray(prayDuration);

        // ⏳ รอ animation
        yield return new WaitForSeconds(prayDuration);

        // 🔥 แปลง Ghost → Money
        lifeManager.ConvertGhostToMoney();

        // 🃏 เปิด Upgrade Panel
        if (upgradeManager != null)
        {
            upgradeManager.OnUpgradeComplete = () =>
            {
                Debug.Log("Upgrade เสร็จ → เริ่ม Save");

                if (autoSaveTrigger != null)
                {
                    // เรียก Coroutine ของ AutoSaveTrigger เพื่อ Save + แสดง UI
                    StartCoroutine(autoSaveTrigger.SaveProcess(player.transform));

                    // หลัง Save เสร็จ → เปลี่ยน Scene
                    // ใน AutoSaveTrigger เราจะต้องเพิ่ม callback หรือใช้ Delay นิดหน่อย
                    StartCoroutine(WaitAndLoadNextScene(autoSaveTrigger.finishDelay));
                }
                else
                {
                    // ถ้าไม่มี AutoSaveTrigger → เปลี่ยน Scene ทันที
                    if (!string.IsNullOrEmpty(nextSceneName))
                        SceneManager.LoadScene(nextSceneName);
                }
            };

            upgradeManager.ShowUpgradePanel();
        }

        Debug.Log("Pray เสร็จแล้ว → เปิด Upgrade Panel");
    }

    IEnumerator WaitAndLoadNextScene(float delay)
    {
        yield return new WaitForSeconds(delay + 0.5f); // +0.5f ตาม SaveProcess
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}