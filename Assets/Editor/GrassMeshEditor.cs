using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GrassPointCloudRenderer))]
public class GrassMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(8);

        if (GUILayout.Button("Update Mesh"))
        {
            GrassPointCloudRenderer renderer = (GrassPointCloudRenderer)target;
            renderer.GenerateCloudMesh();
        }
    }
}