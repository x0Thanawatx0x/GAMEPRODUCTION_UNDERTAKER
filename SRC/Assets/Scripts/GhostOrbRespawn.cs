using UnityEngine;
using System.Collections.Generic;

public class GhostOrbRespawn : MonoBehaviour
{
    static List<GhostOrbRespawn> collectedOrbs = new List<GhostOrbRespawn>();

    SpriteRenderer sr;
    Collider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public void Collect()
    {
        Debug.Log("Orb collected: " + gameObject.name);

        collectedOrbs.Add(this);

        if (sr != null)
            sr.enabled = false;

        if (col != null)
            col.enabled = false;
    }

    public void Respawn()
    {
        Debug.Log("Respawning orb: " + gameObject.name);

        if (sr != null)
        {
            Color c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, 1f);
            sr.enabled = true;
        }

        if (col != null)
            col.enabled = true;
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
}