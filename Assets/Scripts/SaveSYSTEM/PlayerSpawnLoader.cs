using UnityEngine;

public class PlayerSpawnLoader : MonoBehaviour
{
    void Start()
    {
        if (!PlayerPrefs.HasKey("PlayerX")) return;

        float x = PlayerPrefs.GetFloat("PlayerX");
        float y = PlayerPrefs.GetFloat("PlayerY");
        float z = PlayerPrefs.GetFloat("PlayerZ");

        transform.position = new Vector3(x, y, z);
    }
}
