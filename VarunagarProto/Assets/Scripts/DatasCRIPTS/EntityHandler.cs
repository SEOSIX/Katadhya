using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/EntityHandler", order = 1)]
public class EntityHandler : ScriptableObject
{
    public DataEntity[] players;
    
    public DataEntity[] ennemies;
}