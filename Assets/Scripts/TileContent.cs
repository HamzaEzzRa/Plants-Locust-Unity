using UnityEngine;

[SelectionBase, System.Serializable]
public class TileContent : MonoBehaviour
{
    public static bool IsPlantType(TileType type) => type == TileType.FIRE_PLANT_TILE || type == TileType.GRENADE_PLANT_TILE || type == TileType.POISON_PLANT_TILE;

    public bool BlocksPath => type != TileType.PATH_TILE;

    [SerializeField] private TileType type = default;
    
    [SerializeField, HideInInspector] private float defaultOutlineWidth = 0.5f;
    [SerializeField, HideInInspector] private TileContentFactory originFactory;
    [SerializeField, HideInInspector] protected Outline outline;

    [HideInInspector] public Tile tile;

    public TileType Type
    {
        get => type;
        set
        {
            type = value;
        }
    }

    public TileContentFactory OriginFactory
    {
        get => originFactory;
        set
        {
            originFactory = value;
        }
    }

    protected void Awake()
    {
        outline = GetComponent<Outline>();
    }

    public void Recycle()
    {
        originFactory.Reclaim(this);
    }

    public void SetOutlineEnabled(bool value)
    {
        if (outline == null)
        {
            return;
        }

        if (value)
        {
            outline.OutlineWidth = defaultOutlineWidth;
        }
        else if (outline.OutlineWidth != 0f)
        {
            defaultOutlineWidth = outline.OutlineWidth;
            outline.OutlineWidth = 0f;
        }
    }

    public virtual void GameUpdate() {}
}
