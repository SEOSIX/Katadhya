using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GameManager gm = (GameManager)target;

        DrawSafeProperty("isCombatEnabled");
        DrawSafeProperty("salleSpeciale");
        DrawSafeProperty("playerSpawnPoints", true);
        DrawSafeProperty("entityHandler");
        DrawSafeProperty("playerPrefabs", true);
        DrawSafeProperty("sizeChara");

        if (gm.isCombatEnabled)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Enemy Section", EditorStyles.boldLabel);
            DrawSafeProperty("enemySpawnPoints", true);
            DrawSafeProperty("EnemyPackIndex");
            DrawSafeProperty("enemyPacks", true);
            DrawSafeProperty("allEnemiesEncountered", true);
        }

        if (gm.salleSpeciale)
        {
            DrawSafeProperty("objectsToSpawn");
            DrawSafeProperty("ispressed");
            DrawSafeProperty("uiCamera");
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSafeProperty(string propertyName, bool includeChildren = false)
    {
        SerializedProperty prop = serializedObject.FindProperty(propertyName);
        if (prop != null)
        {
            EditorGUILayout.PropertyField(prop, includeChildren);
        }
        else
        {
            Debug.LogWarning($"Property '{propertyName}' not found on GameManager.");
        }
    }

}