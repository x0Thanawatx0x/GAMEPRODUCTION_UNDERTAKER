using UnityEngine;

public class PlatformPassenger : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
            col.transform.SetParent(transform);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
            col.transform.SetParent(null);
    }
}
