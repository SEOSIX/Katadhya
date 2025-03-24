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
    [field: SerializeField]  public int UnitDef { get; set; }
    [field: SerializeField]  public int UnitSpeed { get; set; }
    
    [field: Header("Art"), SerializeField] 
    public Sprite portrait;
    public Sprite portraitUI;
    public Animator manimator;

    [field: Header("Capacities"), SerializeField]
    public Sprite capacity1;
    public Sprite capacity2;

    [field: Header("Custo"), SerializeField]
    public string namE;

    [field: Header("Line"), SerializeField]
    public float position;
}