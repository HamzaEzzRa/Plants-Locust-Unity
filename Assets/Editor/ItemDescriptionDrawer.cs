using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ItemDescriptionAttribute))]
public class ItemDescriptionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property) * 2f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            EditorGUI.BeginChangeCheck();

            var newValue = EditorGUI.IntField(new Rect(position.x, position.y, position.width, position.height * 0.5f), label, property.intValue);
            ItemData itemData = GetItemData(property.intValue);

            EditorGUI.LabelField(
                new Rect(position.x, position.y + position.height * 0.5f, position.width, position.height * 0.5f),
                "Item Name",
                itemData != null ? itemData.Name : "Not an Item"
            );

            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
        }

        EditorGUI.EndProperty();
    }

    private ItemData GetItemData(int id)
    {
        ItemListSO itemListSO = AssetDatabase.LoadAssetAtPath<ItemListSO>("Assets/ScriptableObjects/Main Item List.asset");
        ItemData data = itemListSO.ItemList.Find(x => x.ID == id);

        return data;
    }
}
