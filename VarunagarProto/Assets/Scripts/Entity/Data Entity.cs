using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/DataEntity", order = 2)]
public class DataEntity : ScriptableObject
{
    
    public GameObject instance;
    [field: Header("Unit Current Stat"), SerializeField]
    public int UnitLife { get; set; }
    public int UnitAtk;
    public int UnitSpeed;
    public int UnitDef;
    public int UnitAim;
    public List<ActiveBuff> ActiveBuffs;
    public List<CooldownData> ActiveCooldowns = new List<CooldownData>();
    public int Affinity;

    [field: Header("Unit Base Stat"), SerializeField]
    public int BaseLife;
    public int BaseAtk;
    public int BaseSpeed;
    public int BaseDef;
    public int BaseAim;

    [field: Header("Special Effect"), SerializeField]
    public int UnitShield;
    public int ShockMark;
    public int RageTick;
    [HideInInspector] public int LastRageTick;
    public List<Necrosis> necrosis = new List<Necrosis>();

    [field: Header("Other Information"), SerializeField]
    public bool beenHurtThisTurn;
    public bool provoking;

    [field: Header("Art"), SerializeField] 
    public Sprite portrait;
    public Sprite portraitUI;
    public Sprite bandeauUI;
    public GameObject Animator;

    [field: Header("Capacities"), SerializeField]
    public int Cpt1lvl;
    public Sprite capacity1;
    public CapacityData _CapacityData1;
    public int Cpt2lvl;
    public Sprite capacity2;
    public CapacityData _CapacityData2;
    public int Cpt3lvl;
    public Sprite capacity3;
    public CapacityData _CapacityData3;

    [field: Header("ultimate"), SerializeField]
    public int CptUltlvl;
    public Sprite Ultimate;
    public Sprite UltimateEmpty;
    public CapacityData _CapacityDataUltimate;
    [Range(0, 100)]
    public int UltimateSlider;
    public bool UltIsReady;
    public int UltLvl_1;
    public int UltLvl_2;
    public int UltLvl_3;
    public int UltLvl_4;




    [field: Header("Custom"), SerializeField]
    public string namE;
    public float size;
    public int index;
    
    
    public GameObject GameObject()
    {
        return instance;
    }

    [System.Serializable]
    public class ActiveBuff
    {
        public int type;
        public float value;
        public int duration;

        public ActiveBuff(int type, float value, int duration)
        {
            this.type = type;
            this.value = value;
            this.duration = duration;
        }
    }

    [System.Serializable]
    public struct CooldownData
    {
        public CapacityData capacity;
        public int remainingCooldown;

        public CooldownData(CapacityData capacity, int cooldown)
        {
            this.capacity = capacity;
            this.remainingCooldown = cooldown;
        }
    }
    public int RageAtkBonus = 0;

    public int GetEffectiveAtk()
    {
        return UnitAtk + RageAtkBonus;
    }

    [System.Serializable]
    public class Necrosis
    {
        public int level; // 1 à 5
        public int remainingTurns;

        public Necrosis(int level)
        {
            this.level = Mathf.Clamp(level, 1, 5);
            this.remainingTurns = 4;
            Debug.Log($"[NÉCROSE] Nouveau statut appliqué : niveau {this.level}, {this.remainingTurns} tours");
        }
        public bool IsExpired => remainingTurns <= 0;
    }

    public struct DelayedAction
    {
        public CapacityData capacity;
        public DataEntity target;

        public DelayedAction(CapacityData capacity, DataEntity target)
        {
            this.capacity = capacity;
            this.target = target;
        }
    }

    public bool skipNextTurn;
    public List<DelayedAction> delayedActions = new List<DelayedAction>();
}


