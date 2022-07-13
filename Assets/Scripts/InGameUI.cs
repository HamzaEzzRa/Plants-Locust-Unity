using UnityEngine;

public class InGameUI : SingletonMonoBehavior<InGameUI>
{
    public Transform canvas = default;
    [SerializeField] private Transform debugView = default;
    
    public void ToogleDebugView()
    {
        debugView.gameObject.SetActive(!debugView.gameObject.activeInHierarchy);
    }
}
