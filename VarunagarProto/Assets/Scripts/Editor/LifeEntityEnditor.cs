using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LifeEntity))]
public class LifeEntityEditor : Editor
{
    SerializedProperty isCombatEnabledProp;
    SerializedProperty playerSlidersProp;
    SerializedProperty playerShieldSlidersProp;
    SerializedProperty playerPVTextsProp;
    SerializedProperty healingPlayers;
    SerializedProperty GlobalHealingAmount;
    SerializedProperty entityHandlerProp;

    SerializedProperty enemySlidersProp;
    SerializedProperty enemyShieldSlidersProp;
    SerializedProperty enemyPVTextsProp;

    void OnEnable()
    {
        isCombatEnabledProp = serializedObject.FindProperty("isCombatEnabled");
        playerSlidersProp = serializedObject.FindProperty("PlayerSliders");
        playerShieldSlidersProp = serializedObject.FindProperty("PlayerShieldSliders");
        playerPVTextsProp = serializedObject.FindProperty("PlayerPVTexts");
        healingPlayers = serializedObject.FindProperty("healingPlayers");
        GlobalHealingAmount = serializedObject.FindProperty("GlobalHealingAmount");
        entityHandlerProp = serializedObject.FindProperty("entityHandler");

        enemySlidersProp = serializedObject.FindProperty("enemySliders");
        enemyShieldSlidersProp = serializedObject.FindProperty("enemyShieldSliders");
        enemyPVTextsProp = serializedObject.FindProperty("enemyPVTexts");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(isCombatEnabledProp);

        EditorGUILayout.PropertyField(playerSlidersProp, true);
        EditorGUILayout.PropertyField(playerShieldSlidersProp, true);
        EditorGUILayout.PropertyField(healingPlayers);
        EditorGUILayout.PropertyField(GlobalHealingAmount);
        EditorGUILayout.PropertyField(playerPVTextsProp, true);
        EditorGUILayout.PropertyField(entityHandlerProp, true);
        bool allSame = true;
        bool firstValue = ((LifeEntity)targets[0]).isCombatEnabled;
        foreach (var obj in targets)
        {
            if (((LifeEntity)obj).isCombatEnabled != firstValue)
            {
                allSame = false;
                break;
            }
        }
        if (allSame && firstValue)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Enemy Section", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enemySlidersProp, true);
            EditorGUILayout.PropertyField(enemyShieldSlidersProp, true);
            EditorGUILayout.PropertyField(enemyPVTextsProp, true);
        }
        else if (!allSame)
        {
            EditorGUILayout.HelpBox("Les objets sélectionnés ont des valeurs différentes pour 'isCombatEnabled'. Impossible d'afficher la section ennemis.", MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
