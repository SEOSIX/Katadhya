using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/DataEntity", order = 2)]
public class DataEntity : ScriptableObject
{
    public GameObject instance;

    [SerializeField] public int UnitLife { get; set; }
    public int UnitAtk;
    public int UnitSpeed;
    public int UnitDef;
    public int UnitAim;
    public List<ActiveBuff> ActiveBuffs;
    public List<CooldownData> ActiveCooldowns = new List<CooldownData>();
    public int Affinity;
    public int ChargePower;
    public int UltChargePowerCost;

    [Header("Unit Base Stat")]
    public int BaseLife;
    public int BaseAtk;
    public int BaseSpeed;
    public int BaseDef;
    public int BaseAim;
    
    public int AtkLevel { get; set; } = 0;
    public int DefLevel { get; set; } = 0;
    public int SpeedLevel { get; set; } = 0;
    public int LifeLevel { get; set; } = 0;

    [Header("Special Effect")]
    public int UnitShield;
    public int ShockMark;
    public int RageTick;
    [HideInInspector] public int LastRageTick;
    public List<Necrosis> necrosis = new List<Necrosis>();

    [Header("Other Information")]
    public bool beenHurtThisTurn;
    public bool provoking;
    public int provokingDuration;
    [HideInInspector] public DataEntity provokingCaracter;

    [Header("Art")]
    public Sprite portrait;
    public Sprite portraitUI;
    public Sprite bandeauUI;
    public GameObject Animator;

    [Header("Capacities")]
    [SerializeField] private CapacityData capacityData1Source;
    public Sprite capacity1;
    public int Cpt1lvl;

    [SerializeField] private CapacityData capacityData2Source;
    public Sprite capacity2;
    public int Cpt2lvl;

    [SerializeField] private CapacityData capacityData3Source;
    public Sprite capacity3;
    public int Cpt3lvl;

    [SerializeField] private CapacityData capacityDataUltimateSource;
    public Sprite Ultimate;
    public Sprite UltimateEmpty;
    public int CptUltlvl;

    private CapacityData _capacityData1Instance;
    private CapacityData _capacityData2Instance;
    private CapacityData _capacityData3Instance;
    private CapacityData _capacityDataUltimateInstance;

    public CapacityData _CapacityData1
    {
        get
        {
            if (_capacityData1Instance == null && capacityData1Source != null)
                _capacityData1Instance = capacityData1Source;
            return _capacityData1Instance;
        }
        set
        {
            _capacityData1Instance = value;
        }
    }

    public CapacityData _CapacityData2
    {
        get
        {
            if (_capacityData2Instance == null && capacityData2Source != null)
                _capacityData2Instance = capacityData2Source;
            return _capacityData2Instance;
        }
        set
        {
            _capacityData2Instance = value;
        }
    }

    public CapacityData _CapacityData3
    {
        get
        {
            if (_capacityData3Instance == null && capacityData3Source != null)
                _capacityData3Instance = capacityData3Source;
            return _capacityData3Instance;
        }
        set
        {
            _capacityData3Instance = value;
        }
    }

    public CapacityData _CapacityDataUltimate
    {
        get
        {
            if (_capacityDataUltimateInstance == null && capacityDataUltimateSource != null)
                _capacityDataUltimateInstance = capacityDataUltimateSource;
            return _capacityDataUltimateInstance;
        }
        set
        {
            _capacityDataUltimateInstance = value;
        }
    }


    [Header("Ultimate")]
    [Range(0, 100)]
    public int UltimateSlider;
    public bool UltIsReady;
    public int UltLvl_1;
    public int UltLvl_2;
    public int UltLvl_3;
    public int UltLvl_4;

    [Header("Ultimate Pas Touche"), HideInInspector]
    public int UltLvlHit;

    [Header("Custom")]
    [SerializeField] public string namE;
    public float size;
    public int index;
    public int ID;

    public GameObject GameObject()
    {
        return instance;
    }

    [Serializable]
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

    [Serializable]
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

    [Serializable]
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

    [Serializable]
    public class DelayedAction
    {
        public CapacityData capacity;
        [NonSerialized] public DataEntity target;

        public DelayedAction(CapacityData capacity, DataEntity target)
        {
            this.capacity = capacity;
            this.target = target;
        }
    }

    public bool skipNextTurn;
    public List<DelayedAction> delayedActions = new List<DelayedAction>();
}
