using UnityEngine;

[System.Serializable]
public class SaveData : MonoBehaviour
{
    private static SaveData current;

    public static SaveData Current
    {
        get
        {
            if (current == null)
                current = new SaveData();

            return current;
        }
    }
}
