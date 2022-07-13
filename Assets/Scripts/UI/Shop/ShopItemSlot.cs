using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public enum Type
    {
        BUYABLE,
        SELLABLE,
    }

    public int Index { get; set; }

    public int ItemID => itemId;

    public int Quantity
    {
        get => quantity;
        set
        {
            quantity = value;
            if (value > 1)
            {
                quantityTextMesh.SetText("x" + value.ToString());
            }
            else
            {
                quantityTextMesh.SetText("");
            }
        }
    }

    public int Price
    {
        get => price;
        set
        {
            price = value;
            priceTextMesh.SetText(Extra.MoneyFormat(value));
        }
    }

    public Type ShopType => type;

    [SerializeField] private Image itemImage = default;
    [SerializeField] private TextMeshProUGUI quantityTextMesh = default, priceTextMesh = default;

    [SerializeField] private Color defaultColor = default;
    [SerializeField] private Color hoverColor = default;
    [SerializeField] private Color selectColor = default;

    [SerializeField] private Image[] toColor = default;

    private int itemId, quantity, price;
    private Type type;

    private ShopManager shopManager;

    public void Initialize(int index, int id, Sprite sprite, int price, Type type, ShopManager shopManager)
    {
        itemId = id;
        itemImage.sprite = sprite;
        itemImage.color = Color.white;

        Index = index;
        this.type = type;
        this.shopManager = shopManager;

        Price = price;
    }

    public void Initialize(int index, int id, Sprite sprite, int price, int quantity, Type type, ShopManager shopManager)
    {
        Initialize(index, id, sprite, price, type, shopManager);
        Quantity = quantity;
    }

    private void ChangeColor(Color color)
    {
        foreach (Image image in toColor)
        {
            image.color = color;
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (shopManager.ActiveItemSlot != this)
        {
            ChangeColor(hoverColor);
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (shopManager.ActiveItemSlot != this)
        {
            ChangeColor(defaultColor);
        }
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        EventHandler.CallSelectedShopItemUpdateEvent(this);
    }

    private void OnSelectedShopItemUpdate(ShopItemSlot itemSlot)
    {
        Color color = itemSlot != this ? defaultColor : selectColor;
        ChangeColor(color);
    }

    private void OnEnable()
    {
        EventHandler.SelectedShopItemUpdateEvent += OnSelectedShopItemUpdate;
    }

    private void OnDisable()
    {
        EventHandler.SelectedShopItemUpdateEvent -= OnSelectedShopItemUpdate;
    }
}
