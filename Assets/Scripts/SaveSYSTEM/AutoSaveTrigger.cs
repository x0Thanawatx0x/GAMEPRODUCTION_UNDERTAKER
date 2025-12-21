using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class AutoSaveTrigger : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text saveText;

    [Header("Setting")]
    public float finishDelay = 1.5f;

    private bool hasSaved = false;

    void Start()
    {
        if (saveText != null)
            saveText.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasSaved)
        {
            hasSaved = true;
            StartCoroutine(SaveProcess(other.transform));
        }
    }

    IEnumerator SaveProcess(Transform player)
    {
        // UI
        saveText.gameObject.SetActive(true);
        saveText.text = "Saving...";

        yield return new WaitForSeconds(0.5f);

        // ✅ Save Scene
        PlayerPrefs.SetString("SavedScene", SceneManager.GetActiveScene().name);

        // ✅ Save Position
        PlayerPrefs.SetFloat("PlayerX", player.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.position.y);
        PlayerPrefs.SetFloat("PlayerZ", player.position.z);

        PlayerPrefs.Save();

        saveText.text = "Finish";
        yield return new WaitForSeconds(finishDelay);

        saveText.gameObject.SetActive(false);
    }
}
