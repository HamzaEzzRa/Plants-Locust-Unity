using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public ShopItemSlot ActiveItemSlot => activeItemSlot;

    [SerializeField] private ItemListSO itemListSO = default;
    [SerializeField] private Transform buyLayout = default;
    [SerializeField] private Transform sellLayout = default;

    [SerializeField] private ShopItemSlot buySlotPrefab = default;
    [SerializeField] private ShopItemSlot sellSlotPrefab = default;

    [SerializeField] private QuickItemBar quickItemBar = default;
    [SerializeField] private Button actionButton = default, maxButton = default;
    [SerializeField] private TMP_InputField quantityField = default;

    private ShopItemSlot activeItemSlot;
    private TextMeshProUGUI actionButtonTextMesh;

    private List<ShopItemSlot> buySlots = new List<ShopItemSlot>();
    private List<ShopItemSlot> sellSlots = new List<ShopItemSlot>();

    private void Start()
    {
        if (actionButtonTextMesh == null)
        {
            actionButtonTextMesh = actionButton.GetComponentInChildren<TextMeshProUGUI>(true);
        }

        SetInteractable(false);
        actionButtonTextMesh.SetText("");

        quantityField.onValueChanged.AddListener((string input) =>
        {
            if (input == "")
            {
                return;
            }

            int quantity = int.Parse(input);
            if (activeItemSlot)
            {
                if (activeItemSlot.ShopType == ShopItemSlot.Type.SELLABLE)
                {
                    if (activeItemSlot.Quantity < quantity)
                    {
                        quantityField.text = activeItemSlot.Quantity.ToString();
                    }
                }
                else
                {
                    int maxBuyableQuantity = GameManager.Instance.Player.CurrentMoney / activeItemSlot.Price;
                    if (quantity > maxBuyableQuantity)
                    {
                        quantityField.text = maxBuyableQuantity.ToString();
                    }
                }
            }
        });
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

    public void ActionEvent()
    {
        if (activeItemSlot == null)
        {
            actionButtonTextMesh.SetText("");
            SetInteractable(false);

            return;
        }
        int selectedQuantity = int.Parse(quantityField.text);

        if (activeItemSlot.ShopType == ShopItemSlot.Type.BUYABLE)
        {
            InventoryManager.Instance.AddItem(InventoryLocation.PLAYER, activeItemSlot.ItemID, selectedQuantity);
            EventHandler.CallMoneyUpdateEvent(-activeItemSlot.Price * selectedQuantity);
        }
        else if (activeItemSlot.ShopType == ShopItemSlot.Type.SELLABLE)
        {
            InventoryManager.Instance.RemoveItem(InventoryLocation.PLAYER, activeItemSlot.ItemID, selectedQuantity);
            EventHandler.CallMoneyUpdateEvent(activeItemSlot.Price * selectedQuantity);
            activeItemSlot.Quantity -= selectedQuantity;

            if (activeItemSlot.Quantity <= 0)
            {
                int index = activeItemSlot.Index;
                ShopItemSlot slot = sellSlots[index];
                sellSlots.RemoveAt(index);
                Destroy(slot.gameObject);
                
                activeItemSlot = null;
                actionButtonTextMesh.SetText("");
                SetInteractable(false);

                for (int i = index; i < sellSlots.Count; i++)
                {
                    sellSlots[i].Index--;
                }
            }
        }
    }

    private void ClearLayouts()
    {
        for (int i = buySlots.Count - 1; i >= 0; i--)
        {
            ShopItemSlot slot = buySlots[i];
            buySlots.RemoveAt(i);
            
            Destroy(slot.gameObject);
        }
        for (int i = sellSlots.Count - 1; i >= 0; i--)
        {
            ShopItemSlot slot = sellSlots[i];
            sellSlots.RemoveAt(i);

            Destroy(slot.gameObject);
        }

        activeItemSlot = null;
        actionButtonTextMesh.SetText("");
        SetInteractable(false);
    }

    private void SetInteractable(bool value)
    {
        string quantity = !value ? "0" : "1";

        quantityField.text = quantity;
        actionButton.interactable = value;
        quantityField.interactable = value;
        maxButton.interactable = value;
    }

    public void SetMax()
    {
        if (activeItemSlot == null)
        {
            return;
        }

        if (activeItemSlot.ShopType == ShopItemSlot.Type.SELLABLE)
        {
            quantityField.text = activeItemSlot.Quantity.ToString();
        }
        else
        {
            int maxBuyableQuantity = GameManager.Instance.Player.CurrentMoney / activeItemSlot.Price;
            quantityField.text = maxBuyableQuantity.ToString();
        }
    }

    private void OnSelectedShopItemUpdate(ShopItemSlot itemSlot)
    {
        activeItemSlot = itemSlot;
        
        string text = itemSlot.ShopType == ShopItemSlot.Type.BUYABLE ? "Buy" : "Sell";
        actionButtonTextMesh.SetText(text);

        SetInteractable(true);
    }

    private void OnEnable()
    {
        if (itemListSO == null)
        {
            return;
        }

        if (actionButtonTextMesh == null)
        {
            actionButtonTextMesh = actionButton.GetComponentInChildren<TextMeshProUGUI>(true);
        }

        foreach (ItemData data in itemListSO.ItemList)
        {
            if (data.IsBuyable)
            {
                ShopItemSlot slot = Instantiate(buySlotPrefab, buyLayout);
                slot.Initialize(buySlots.Count, data.ID, data.Sprite, data.BuyPrice, ShopItemSlot.Type.BUYABLE, this);

                buySlots.Add(slot);
            }
        }
        foreach (QuickItemSlot quickItem in quickItemBar.ItemSlots)
        {
            ItemData data = quickItem.ItemData;
            if (data != null && data.IsSellable)
            {
                ShopItemSlot slot = Instantiate(sellSlotPrefab, sellLayout);
                slot.Initialize(sellSlots.Count, data.ID, data.Sprite, data.SellPrice, quickItem.ItemQuantity, ShopItemSlot.Type.SELLABLE, this);

                sellSlots.Add(slot);
            }
        }

        EventHandler.SelectedShopItemUpdateEvent += OnSelectedShopItemUpdate;
    }

    private void OnDisable()
    {
        ClearLayouts();

        EventHandler.SelectedShopItemUpdateEvent -= OnSelectedShopItemUpdate;
    }
}
