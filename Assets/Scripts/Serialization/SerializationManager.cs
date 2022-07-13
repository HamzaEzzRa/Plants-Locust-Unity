using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager : MonoBehaviour
{
    public static bool Save(string name, object data)
    {
        BinaryFormatter formatter = GetBinaryFormatter();

        if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves");

        string path = Application.persistentDataPath + "/Saves" + name + ".save";
        FileStream file = File.Create(path);
        formatter.Serialize(file, data);
        file.Close();
        
        return true;
    }

    public static object Load(string path)
    {
        if (!File.Exists(path))
            return false;

        BinaryFormatter formatter = GetBinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);
        
        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch
        {
            Debug.LogErrorFormat("Failed to load save at {0}", path);
            file.Close();
            return null;
        }
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        return formatter;
    }

}
