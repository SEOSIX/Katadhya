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
    public List<ActiveBuff> ActiveBuffs;
    public List<CooldownData> ActiveCooldowns = new List<CooldownData>();
    public int Affinity;

    [field: Header("Unit Base Stat"), SerializeField]
    public int BaseLife;
    public int BaseAtk;
    public int BaseSpeed;
    public int BaseDef;

    [field: Header("Special Effect"), SerializeField]
    public int UnitShield;
    public int ShockMark;
    public int SkipPlay;

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
    
    [field: Header("ultimate"), SerializeField]
    public Sprite Ultimate;
    public Sprite UltimateEmpty;
    public CapacityData _CapacityDataUltimate;
    [Range(0, 100)]
    public int UltimateSlider;
    public bool UltIsReady;
    
    [field: Header("Custo"), SerializeField]
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


