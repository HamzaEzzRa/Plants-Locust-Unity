using System.Collections.Generic;
using UnityEngine;

public enum InventoryLocation
{
    PLAYER,
    CHEST,
    COUNT
}

public enum InventoryActionType
{
    ADD,
    SWAP,
    REMOVE
}

[System.Serializable]
public struct InventoryItem
{
    public InventoryItem(int id, int quantity)
    {
        this.id = id;
        this.quantity = quantity;
    }

    public int id;
    public int quantity;
}

public class InventoryManager : SingletonMonoBehavior<InventoryManager>
{
    public bool IsPlayerAtFullCapacity
    {
        get => inventoryLists[(int)InventoryLocation.PLAYER].Count >= inventoryLists[(int)InventoryLocation.PLAYER].Capacity;
    }

    [SerializeField] private ItemListSO itemListSO = default;

    private int[] initialCapacities = new int[] { 20, 50 };
    private int[] maxCapacities = new int[] { 40, 100 };

    private List<InventoryItem>[] inventoryLists;

    private Dictionary<int, ItemData> itemDataDictionary;

    protected override void Awake()
    {
        base.Awake();
        PopulateItemDictionary();
        GenerateInventoryLists();
    }

    private void PopulateItemDictionary()
    {
        itemDataDictionary = new Dictionary<int, ItemData>(itemListSO.ItemList.Count);

        foreach(ItemData data in itemListSO.ItemList)
        {
            itemDataDictionary.Add(data.ID, data);
        }
    }

    private void GenerateInventoryLists()
    {
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.COUNT];
        for (int i = 0; i < (int)InventoryLocation.COUNT; i++)
        {
            inventoryLists[i] = new List<InventoryItem>(initialCapacities[i]);
            inventoryLists.Initialize();
        }
    }

    public ItemData GetItemData(int itemId)
    {
        itemDataDictionary.TryGetValue(itemId, out ItemData itemData);
        return itemData;
    }

    public ItemData GetItemData(string itemName)
    {
        foreach (ItemData data in itemDataDictionary.Values)
        {
            if (data.Name == itemName)
            {
                return data;
            }
        }
        return null;
    }

    public void AddItem(InventoryLocation inventoryLocation, int id, int quantity)
    {
        if (GetItemData(id) == null)
        {
            return;
        }
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];
        int itemPosition = FindItemInInventory(inventoryLocation, id);

        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryLocation, id, itemPosition, quantity);
        }
        else
        {
            AddItemAtPosition(inventoryLocation, id, quantity);
        }

        EventHandler.CallInventoryUpdateEvent(inventoryLocation, InventoryActionType.ADD, id, quantity);
    }

    public void AddItem(InventoryLocation inventoryLocation, Item item)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        int itemPosition = FindItemInInventory(inventoryLocation, item.id);

        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryLocation, item.id, itemPosition, item.quantity);
        }
        else
        {
            AddItemAtPosition(inventoryLocation, item.id, item.quantity);
            itemPosition = inventoryList.Count - 1;
        }

        EventHandler.CallInventoryUpdateEvent(inventoryLocation, InventoryActionType.ADD, item.id, item.quantity);
    }

    public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject pickableObject, Transform target, float inverseSpeed)
    {
        float distance = Vector3.Distance(pickableObject.transform.position, target.position);

        item.IsBeingPicked = true;
        LeanTween.move(pickableObject, target, distance * inverseSpeed * 0.5f)
            .setOnComplete(() => {
                AddItem(inventoryLocation, item);
                item.IsBeingPicked = false;
                Destroy(pickableObject);
            });
    }

    public void SwapItems(InventoryLocation inventoryLocation, int startSlotNumber, int targetSlotNumber)
    {
        List<InventoryItem> itemList = inventoryLists[(int)inventoryLocation];
        if (startSlotNumber < itemList.Capacity && targetSlotNumber < itemList.Capacity
            && startSlotNumber != targetSlotNumber && startSlotNumber >= 0 && targetSlotNumber >= 0)
        {
            EventHandler.CallInventoryUpdateEvent(inventoryLocation, InventoryActionType.SWAP, startSlotNumber, targetSlotNumber);
        }
    }

    public void RemoveItem(InventoryLocation location, int id, int quantity)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)location];

        int itemPosition = FindItemInInventory(location, id);

        if (itemPosition != -1)
        {
            if (quantity > inventoryList[itemPosition].quantity)
            {
                Debug.LogError("Trying to remove " + quantity + " of item " + id +
                    " while only having " + inventoryList[itemPosition].quantity + " in inventory " + location.ToString());
                quantity = inventoryList[itemPosition].quantity;
            }

            RemoveItemAtPosition(inventoryList, itemPosition, quantity);
            EventHandler.CallInventoryUpdateEvent(location, InventoryActionType.REMOVE, id, -quantity);
        }
    }

    private int FindItemInInventory(InventoryLocation location, int id)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)location];

        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].id == id)
                return i;
        }

        return -1;
    }

    private void AddItemAtPosition(InventoryLocation location, int id, int position, int quantity)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)location];

        InventoryItem inventoryItem = new InventoryItem(id, inventoryList[position].quantity + quantity);
        inventoryList[position] = inventoryItem;

        //DebugPrintInventory(location);
    }

    private void AddItemAtPosition(InventoryLocation location, int id, int quantity)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)location];

        InventoryItem inventoryItem = new InventoryItem(id, quantity);
        inventoryList.Add(inventoryItem);

        //DebugPrintInventory(location);
    }

    private void RemoveItemAtPosition(List<InventoryItem> inventoryList, int position, int quantity)
    {
        if (quantity == inventoryList[position].quantity)
        {
            inventoryList.RemoveAt(position);
        }
        else
        {
            inventoryList[position] = new InventoryItem(inventoryList[position].id, inventoryList[position].quantity - quantity);
        }
    }

    private void DebugPrintInventory(InventoryLocation location)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)location];

        Debug.ClearDeveloperConsole();
        foreach (InventoryItem item in inventoryList)
        {
            Debug.Log("Item Name -> " + GetItemData(item.id).Name + "\tItem Quantity -> " + item.quantity);
        }
    }
}
