using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    private SerializedProperty isCombatEnabled;
    private SerializedProperty salleSpeciale;
    private SerializedProperty playerSpawnPoints;
    private SerializedProperty entityHandler;
    private SerializedProperty playerPrefabs;
    private SerializedProperty sizeChara;
    private SerializedProperty enemySpawnPoints;
    private SerializedProperty enemyPackIndex;
    private SerializedProperty allEnemiesEncountered;
    private SerializedProperty objectsToSpawn;
    private SerializedProperty ispressed;
    private SerializedProperty uiCamera;
    private SerializedProperty NewWavePanel;
    private SerializedProperty WaveText;
    private SerializedProperty EndScreen;


    private void OnEnable()
    {
        isCombatEnabled = serializedObject.FindProperty("isCombatEnabled");
        salleSpeciale = serializedObject.FindProperty("salleSpeciale");
        playerSpawnPoints = serializedObject.FindProperty("playerSpawnPoints");
        entityHandler = serializedObject.FindProperty("entityHandler");
        playerPrefabs = serializedObject.FindProperty("playerPrefabs");
        sizeChara = serializedObject.FindProperty("sizeChara");
        NewWavePanel = serializedObject.FindProperty("NewWavePanel");
        WaveText = serializedObject.FindProperty("WaveText");
        EndScreen = serializedObject.FindProperty("EndScreen");


        enemySpawnPoints = serializedObject.FindProperty("enemySpawnPoints");
        enemyPackIndex = serializedObject.FindProperty("EnemyPackIndex");
        allEnemiesEncountered = serializedObject.FindProperty("allEnemiesEncountered");

        objectsToSpawn = serializedObject.FindProperty("objectsToSpawn");
        ispressed = serializedObject.FindProperty("ispressed");
        uiCamera = serializedObject.FindProperty("uiCamera");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(isCombatEnabled);
        EditorGUILayout.PropertyField(salleSpeciale);
        EditorGUILayout.PropertyField(playerSpawnPoints, true);
        EditorGUILayout.PropertyField(entityHandler);
        EditorGUILayout.PropertyField(playerPrefabs, true);
        EditorGUILayout.PropertyField(sizeChara);


        if (isCombatEnabled.boolValue)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Enemy Section", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enemySpawnPoints, true);
            EditorGUILayout.PropertyField(enemyPackIndex);
            EditorGUILayout.PropertyField(allEnemiesEncountered, true);
            EditorGUILayout.PropertyField(NewWavePanel);
            EditorGUILayout.PropertyField(WaveText);
            EditorGUILayout.PropertyField(EndScreen);



        }

        if (salleSpeciale.boolValue)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Special Room Section", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(objectsToSpawn, true);
            EditorGUILayout.PropertyField(ispressed);
            EditorGUILayout.PropertyField(uiCamera);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
