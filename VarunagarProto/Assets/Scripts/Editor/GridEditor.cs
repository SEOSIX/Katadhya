using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GlobalPlayerData))]
public class GlobalPlayerDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GlobalPlayerData data = (GlobalPlayerData)target;
        EditorGUILayout.LabelField("Cauris", EditorStyles.boldLabel);
        data.caurisCount = EditorGUILayout.IntField("Cauris Count", data.caurisCount);
        
        
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Inventory Dimensions", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        data.width = EditorGUILayout.IntField("Width", data.width);
        data.height = EditorGUILayout.IntField("Height", data.height);
        if (EditorGUI.EndChangeCheck())
        {
            data.LoadGrid();
        }
        if (data.grid == null || data.grid.GetLength(0) != data.width || data.grid.GetLength(1) != data.height)
        {
            data.LoadGrid();
        }
        
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();

        for (int y = 0; y < data.height; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < data.width; x++)
            {
                data.grid[x, y] = EditorGUILayout.IntField(data.grid[x, y], GUILayout.MaxWidth(40));
            }
            EditorGUILayout.EndHorizontal();
        }

        if (EditorGUI.EndChangeCheck())
        {
            data.SaveGrid();
            EditorUtility.SetDirty(data);
        }
    }
}