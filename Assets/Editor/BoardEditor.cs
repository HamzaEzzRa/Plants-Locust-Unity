using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class BoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(8);

        if (GUILayout.Button("Generate Tiles"))
        {
            GameManager manager = (GameManager)target;
            manager.Board.Initialize(manager.TileContentFactory);
        }

        GUILayout.Space(8);

        if (GUILayout.Button("Update Board"))
        {
            GameManager manager = (GameManager)target;
            manager.Board.UpdateTiles();
        }

        GUILayout.Space(8);

        if (GUILayout.Button("Clear Board"))
        {
            GameManager manager = (GameManager)target;
            manager.Board.ClearBoard();
        }
    }
}
