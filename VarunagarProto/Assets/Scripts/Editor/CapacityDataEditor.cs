using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CapacityData))]
public class CapacityDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Récupérer l'objet cible (CapacityData)
        var capacityData = (CapacityData)target;

        // Dessiner tous les champs de base par défaut (comme vous l'avez normalement dans l'éditeur)
        DrawDefaultInspector();

        // Appliquer la logique pour DoubleEffect
        if (capacityData.DoubleEffect)
        {
            capacityData.secondaryAtk = EditorGUILayout.IntField("Secondary Attack", capacityData.secondaryAtk);
            capacityData.secondaryHeal = EditorGUILayout.IntField("Secondary Heal", capacityData.secondaryHeal);
            capacityData.secondaryBuffType = EditorGUILayout.IntField("Secondary Buff Type", capacityData.secondaryBuffType);
            capacityData.secondaryBuffValue = EditorGUILayout.FloatField("Secondary Buff Value", capacityData.secondaryBuffValue);
            capacityData.secondaryBuffDuration = EditorGUILayout.IntField("Secondary Buff Duration", capacityData.secondaryBuffDuration);
        }

        // Forcer Unity à rafraîchir l'éditeur si des changements ont été faits
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
