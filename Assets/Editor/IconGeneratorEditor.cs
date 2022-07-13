using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IconGenerator))]
public class IconGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(8);

        if (GUILayout.Button("Take Screenshot"))
        {
            IconGenerator iconGenerator = (IconGenerator)target;
            iconGenerator.Screenshot();
        }
    }
}
