using System.Collections;
using UnityEngine;

public class DoorSceneTrigger : MonoBehaviour
{
    [Header("Ghost Requirement")]
    public int requiredGhostAmount = 3;

    [Header("Upgrade Panel")]
    public UpgradeManager upgradeManager;

    [Header("Pray Animation")]
    public float prayDuration = 2f;

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

            StartCoroutine(
                PraySequence(player, lifeManager)
            );
        }
        else
        {
            Debug.Log("วิญญาณยังไม่พอ");
        }
    }

    IEnumerator PraySequence(PlayerControllerMain player, PlayerLifeManager lifeManager)
    {
        // ▶ เล่น Pray
        player.PlayPray(prayDuration);

        // ⏳ รอ animation
        yield return new WaitForSeconds(prayDuration);

        // 🔥 แปลง Ghost → Money
        lifeManager.ConvertGhostToMoney();

        // 🃏 เปิด Upgrade Panel
        if (upgradeManager != null)
            upgradeManager.ShowUpgradePanel();

        Debug.Log("Pray เสร็จแล้ว → เปิดการ์ด");
    }
}