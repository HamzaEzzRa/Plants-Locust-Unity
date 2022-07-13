using UnityEngine;

[CreateAssetMenu]
public class WarFactory : GameObjectFactory
{
    public Shell Shell => Get(shellPrefab);

    public Explosion Explosion => Get(explosionPrefab);

    public PoisonCloud PoisonCloud => Get(cloudPrefab);

    [SerializeField] private Shell shellPrefab = default;
    [SerializeField] private Explosion explosionPrefab = default;
    [SerializeField] private PoisonCloud cloudPrefab = default;

    private T Get<T>(T prefab) where T : WarEntity
    {
        T instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance;
    }

    public void Reclaim(WarEntity entity)
    {
        if (entity.OriginFactory != this)
            return;
        Destroy(entity.gameObject);
    }
}