using UnityEngine;
using System.Collections.Generic;

public class GhostOrbRespawn : MonoBehaviour
{
    static List<GhostOrbRespawn> collectedOrbs = new List<GhostOrbRespawn>();

    public GameObject pressEUI; // 🔥 ลาก UI มาใส่ใน Inspector

    bool playerInRange = false;

    void Start()
    {
        if (pressEUI != null)
            pressEUI.SetActive(false); // เริ่มต้นซ่อนไว้
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Collect();
        }
    }

    public void Collect()
    {
        Debug.Log("Orb collected: " + gameObject.name);

        if (!collectedOrbs.Contains(this))
            collectedOrbs.Add(this);

        if (pressEUI != null)
            pressEUI.SetActive(false);

        gameObject.SetActive(false);
    }

    public void Respawn()
    {
        Debug.Log("Respawning orb: " + gameObject.name);

        gameObject.SetActive(true);

        if (pressEUI != null)
            pressEUI.SetActive(false);
    }

    public static void RespawnAll()
    {
        Debug.Log("Total collected orbs: " + collectedOrbs.Count);

        foreach (GhostOrbRespawn orb in collectedOrbs)
        {
            if (orb != null)
                orb.Respawn();
        }

        collectedOrbs.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (pressEUI != null)
                pressEUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (pressEUI != null)
                pressEUI.SetActive(false);
        }
    }
}