using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory
{
    [SerializeField] private Enemy prefab = default;

    [SerializeField, FloatRangeSlider(0.5f, 2f)] private FloatRange scale = new FloatRange(1f);
    [SerializeField, FloatRangeSlider(0.2f, 5f)] private FloatRange speed = new FloatRange(1f);
    [SerializeField, FloatRangeSlider(-0.4f, 0.4f)] private FloatRange pathOffset = new FloatRange(0f);

    public Enemy Get()
    {
        Enemy instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        instance.Initialize(scale.RandomValueInRange, speed.RandomValueInRange, pathOffset.RandomValueInRange);
        return instance;
    }

    public void Reclaim(Enemy enemy)
    {
        if (enemy == null || enemy.OriginFactory != this)
            return;

        Destroy(enemy.gameObject);
    }
}
