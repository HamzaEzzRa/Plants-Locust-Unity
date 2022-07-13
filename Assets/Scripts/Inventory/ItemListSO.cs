using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemListSO : ScriptableObject
{
    public List<ItemData> ItemList => itemList;

    [SerializeField] private List<ItemData> itemList;
}
