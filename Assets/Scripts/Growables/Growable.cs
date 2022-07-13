using UnityEngine;

[System.Serializable]
public class GrowableData
{
    public int ID => id;
    public string Name => name;
    public float TimeToWhole => timeToWhole;
    public float TimeToRotten => timeToRotten;
    public float TimeToHostile => timeToHostile;

    [SerializeField] private int id;
    [SerializeField] private string name;
    [ItemDescription, SerializeField] private int itemID;
    [SerializeField, Range(0f, 120f)] private float timeToWhole;
    [SerializeField, Range(0f, 120f)] private float timeToRotten;
    [SerializeField, Range(0f, 120f)] private float timeToHostile;
    [SerializeField] private GameObject itemPrefab;
}

public class Growable : Damageable
{
    public GrowableData GrowableData => growableData;

    public AnimationManager AnimationManager => animationManager;

    [GrowableDescription, SerializeField] protected int id;

    protected AnimationManager animationManager;
    protected GrowableData growableData;

    protected virtual void Start()
    {
        growableData = GrowableManager.Instance.GetGrowableData(id);
        animationManager = gameObject.AddComponent<AnimationManager>();
    }
}
