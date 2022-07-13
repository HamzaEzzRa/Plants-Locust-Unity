using UnityEngine;

[CreateAssetMenu, System.Serializable]
public class TileContentFactory : GameObjectFactory
{
    [SerializeField]
    private TileContent
        pathPrefab = default,
        destinationPrefab = default,
        treePrefab = default,
        cropPrefab = default,
        spawnPrefab = default,
        emptyPrefab = default,
        carrotSeedPrefab = default;

    [SerializeField] private Plant
        firePlantPrefab = default,
        grenadePlantPrefab = default,
        poisonPlantPrefab = default;

    public TileContent Get(TileType type)
    {
        switch (type)
        {
            case TileType.DESTINATION_TILE: return Get(destinationPrefab);
            case TileType.PATH_TILE: return Get(pathPrefab);
            case TileType.TREE_TILE: return Get(treePrefab);
            case TileType.CROP_TILE: return Get(cropPrefab);
            case TileType.FIRE_PLANT_TILE: return Get(firePlantPrefab);
            case TileType.GRENADE_PLANT_TILE: return Get(grenadePlantPrefab);
            case TileType.POISON_PLANT_TILE: return Get(poisonPlantPrefab);
            case TileType.SPAWN_TILE: return Get(spawnPrefab);
            case TileType.EMPTY_TILE: return Get(emptyPrefab);
            case TileType.CARROT_PLANT_TILE: return Get(carrotSeedPrefab);
            default: return null;
        }
    }

    public void Reclaim(TileContent content)
    {
        if (content.OriginFactory != this)
        {
            return;
        }

        if (Application.isEditor && !UnityEditor.EditorApplication.isPlaying)
        {
            DestroyImmediate(content.gameObject);
        }
        else
        {
            Destroy(content.gameObject);
        }
    }

    private TileContent Get(TileContent prefab)
    {
        TileContent instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance;
    }
}
