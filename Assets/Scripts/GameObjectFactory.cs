using UnityEngine;

public abstract class GameObjectFactory : ScriptableObject
{
    private GameObject objectHolder;

    protected T CreateGameObjectInstance<T>(T prefab) where T : MonoBehaviour
    {
        if (objectHolder == null && (objectHolder = GameObject.Find(name)) == null)
        {
            objectHolder = new GameObject(name);
            objectHolder.transform.position = Vector3.zero;
            objectHolder.transform.rotation = Quaternion.identity;
        }
        T instance = Instantiate(prefab, objectHolder.transform);
        return instance;
    }
}
