using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/GlobalGameData", order = 5)]
public class GlobalGameData : ScriptableObject
{
    public int CurrentCombat;   
    public GameObject[] BackgroundPrefabs;

    public void LoadBackground(Transform BackgroundParent, int index)
    {
        if (BackgroundParent.GetChild(0))
        {
            Destroy(BackgroundParent.GetChild(0).gameObject);
        }
        Instantiate(BackgroundPrefabs[index],BackgroundParent);
    }
}
