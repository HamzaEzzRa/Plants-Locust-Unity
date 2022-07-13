using UnityEngine;

public class SingletonMonoBehavior<T> : MonoBehaviour where T:MonoBehaviour
{
    private static T instance;

    public static T Instance => instance;

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
