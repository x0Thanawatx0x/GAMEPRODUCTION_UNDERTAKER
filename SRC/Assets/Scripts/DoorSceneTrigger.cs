using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSceneTrigger : MonoBehaviour
{
    [Header("Setting")]
    public string playerTag = "Player";

    [Header("Requirement")]
    public int requiredOrbAmount = 1;

    [Header("Scene")]
    public string nextSceneName;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        GhostOrbManager orbManager = other.GetComponent<GhostOrbManager>();

        if (orbManager == null)
        {
            Debug.LogWarning("Player has no GhostOrbManager!");
            return;
        }

        if (orbManager.GetOrbCount() >= requiredOrbAmount)
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log($"Need {requiredOrbAmount} GhostOrb, but player has {orbManager.GetOrbCount()}");
        }
    }
}
