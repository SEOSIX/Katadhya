using System.Security.Cryptography.X509Certificates;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
public enum SpecialCapacityType
{
    None,
    DelayedAttack,

}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/CapacityData", order = 4)]
public class CapacityData : ScriptableObject
    //on appelle les comp�tences Cpt1a1 (1 : premier perso, a : premi�re cpt, 0 : affinity)
{
    [field: Header("Cpt Stat"), SerializeField]
    public int atk;
    public int heal;
    public int buffType;
    public float buffValue;
    public int buffDuration;
    public int critique;
    public int précision;
    public int cooldown;
    public int chargeUlti;

    [field: Header("Cpt Effect"), SerializeField]
    public int Shield;
    public int Shock;
    public int ShieldRatioAtk;
    public bool MultipleHeal;
    public bool MultipleAttack;
    public bool TargetingAlly;
    public bool Provocation;

    [Header("Secondary Stats")]
    [HideInInspector] public int secondaryAtk;
    [HideInInspector] public int secondaryHeal;
    [HideInInspector] public int secondaryBuffType;
    [HideInInspector] public float secondaryBuffValue;
    [HideInInspector] public int secondaryBuffDuration;

    [Header("Special Capacity Settings")]
    [SerializeField] public SpecialCapacityType specialType = SpecialCapacityType.None;

    [Tooltip("Used if the special capacity needs a delay or multi-hit settings")]
    [SerializeField] public int specialDelay = 1;

    [field: Header("Text & icons"), SerializeField]
    public string Name;
    public string Description;
    public int PictoType;
    public int TargetType;

    [field: Header("Enemy Only"), SerializeField]
    public int ValueAI;

    [field: Header("Second Effect"), SerializeField]
    public bool DoubleEffect;
}