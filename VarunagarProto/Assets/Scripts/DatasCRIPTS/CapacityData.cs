using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/CapacityData", order = 4)]
public class CapacityData : ScriptableObject
    //on appelle les comp�tences Cpt1a1 (1 : premier perso, a : premi�re cpt, 1 : premier niveau de la cpt)
{
    [field: Header("Cpt Stat"), SerializeField]
    public int atk;
    public int heal;
    public int buffType;
    public int buffValue;
    public int buffTime;

    [field: Header("Cpt Effect"), SerializeField]
    public int Shield;
    public int Electrik;
    public bool CanHeal;
}