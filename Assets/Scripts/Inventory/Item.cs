using UnityEngine;

public enum ItemType
{
    NONE,
    SEED,
    EQUIPMENT,
    CONSUMABLE,
}

[System.Serializable]
public class ItemData
{
    public int ID => id;

    public string Name => name;
    public ItemType Type => type;
    public TileType TileType => tileType;
    public Sprite Sprite => sprite;
    public GameObject Prefab => prefab;

    public int BuyPrice => buyPrice;
    public int SellPrice => sellPrice;

    public bool IsBuyable => buyPrice > 0;
    public bool IsSellable => sellPrice > 0;

    public bool IsPickable => isPickable;
    public bool IsDropable => isDropable;

    [SerializeField] private int id;
    [SerializeField] private ItemType type;
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject prefab;
    [SerializeField] private TileType tileType;

    [SerializeField] private int buyPrice;
    [SerializeField] private int sellPrice;
    [SerializeField] private bool isPickable;
    [SerializeField] private bool isDropable;
}

[RequireComponent(typeof(Renderer))]
public class Item : MonoBehaviour
{
    public ItemData ItemData => itemData;

    public bool IsBeingPicked { get => isBeingPicked; set => isBeingPicked = value; }

    [ItemDescription, SerializeField] public int id;

    public int quantity = 1;

    private ItemData itemData;

    private bool isBeingPicked;

    private void Start()
    {
        itemData = InventoryManager.Instance.GetItemData(id);
    }
}
