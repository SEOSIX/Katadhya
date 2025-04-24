using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/EntityHandler", order = 1)]
public class EntityHandler : ScriptableObject
{
    public List<DataEntity> ennemies;
    public List<DataEntity> players;
}