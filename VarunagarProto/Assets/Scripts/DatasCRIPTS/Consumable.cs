using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/ConsumableData", order = 3)]
public class Consumable : ScriptableObject
{
    [field: Header("Stats"), Space,SerializeField] 
    public int HealAdding { get; set; }
    [field: SerializeField]  
    public int DefAdding { get; set; }
    [field: SerializeField]  
    public int SpeedAdding { get; set; }
    
    [field: Header("Art"), Space,SerializeField] 
    public Sprite spriteRender;
    
    [field: Header("Custom"), Space,SerializeField]
    public string Name;

    public int IndexRef;
    
    [field: Header("Scripts"), Space,SerializeField]
    public GrabdableItem ItemParameter;
    
    [field: Header("Effects"), Space, SerializeField]
    private float m_stuntValue ;
    private int m_poisonStrength;
    private int m_poisonDuration ;
    private bool m_stuntProjectiles;
}