using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/DataEntity", order = 2)]
public class DataEntity : ScriptableObject
{
    
    public GameObject instance;
    [field: Header("Unit Stat"), SerializeField] 
    public int UnitLife { get; set; }

    public int BaseLife;
    public int UnitDef;
    public int UnitSpeed;
    public int UnitAtk;
    public int UnitShield;
    
    [field: Header("Art"), SerializeField] 
    public Sprite portrait;
    public Sprite portraitUI;
    public Sprite bandeauUI;
    public Animator manimator;

    [field: Header("Capacities"), SerializeField]
    public Sprite capacity1;

    public CapacityData _CapacityData1;
    public Sprite capacity2;
    public CapacityData _CapacityData2;
    public Sprite capacity3;
    public CapacityData _CapacityData3;
    public Sprite Ultimate;
    public CapacityData _CapacityDataUltimate;

    [field: Header("Custo"), SerializeField]
    public string namE;
    public float size;
}