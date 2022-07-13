using UnityEngine;
using System.Collections.Generic;

public class QuickItemBar : MonoBehaviour
{
    public QuickItemSlot[] ItemSlots => itemSlots;
    public Player Player => player;

    [SerializeField] private Player player;

    private QuickItemSlot[] itemSlots;
    private Dictionary<int, int> quickItemIndices;

    private int selectedItemIndex;

    private void Start()
    {
        itemSlots = GetComponentsInChildren<QuickItemSlot>();
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].Initialize(this, i);
        }

        quickItemIndices = new Dictionary<int, int>(itemSlots.Length);
    }

    public void SelectItem(int slotIndex)
    {
        if (!itemSlots[slotIndex].IsEmpty)
        {
            if (selectedItemIndex != -1)
            {
                itemSlots[selectedItemIndex].Highlighter.gameObject.SetActive(false);
            }
            selectedItemIndex = slotIndex;
            itemSlots[selectedItemIndex].Highlighter.gameObject.SetActive(true);

            EventHandler.CallEquippedItemUpdateEvent(itemSlots[slotIndex].ItemData.ID);
        }
    }

    private void OnInventoryUpdate(InventoryLocation inventoryLocation,
        InventoryActionType actionType, int data1, int data2)
    {
        if (inventoryLocation == InventoryLocation.PLAYER)
        {
            if (actionType == InventoryActionType.ADD || actionType == InventoryActionType.REMOVE)
            {
                int updateId = data1;
                int updateQuantity = data2;

                ItemData data = InventoryManager.Instance.GetItemData(updateId);

                int index;
                if (quickItemIndices.TryGetValue(updateId, out index))
                {
                    int totalQuantity = ItemSlots[index].ItemQuantity + updateQuantity;
                    if (totalQuantity <= 0)
                    {
                        if (selectedItemIndex == index)
                        {
                            selectedItemIndex = -1;
                            itemSlots[index].Highlighter.gameObject.SetActive(false);
                            EventHandler.CallEquippedItemUpdateEvent(-1);
                        }

                        itemSlots[index].Clear();
                        quickItemIndices.Remove(updateId);
                    }
                    else
                    {
                        itemSlots[index].ItemQuantity = totalQuantity;
                    }
                }
                else if (actionType == InventoryActionType.ADD && HasFreeSpace(out index))
                {
                    Color newColor = itemSlots[index].ItemImage.color;
                    newColor.a = 1f;
                    itemSlots[index].ItemImage.color = newColor;
                    itemSlots[index].ItemImage.sprite = data.Sprite;
                    itemSlots[index].ItemQuantity = updateQuantity;
                    itemSlots[index].ItemData = data;

                    quickItemIndices.Add(updateId, index);
                    if (quickItemIndices.Count == 1)
                    {
                        selectedItemIndex = index;
                        itemSlots[index].Highlighter.gameObject.SetActive(true);
                        EventHandler.CallEquippedItemUpdateEvent(updateId);
                    }
                }

                // Update shop manager sellable items quantity
            }
            else if (actionType == InventoryActionType.SWAP)
            {
                int startSlotNumber = data1;
                int targetSlotNumber = data2;

                quickItemIndices[itemSlots[startSlotNumber].ItemData.ID] = targetSlotNumber;
                if (!itemSlots[targetSlotNumber].IsEmpty)
                {
                    quickItemIndices[itemSlots[targetSlotNumber].ItemData.ID] = startSlotNumber;
                }

                if (selectedItemIndex == startSlotNumber)
                {
                    selectedItemIndex = targetSlotNumber;
                }
                else if (!itemSlots[targetSlotNumber].IsEmpty && selectedItemIndex == targetSlotNumber)
                {
                    selectedItemIndex = startSlotNumber;
                }

                itemSlots[startSlotNumber].SwapData(itemSlots[targetSlotNumber]);
            }
        }
    }

    private bool HasFreeSpace(out int index)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].IsEmpty)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    private void OnEnable()
    {
        EventHandler.InventoryUpdateEvent += OnInventoryUpdate;
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdateEvent -= OnInventoryUpdate;
    }
}
