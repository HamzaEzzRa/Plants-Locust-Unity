using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GrowableDescriptionAttribute))]
public class GrowableDescriptionDrawer : PropertyDrawer
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
            GrowableData growableData = GetGrowableData(property.intValue);

            EditorGUI.LabelField(
                new Rect(position.x, position.y + position.height * 0.5f, position.width, position.height * 0.5f),
                "Growable Name",
                growableData != null ? growableData.Name : "Not a Growable"
            );

            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
        }

        EditorGUI.EndProperty();
    }

    private GrowableData GetGrowableData(int id)
    {
        GrowableListSO growableListSO = AssetDatabase.LoadAssetAtPath<GrowableListSO>("Assets/ScriptableObjects/Main Growable List.asset");
        GrowableData data = growableListSO.GrowableList.Find(x => x.ID == id);

        return data;
    }
}
