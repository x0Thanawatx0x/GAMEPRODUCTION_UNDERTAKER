using UnityEngine;

public class ClearSaveButton : MonoBehaviour
{
    public void ClearSave()
    {
        PlayerPrefs.DeleteKey("PlayerX");
        PlayerPrefs.DeleteKey("PlayerY");
        PlayerPrefs.DeleteKey("PlayerZ");

        PlayerPrefs.Save();

        Debug.Log("Save data cleared!");
    }
}
