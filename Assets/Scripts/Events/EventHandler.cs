using System;

public static class EventHandler
{
    public static event Action<InventoryLocation, InventoryActionType, int, int> InventoryUpdateEvent;
    public static void CallInventoryUpdateEvent(InventoryLocation inventoryLocation,
        InventoryActionType actionType, int data1, int data2)
    {
        InventoryUpdateEvent?.Invoke(inventoryLocation, actionType, data1, data2);
    }

    public static event Action<int> EquippedItemUpdateEvent;
    public static void CallEquippedItemUpdateEvent(int equippedId)
    {
        EquippedItemUpdateEvent?.Invoke(equippedId);
    }

    public static event Action<ShopItemSlot> SelectedShopItemUpdateEvent;
    public static void CallSelectedShopItemUpdateEvent(ShopItemSlot itemSlot)
    {
        SelectedShopItemUpdateEvent?.Invoke(itemSlot);
    }

    public static event Action<int> MoneyUpdateEvent;
    public static void CallMoneyUpdateEvent(int change)
    {
        MoneyUpdateEvent?.Invoke(change);
    }
}
