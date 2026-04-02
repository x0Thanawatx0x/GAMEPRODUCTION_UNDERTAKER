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

    [Header("Slide Transition")]
    public SlideTransition slideTransition;

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
        player.EnableControl(false);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        player.PlayPray(prayDuration);

        yield return new WaitForSeconds(prayDuration);

        lifeManager.ConvertGhostToMoney();

        if (upgradeManager != null)
        {
            upgradeManager.OnUpgradeComplete = () =>
            {
                Debug.Log("Upgrade เสร็จ → เริ่ม Save");

                if (autoSaveTrigger != null)
                {
                    StartCoroutine(autoSaveTrigger.SaveProcess(player.transform));
                    StartCoroutine(WaitAndLoadNextScene(autoSaveTrigger.finishDelay));
                }
                else
                {
                    if (!string.IsNullOrEmpty(nextSceneName))
                    {
                        // 🔍 DEBUG SLIDE
                        if (slideTransition != null)
                        {
                            Debug.Log("[SLIDE] เริ่ม Slide ไป Scene: " + nextSceneName);
                            slideTransition.SlideAndLoad(nextSceneName);
                        }
                        else
                        {
                            Debug.LogWarning("[SLIDE] ไม่มี SlideTransition → ใช้ LoadScene ปกติ");
                            SceneManager.LoadScene(nextSceneName);
                        }
                    }
                    else
                    {
                        Debug.LogError("[SLIDE] nextSceneName ว่าง!");
                    }
                }
            };

            upgradeManager.ShowUpgradePanel();
        }

        Debug.Log("Pray เสร็จแล้ว → เปิด Upgrade Panel");
    }

    IEnumerator WaitAndLoadNextScene(float delay)
    {
        Debug.Log("[SLIDE] รอเวลา Save: " + delay);

        yield return new WaitForSeconds(delay + 0.5f);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            if (slideTransition != null)
            {
                Debug.Log("[SLIDE] (After Save) เริ่ม Slide ไป Scene: " + nextSceneName);
                slideTransition.SlideAndLoad(nextSceneName);
            }
            else
            {
                Debug.LogWarning("[SLIDE] (After Save) ไม่มี SlideTransition → ใช้ LoadScene");
                SceneManager.LoadScene(nextSceneName);
            }
        }
        else
        {
            Debug.LogError("[SLIDE] (After Save) nextSceneName ว่าง!");
        }
    }
}