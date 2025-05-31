using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
public enum SpecialCapacityType
{
    None,
    DelayedAttack,
    UltMoine,
    UltPriso,
    UltGarde,

}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/CapacityData", order = 4)]
public class CapacityData : ScriptableObject
    //on appelle les comp�tences Cpt1a11 (1 : premier perso, a : premi�re cpt, 1 : affinity, 1 : Level)
{
    [Header("CptStats")]
    public int atk;
    [SerializeField] public int heal;
    [SerializeField] public int critique;
    [SerializeField] public int précision;
    [SerializeField] public int cooldown;
    [SerializeField] public int chargeUlti;

    [Header("Cpt Effect")]
    [SerializeField] public bool MultipleHeal;
    [SerializeField] public bool MultipleAttack;
    [SerializeField] public bool MultipleBuff;
    [SerializeField] public bool TargetingAlly;
    [SerializeField] public bool Provocation;

    [Header("Buff")]
    [SerializeField] public bool BuffFromAffinity;
    [SerializeField] public int buffType;
    [SerializeField] public float buffValue;
    [SerializeField] public int buffDuration;

    [Header("Affinity Effect")]
    [SerializeField] public int Shield;
    [SerializeField] public int ShieldRatioAtk;
    [SerializeField] public int Shock;
    [SerializeField] public int Necrosis;

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

    [Header("Text & Icons")]
    [SerializeField] public string Name;
    [SerializeField] public string Description;
    [SerializeField] public string AffinityDescription;
    [SerializeField] public int PictoType;
    [SerializeField] public int TargetType;

    [Header("Enemy Only")]
    [SerializeField] public int ValueAI;

    [Header("Second Effect")]
    [SerializeField] public bool DoubleEffect;
}