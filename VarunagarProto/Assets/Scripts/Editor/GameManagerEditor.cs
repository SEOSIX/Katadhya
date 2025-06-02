using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GameManager gm = (GameManager)target;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isCombatEnabled"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("salleSpeciale"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerSpawnPoints"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("entityHandler"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerPrefabs"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("sizeChara"));
        if (gm.isCombatEnabled)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Enemy Section", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enemySpawnPoints"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EnemyPackIndex"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enemyPacks"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("allEnemiesEncountered"), true);
        }

        if (gm.salleSpeciale)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("objectsToSpawn"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ispressed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("uiCamera"));
        }
        serializedObject.ApplyModifiedProperties();
    }
}