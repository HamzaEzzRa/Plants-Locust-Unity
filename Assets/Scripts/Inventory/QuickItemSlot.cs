using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuickItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image highlighter;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemCountText;

    public Image Highlighter => highlighter;
    public Image ItemImage => itemImage;
    public TextMeshProUGUI ItemCountText => itemCountText;
    public bool IsEmpty => ItemQuantity <= 0;

    public ItemData ItemData { get; set; }
    public int ItemQuantity
    {
        get => itemQuantity;
        set
        {
            itemQuantity = value;
            if (value > 1)
            {
                itemCountText.SetText("x" + itemQuantity.ToString());
            }
            else
            {
                itemCountText.SetText("");
            }
        }
    }

    private Camera mainCamera;
    private bool isDragged;

    private Vector3 imageOriginalPos;
    private Vector3 textOriginalPos;

    private QuickItemBar parentBar;
    private int slotNumber;
    private int itemQuantity;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void Initialize(QuickItemBar quickItemBar, int number)
    {
        parentBar = quickItemBar;
        slotNumber = number;
    }

    public void SwapData(QuickItemSlot other)
    {
        Sprite sprite = itemImage.sprite;
        itemImage.sprite = other.itemImage.sprite;
        other.itemImage.sprite = sprite;

        Color color = itemImage.color;
        itemImage.color = other.itemImage.color;
        other.itemImage.color = color;

        string countText = ItemCountText.text;
        itemCountText.text = other.ItemCountText.text;
        other.itemCountText.text = countText;

        if (highlighter.gameObject.activeInHierarchy)
        {
            highlighter.gameObject.SetActive(false);
            other.highlighter.gameObject.SetActive(true);
        }
        else if (other.highlighter.gameObject.activeInHierarchy)
        {
            other.highlighter.gameObject.SetActive(false);
            highlighter.gameObject.SetActive(true);
        }

        ItemData itemData = ItemData;
        ItemData = other.ItemData;
        other.ItemData = itemData;

        int itemQuantity = ItemQuantity;
        ItemQuantity = other.ItemQuantity;
        other.ItemQuantity = itemQuantity;
    }

    public void Clear()
    {
        itemImage.sprite = null;
        itemImage.color = new Color(1f, 1f, 1f, 0f);
        itemCountText.text = "";

        ItemData = null;
        ItemQuantity = 0;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsEmpty && parentBar.Player.canSwitchEquipped)
        {
            isDragged = true;
            itemCountText.text = itemCountText.text;

            imageOriginalPos = itemImage.transform.position;
            textOriginalPos = itemCountText.transform.position;

            itemImage.transform.SetParent(parentBar.transform.parent);
            itemCountText.transform.SetParent(parentBar.transform.parent);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragged)
        {
            itemImage.transform.position = eventData.position;

            Vector2 offset = imageOriginalPos - textOriginalPos;
            itemCountText.transform.position = eventData.position - offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDragged)
        {
            QuickItemSlot targetSlot = eventData.pointerCurrentRaycast.gameObject?.GetComponent<QuickItemSlot>();
            if (targetSlot != null)
            {
                InventoryManager.Instance.SwapItems(InventoryLocation.PLAYER, slotNumber, targetSlot.slotNumber);
            }
            else
            {
                if (ItemData.IsDropable)
                {
                    Vector3 dropPosition = parentBar.Player.transform.position;
                    float radius = parentBar.Player.GetComponent<ItemPicker>().PickUpRadius;

                    Vector2 viewportPosition = mainCamera.ScreenToViewportPoint(eventData.position);
                    viewportPosition -= Vector2.one * 0.5f;
                    viewportPosition.Normalize();

                    dropPosition += new Vector3(viewportPosition.x, 0f, viewportPosition.y) * radius * 1.5f;
                    dropPosition.y = ItemData.Prefab.transform.position.y;

                    GameObject itemGameObject = Instantiate(ItemData.Prefab, dropPosition, ItemData.Prefab.transform.rotation);
                    Item item = itemGameObject.GetComponent<Item>();
                    item.id = ItemData.ID;
                    item.quantity = ItemQuantity;

                    InventoryManager.Instance.RemoveItem(InventoryLocation.PLAYER, item.id, ItemQuantity);
                }
            }

            itemImage.transform.SetParent(transform);
            itemCountText.transform.SetParent(transform);

            itemImage.transform.position = imageOriginalPos;
            itemCountText.transform.position = textOriginalPos;
            
            isDragged = false;
        }
    }
}
