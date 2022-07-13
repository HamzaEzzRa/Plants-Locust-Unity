using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TooltipTrigger))]
public class TooltipTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        TooltipTrigger trigger = (TooltipTrigger)target;

        if (trigger.isBodyDynamic)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dynamicBody"), true);
            serializedObject.ApplyModifiedProperties();
        }
        else
        {
            trigger.body = EditorGUILayout.TextField("Body", trigger.body);
        }
    }
}
