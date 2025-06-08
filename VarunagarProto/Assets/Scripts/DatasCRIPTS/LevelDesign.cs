using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/LevelDesign", order = 6)]
public class LevelDesign : ScriptableObject
{
    public Combat[] CombatList;
}

[System.Serializable]
public class Combat
{
    public List<EnemyPack> WaveList = new List<EnemyPack>();
    public int NextCombatIndex;
    public int CaurisDor;
    public int CaurisSpe1;
    public int CaurisSpe2;
    public int CaurisSpe3;
    public int CaurisSpe4;

    public string[] RoomOptions = new string[2];
    public bool TwoRoomsInARow;
    public string[] SecondRoomOptions = new string[2];

}
