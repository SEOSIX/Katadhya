using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static DataEntity;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEditor.ShaderKeywordFilter;
using Random = UnityEngine.Random;
using static UnityEditor.Experimental.GraphView.Port;
using static UnityEngine.GraphicsBuffer;
using Unity.Collections;

public class CombatManager : MonoBehaviour
{
    public static CombatManager SINGLETON { get; private set; }

    [Header("Entity Handler")]
    [SerializeField]
    public EntityHandler entityHandler;
    public EntiityManager entiityManager;

    [Header("Entity UI")]
    public TextMeshProUGUI textplayer;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI atck;
    public TextMeshProUGUI def;
    public TextMeshProUGUI lifeText;
    public Image playerPortrait;
    public Image[] ImagePortrait;
    public Button[] capacityButtons;
    public Button[] capacityAnimButtons;
    public GameObject[] capacityPage;
    public GameObject[] capacityPageAffinity;
    public GameObject[] Banderoles;
    public TextMeshProUGUI[] CoolDownTexts;
    public Sprite[] Pictos;
    public Sprite[] PictosAffinity;
    public Sprite[] TargetType;
    public Sprite[] PictoBuffs;
    public Material GreyScale;
    private DataEntity currentPlayer;
    public GameObject currentInterractingButton;
    public GameObject currentSelectedButton;



    [Header("Turn Management")]
    public Button endTurnButton;
    public Slider PlayerCharge;

    [Header("TurnObject")]
    public GameObject ennemyTurn;
    public GameObject TurnUI;
    public TextMeshProUGUI attackEnnemy;
    public CanvasGroup canvasGroup;
    public RectTransform circleParentUI;

    [Header("Turn Indicators")]
    public GameObject[] playerTurnIndicators;

    [Header("Target Indicators")]
    public List<GameObject> circlesEnnemy;
    public GameObject circlePrefab;
    [HideInInspector]public List<Vector3> originalCircleEnemysPositions = new List<Vector3>();
    public List<GameObject> circlesPlayer;
    [HideInInspector]public List<Vector3> originalCirclePlayersPositions = new List<Vector3>();
    
    public Sprite hoverSprite;
    public Sprite defaultSprite;


    [Header("Ultimate")]
    public Ultimate ultimateScript;



    [HideInInspector] public List<DataEntity> currentTurnOrder = new List<DataEntity>();
    [HideInInspector] public List<DataEntity> unitPlayedThisTurn = new List<DataEntity>();
    
    private Dictionary<DataEntity, int> visualIndexes = new Dictionary<DataEntity, int>();
    
    private System.Random r = new System.Random();
    [HideInInspector] public bool PlayerClickable;
    [HideInInspector] public bool EnemyClickable;

    public static class GlobalVars
    {
        public static CapacityData currentSelectedCapacity;
        public static DataEntity currentPlayer;
    }
    public bool isEnnemyTurn;
    private Vector2[] targetSizes;
    

    void Awake()
    {
        if (SINGLETON != null)
        {
            Destroy(SINGLETON.gameObject);
            return;
        }
        SINGLETON = this;
    }

    
    [ContextMenu("pipi")]
    void Start()
    {
        ReseterData.ResetPlayersComplete(entityHandler, entiityManager);  //a retirer avant de build
        ReseterData.ResetEnemies(entityHandler);
        ReseterData.ResetPlayersBeforeCombat(entityHandler, entiityManager);
        currentTurnOrder = GetUnitTurn();
        StartUnitTurn();
    }

    void Update()
    {
        InitializeStaticUI();
        
    }
    public void InitializeStaticUI()
    {
        if (currentTurnOrder == null || currentTurnOrder.Count == 0) return;

        DataEntity currentEntity = currentTurnOrder[0];

        textplayer.text = currentEntity.name;
        speed.text = "VIT :" + currentEntity.UnitSpeed;
        def.text = "DEF :" + currentEntity.UnitDef;
        atck.text = "ATQ :" + currentEntity.UnitAtk;
        lifeText.text = currentEntity.UnitLife + "/" + currentEntity.BaseLife;
        playerPortrait.sprite = currentEntity.bandeauUI;
        PlayerCharge.value = currentEntity.ChargePower;

        List<DataEntity> initialTurnOrder = GetUnitTurn();
        for (int i = 0; i < ImagePortrait.Length; i++)
        {
            if (i < initialTurnOrder.Count && initialTurnOrder[i].UnitLife > 0)
            {
                ImagePortrait[i].enabled = true;
                ImagePortrait[i].sprite = initialTurnOrder[i].portraitUI;
                targetSizes = new Vector2[ImagePortrait.Length];
                Color PortraitColor = Color.white;
                if (initialTurnOrder[i] == currentEntity)
                {
                    targetSizes[i] = new Vector2(98.67f, 122.36f);
                }
                else
                {
                    targetSizes[i] = new Vector2(75, 93);
                    PortraitColor = new Color(225f/255f, 225f/255f, 225f/255f);
                }
                if (ImagePortrait[i] != null && ImagePortrait[i].enabled)
                {
                    ImagePortrait[i].rectTransform.sizeDelta = Vector2.Lerp(
                        ImagePortrait[i].rectTransform.sizeDelta,
                        targetSizes[i],
                        Time.deltaTime * 10f
                    );
                    ImagePortrait[i].color = PortraitColor;
                }
            }
            else
            {
                ImagePortrait[i].enabled = false;
            }
        }
    }

    #region TurnManagement
    public List<DataEntity> GetUnitTurn()
    {
        var speedValue = new List<DataEntity>();
        speedValue.AddRange(entityHandler.ennemies.Where(e => e != null && e.UnitLife > 0));
        speedValue.AddRange(entityHandler.players.Where(p => p != null && p.UnitLife > 0));
        return speedValue.OrderByDescending(x => x.UnitSpeed).ToList();
    }

    public void EndUnitTurn()
    {
        if (currentTurnOrder.Count == 0) return;

        DataEntity currentCombatData = currentTurnOrder[0];
        currentTurnOrder.RemoveAt(0);
        unitPlayedThisTurn.Add(currentCombatData);

        if (currentTurnOrder.Count == 0)
        {
            EndGlobalTurn();
        }
        StartCoroutine(StartUnitTurnDelayed());
    }
    
    private IEnumerator StartUnitTurnDelayed()
    {
        yield return new WaitForEndOfFrame(); 
        StartUnitTurn();
    }

    public void EndGlobalTurn()
    {
        foreach (var unit in unitPlayedThisTurn)
        {
            if (!currentTurnOrder.Contains(unit))
            {
                currentTurnOrder.Add(unit);
            }
        }

        unitPlayedThisTurn.Clear();
        currentTurnOrder = currentTurnOrder.OrderByDescending(x => x.UnitSpeed).ToList();
    }

    public void StartUnitTurn()
    {
        DataEntity caster = currentTurnOrder[0];
        ChargePower(caster, 2);
        if (caster.beenHurtThisTurn == false && caster.RageTick > 0)
        {
            caster.RageTick -= 1;
        }
        if (caster.necrosis != null && caster.necrosis.Count > 0)
        {
            TickNecrosisEffect(caster);
        }
        if (caster.provokingCaracter != null)
        {
            if (caster.provokingCaracter.provoking == true)
            {
                caster.provokingCaracter.provokingDuration -= 1;
                if (caster.provokingCaracter.provokingDuration <= 0)
                {
                    caster.provokingCaracter.provoking = false;
                }
            } 
        }
        caster.beenHurtThisTurn = false;
        RecalculateStats(caster);
        StartCoroutine(StartUnitTurnRoutine());
    }
    
    private IEnumerator StartUnitTurnRoutine()
    {
        while (currentTurnOrder.Count > 0 && currentTurnOrder[0].UnitLife <= 0)
        {
            currentTurnOrder.RemoveAt(0);
            yield return null;
        }

        if (currentTurnOrder.Count == 0) yield break;

        DataEntity current = currentTurnOrder[0];
        GlobalVars.currentPlayer = current;


        if (current.skipNextTurn)
        {
            yield return new WaitForSeconds(0.5f);
            current.skipNextTurn = false;
            ExecuteDelayedActions(current);
            EndUnitTurn();
            yield break;
        }

        DetectEnnemyTurn();
    
        if (entityHandler.ennemies.Contains(current))
        {
            yield return StartCoroutine(DelayedEnemyTurn(current));
            yield break;
        }

        currentPlayer = current;
        InitializeStaticUI();
        SetupCapacityButtons(currentPlayer);

        foreach (var indicator in playerTurnIndicators)
            indicator.SetActive(false);

        if (entityHandler.players.Contains(currentPlayer))
        {
            int index = entityHandler.players.IndexOf(currentPlayer);
            if (index < playerTurnIndicators.Length)
                playerTurnIndicators[index].SetActive(true);
        }
    }


    public void RemoveUnitFromList(DataEntity _currentUnit)
    {
        if (_currentUnit == null || !currentTurnOrder.Contains(_currentUnit)) return;
        currentTurnOrder.Remove(_currentUnit);
    }

    public void DetectEnnemyTurn()
    {
        DataEntity currentEntity = currentTurnOrder[0];
        isEnnemyTurn = entityHandler.ennemies.Contains(currentEntity);

        if (isEnnemyTurn)
        {
            foreach (var indicator in playerTurnIndicators)
            {
                indicator.SetActive(false);
            }
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            ennemyTurn.SetActive(true);
        }
        else
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            ennemyTurn.SetActive(false);
        }
    }

    private IEnumerator DelayedEnemyTurn(DataEntity currentEntity)
    {
        yield return new WaitForSeconds(1f);
        AI.SINGLETON.Attack(currentEntity, 50);
    }

    #endregion
    void HideTargetIndicators()
    {
        foreach (var circle in circlesEnnemy)
        {
            circle.SetActive(false);
        }
        foreach (var circle in circlesPlayer)
            circle.SetActive(false);
    }
    public void UseCapacity(CapacityData cpt)
    {
        DataEntity player = currentTurnOrder[0];
        Debug.Log(cpt);
        char CptType = cpt.name[4];
        if(CptType == 'd')
        {
            int CptChargeCost = player.UltChargePowerCost;
            if (player.ChargePower >= CptChargeCost)
            {
                player.ChargePower -= CptChargeCost;

                StartTargetSelectionMode(cpt);
            }
            else
            {
                Debug.Log("Pas assez de charge pour Ult");
            }
        }
        else
        {
            int CptLevel = cpt.name[6] - '0';
            int CptChargeCost = 0;
            switch (CptLevel)
            {
                case 0: CptChargeCost = 0; break;
                case 1: CptChargeCost = 2; break;
                case 2: CptChargeCost = 3; break;
            }
            if (player.ChargePower >= CptChargeCost)
            {
                player.ChargePower -= CptChargeCost;

                StartTargetSelectionMode(cpt);
            }
            else Debug.Log("pas assez charge");
        }

    }
    #region Selection
    public void StartTargetSelectionMode(CapacityData capacity)
    {
        DataEntity caster = currentTurnOrder[0]; 
        HideTargetIndicators();
        GlobalVars.currentSelectedCapacity = capacity;
        if (capacity.MultipleHeal)
        {
            List<DataEntity> pool = null;
            if (entityHandler.players.Contains(currentPlayer))
            {
                pool = entityHandler.players;
            }
            else
            {
                pool = entityHandler.ennemies;
            }
            foreach (var target in pool)
            {
                if (target.UnitLife > 0)
                {
                    ApplyCapacityToTarget(capacity, target);
                }
            }
            DecrementBuffDurations(caster);
            DecrementCooldowns(caster);
            GlobalVars.currentSelectedCapacity = null;
            Debug.Log("Capacité de soin appliquée à tous les alliés !");
            EndUnitTurn();
            return;
        }

        if (capacity.MultipleAttack)
        {
            List<DataEntity> pool = null;
            if (entityHandler.players.Contains(currentPlayer))
            {
                pool = entityHandler.ennemies;
            }
            else
            {
                pool = entityHandler.players;
            }
            foreach (var target in pool)
            {
                if (target.UnitLife > 0)
                {
                    ApplyCapacityToTarget(capacity, target);
                }
            }
            DecrementBuffDurations(currentTurnOrder[0]);
            DecrementCooldowns(currentTurnOrder[0]);
            GlobalVars.currentSelectedCapacity = null;
            Debug.Log("Capacité de zone appliquée à tous les ennemis !");
            EndUnitTurn();
            return;
        }
        if (capacity.MultipleBuff)
        {
            List<DataEntity> pool = null;
            if (entityHandler.players.Contains(currentPlayer))
            {
                pool = entityHandler.players;
            }
            else
            {
                pool = entityHandler.ennemies;
            }
            foreach (var target in pool)
            {
                if (target.UnitLife > 0)
                {
                    ApplyCapacityToTarget(capacity, target);
                }
            }
            DecrementBuffDurations(currentTurnOrder[0]);
            DecrementCooldowns(currentTurnOrder[0]);
            GlobalVars.currentSelectedCapacity = null;
            Debug.Log("Buff de zone appliquée à tous les alliés !");
            EndUnitTurn();
            return;
        }
        ShowTargetIndicators(capacity);
        DecrementBuffDurations(caster);
        DecrementCooldowns(caster);
        
        RecalculateStats(caster);
    }

    public void SelectEnemy(int enemyIndex)
    {
        if (enemyIndex == 3 && entityHandler.ennemies.Count==1)
        {
            enemyIndex = 0;
        }
        else
        {
            enemyIndex -= entityHandler.players.Count;
        }
        if (enemyIndex < 0 || enemyIndex >= entityHandler.ennemies.Count)
        {
            return;
        }

        DataEntity target = entityHandler.ennemies[enemyIndex];
        
        if (target == null || target.UnitLife <= 0)
        {
            Debug.LogError("Cible invalide ou morte.");
            return;
        }
        ApplyCapacityToTarget(GlobalVars.currentSelectedCapacity, target);
        GlobalVars.currentSelectedCapacity = null;
        HideTargetIndicators();
        EndUnitTurn();
    }

    public void SelectAlly(int allyIndex)
    {
        if (allyIndex < 0 || allyIndex >= entityHandler.players.Count)
        {
            Debug.LogError("Index d'allié invalide !");
            return;
        }
        DataEntity target = entityHandler.players[allyIndex];
        ApplyCapacityToTarget(GlobalVars.currentSelectedCapacity, target);
        GlobalVars.currentSelectedCapacity = null;
        EndUnitTurn();
        HideTargetIndicators();
    }
    #endregion

    #region ApplyCapacity
    public void ApplyCapacityToTarget(CapacityData capacity, DataEntity target)
    {

        DataEntity caster = currentTurnOrder[0];

        Animator anim = caster.instance?.GetComponent<Animator>();
        if (anim != null && anim.runtimeAnimatorController != null)
        {

            anim.SetTrigger("Attack");
        }

        if (capacity.specialType != SpecialCapacityType.None)
        {
            ApplySpecialCapacity(capacity, caster, target);
        }
        else
        {
            if (capacity.DoubleEffect && entityHandler.ennemies.Contains(target))
            {
                ApplySecondaryCapacity(capacity, caster, target);
            }
            else
            {
                ApplyNormalCapacity(capacity, caster, target);
            }
        }

        if (capacity.cooldown > 0)
        {
            bool alreadyInCooldown = caster.ActiveCooldowns.Exists(cd => cd.capacity == capacity);

            if (!alreadyInCooldown)
            {
                caster.ActiveCooldowns.Add(new CooldownData(capacity, capacity.cooldown));
                Debug.Log($"[Cooldown] Ajouté à {caster.name} pour capacité {capacity.name}, durée {capacity.cooldown}");
            }
        }
        /*if (Ultimate.SINGLETON != null)
        {
            Ultimate.SINGLETON.GainUltimateCharge(capacity.chargeUlti);
        }*/
        InitializeStaticUI();

    }

    private IEnumerator MoveTowardsTarget(DataEntity caster, DataEntity target, float distance = 1.0f, float speed = 5f)
    {
        if (caster.instance == null || target.instance == null)
             yield break;

         Transform casterTransform = caster.instance.transform;
            Vector3 start = casterTransform.position;
            Vector3 direction = (target.instance.transform.position - start).normalized;
            Vector3 end = start + direction * distance;

            float t = 0;
            while (t < 1f)
            {
                   t += Time.deltaTime * speed;
                   casterTransform.position = Vector3.Lerp(start, end, t);
                  yield return null;
            }

            yield return new WaitForSeconds(0.1f);
            t = 0;
            while (t < 1f)
            {
                 t += Time.deltaTime * speed;
                casterTransform.position = Vector3.Lerp(end, start, t);
                yield return null;
            }
    }



    public void ApplyNormalCapacity(CapacityData capacity, DataEntity caster, DataEntity target, float UltMoine = 0, float UltPriso = 0, float UltGarde = 0)
    {
        int DamageDone = 0;
        int visualIndex = entityHandler.players.Contains(target)
            ? entityHandler.players.IndexOf(target)
            : entityHandler.players.Count + entityHandler.ennemies.IndexOf(target);
        float globalAim = Mathf.RoundToInt(capacity.précision * (caster.UnitAim/100));
        float réussite = lancer(globalAim, 2f, 1f);

        if (entityHandler.players.Contains(caster))
        {
            int PlayerIndex = caster.ID -3;
            int CapacityIndex = -1;
            switch (capacity.ToString()[4])
            {
                case 'a': 
                    CapacityIndex = 0;
                    break;

                case 'b': 
                    CapacityIndex = 1;
                    break;

                case 'c':
                    CapacityIndex = 2;
                    break;

                case 'd':
                    CapacityIndex = 3;
                    break;
                    
            }
            SoundPackage pack = EffectsManager.SINGLETON.PlayerCptSounds[PlayerIndex];
            List<AudioClip> PlayerClips = new List<AudioClip>() { pack.Cpt1, pack.Cpt2, pack.Cpt3, pack.Cpt4 };
            if (CapacityIndex!= -1 && PlayerClips[CapacityIndex]!= null)
            {
                Coroutine play = StartCoroutine(AudioManager.SINGLETON.PlayClip(PlayerClips[CapacityIndex]));
            }
        }

        if (réussite == 2)
        {
            Debug.Log("Échec de la compétence");
            return;
        }
        float modifier = lancer(capacity.critique, 1f, 1.5f);
        if (capacity.atk > 0 && caster.instance != null && target.instance != null)
        {
            //StartCoroutine(MoveTowardsTarget(caster, target, 0.5f, 5f)); 
        }

        // ATTAQUE
        if (capacity.atk > 0)
        {
            DamageDone = ApplyDamage(capacity, caster, target, modifier);
        }
        if (capacity.heal > 0)
        {
            StartCoroutine(AudioManager.SINGLETON.PlayCombatClip(8));

            float BonusRageHeal = 0;
            if (caster.Affinity == 4) BonusRageHeal = GetBonusRageDamage(caster);
            int healAmount = Mathf.RoundToInt((Mathf.Sqrt (2*caster.UnitAtk) + capacity.heal+ UltMoine) * modifier + BonusRageHeal);
            target.UnitLife = Mathf.Min(target.UnitLife + healAmount, target.BaseLife);

            EffectsManager.SINGLETON.AfficherHeal(target, healAmount);
        }
        if (capacity.Shield > 0)
        {
            target.UnitShield += Mathf.RoundToInt(capacity.Shield * modifier);
        }
        if (capacity.Provocation == true)
        {
            target.provoking = true;
            target.provokingDuration = capacity.ProvocationDuration;
            caster.provokingCaracter = target;
        }
        if (capacity.buffType > 0)
        {
            GiveBuff(capacity, target, UltGarde);
            EffectsManager.SINGLETON.AfficherPictoBuff(visualIndex,capacity, target);

        }
        if (capacity.ShieldRatioAtk > 0)
        {
            int Shielding = Mathf.RoundToInt(((float)capacity.ShieldRatioAtk / 100) * DamageDone);
            caster.UnitShield += Shielding;
        }
        if (target.Affinity == 4 && capacity.atk > 0)
        {
            StartCoroutine(AudioManager.SINGLETON.PlayCombatClip(5));
            ApplyRage(target);
        }

        if (capacity.Necrosis > 0)
        {
            StartCoroutine(AudioManager.SINGLETON.PlayCombatClip(4));
            ApplyNecrosis(target, capacity.Necrosis);
        }
    }


    private void ApplySpecialCapacity(CapacityData capacity, DataEntity caster, DataEntity target)
    {
        switch (capacity.specialType)
        {
            case SpecialCapacityType.DelayedAttack:
                if (target.UnitLife != 0)
                {
                    caster.skipNextTurn = true;
                    caster.delayedActions.Add(new DelayedAction(capacity, target));
                    Debug.Log($"{caster.namE} prépare une attaque différée !");
                    DecrementBuffDurations(caster);
                    DecrementCooldowns(caster);
                }
                else
                {
                    Debug.Log("Cpt annulé car target est mort");
                    return;
                }
                break;
            case SpecialCapacityType.UltMoine:
                caster.CptUltlvl = caster.UltLvl_1 + caster.UltLvl_2 + caster.UltLvl_3 + caster.UltLvl_4;
                ApplyNormalCapacity(capacity, caster, target, 2 * caster.CptUltlvl);
                break;
            case SpecialCapacityType.UltPriso:
                caster.CptUltlvl = caster.UltLvl_1 + caster.UltLvl_2 + caster.UltLvl_3 + caster.UltLvl_4;
                ApplyNormalCapacity(capacity, caster, target, 0, 2*caster.CptUltlvl);
                break;
            case SpecialCapacityType.UltGarde:
                caster.CptUltlvl = caster.UltLvl_1 + caster.UltLvl_2 + caster.UltLvl_3 + caster.UltLvl_4;
                ApplyNormalCapacity(capacity, caster, target, 0, 0, 0.05f*caster.CptUltlvl);
                break;

            default:
                Debug.LogWarning("Special capacity type not handled.");
                break;
        }
    }
  public void ApplySecondaryCapacity(CapacityData capacity, DataEntity caster, DataEntity target)
    {
        int DamageDone = 0;
        int visualIndex = entityHandler.players.Contains(target)
        ? entityHandler.players.IndexOf(target)
        : entityHandler.players.Count + entityHandler.ennemies.IndexOf(target);
        float globalAim = Mathf.RoundToInt(capacity.précision * (caster.UnitAim / 100));
        float réussite = lancer(globalAim, 2f, 1f);

        if (réussite == 2)
        {
            Debug.Log("Échec de la compétence");
            return;
        }
        float modifier = lancer(capacity.critique, 1f, 1.5f);

        if (capacity.atk > 0 && caster.instance != null && target.instance != null)
        {
            //StartCoroutine(MoveTowardsTarget(caster, target, 0.5f, 5f));
        }

            // ATTAQUE
        if (capacity.atk > 0)
        {
            ApplyDamage(capacity, caster, target, modifier);
        }

        if (capacity.secondaryHeal > 0)
        {
            int healAmount = Mathf.RoundToInt((((caster.UnitAtk) + capacity.secondaryHeal) / 2) * modifier);
            target.UnitLife = Mathf.Min(target.UnitLife + healAmount, target.BaseLife);
        }

        if (capacity.Shield > 0)
        {
            target.UnitShield += Mathf.RoundToInt(capacity.Shield * modifier);
        }

        if (capacity.Provocation == true)
        {
            target.provoking = true;
        }

        if (capacity.secondaryBuffType > 0)
        {
            GiveBuff(capacity, target);
            EffectsManager.SINGLETON.AfficherPictoBuff(visualIndex,capacity, target);
        }

        if (capacity.ShieldRatioAtk > 0)
        {
            int Shielding = Mathf.RoundToInt(((float)capacity.ShieldRatioAtk / 100) * DamageDone);
            caster.UnitShield += Shielding;
        }

        if (target.Affinity == 4 && capacity.atk > 0)
        {
            StartCoroutine(AudioManager.SINGLETON.PlayCombatClip(5));
            ApplyRage(target);
        }

        if (capacity.Necrosis > 0)
        {
            StartCoroutine(AudioManager.SINGLETON.PlayCombatClip(4));
            ApplyNecrosis(target, capacity.Necrosis);
        }
    }
    public int ApplyDamage(CapacityData capacity, DataEntity caster, DataEntity target, float modifier, float UltMoine = 0, float UltPriso = 0, float UltGarde = 0, int DamageDone = 0)
    {
        int visualIndex = entityHandler.players.Contains(target)
        ? entityHandler.players.IndexOf(target)
        : entityHandler.players.Count + entityHandler.ennemies.IndexOf(target);
        Animator anim = target.instance?.GetComponent<Animator>();
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            anim.SetTrigger("TakeDamage");
        }
        else
        {
            Debug.Log("PB Animator");
        }
        bool enemyHasShield = false;
        if (target.UnitShield > 0) enemyHasShield = true;
        float BonusRageDamage = 0;
        if (caster.Affinity == 4) BonusRageDamage = GetBonusRageDamage(caster);
        float calculatedDamage = ((capacity.atk + UltPriso) * (caster.UnitAtk + 20) / (target.UnitDef + 20)) * modifier + BonusRageDamage;
        if (caster.RageTick >= 12) caster.RageTick = 0;
        EffectsManager.SINGLETON.AfficherRageSlider(target.RageTick, visualIndex);
        int icalculatedDamage = Mathf.RoundToInt(calculatedDamage);
        DamageDone += icalculatedDamage;

        if (target.UnitShield > 0)
        {
            if (target.UnitShield < icalculatedDamage)
            {
                icalculatedDamage -= target.UnitShield;
                target.UnitShield = 0;
            }
            else
            {
                target.UnitShield -= icalculatedDamage;
                icalculatedDamage = 0;
            }
            StartCoroutine(AudioManager.SINGLETON.PlayCombatClip(1));

        }
        else
        {
            StartCoroutine(AudioManager.SINGLETON.PlayCombatClip(0));
        }

        if (icalculatedDamage > 0)
        {
            target.UnitLife -= icalculatedDamage;
        }
        target.beenHurtThisTurn = true;

        EffectsManager.SINGLETON.AfficherAttaqueSimple(target, icalculatedDamage, modifier);
        if (capacity.Shock > 0)
        {
            ShockProc(capacity, target, enemyHasShield);
        }

        return DamageDone;
    }

    #endregion

    #region PowerCharge
    public void ChargePower(DataEntity player,int amount)
    {
        player.ChargePower = Mathf.Clamp(player.ChargePower + amount, 0, 10); ;
    }
    public string CycleCapacityName(string currentName, int maxLevel = 2)
    {
        if (string.IsNullOrEmpty(currentName) || currentName.Length == 0)
            return currentName;

        char lastChar = currentName[^1];

        int level = lastChar - '0';
        int nextLevel = (level + 1) % (maxLevel + 1);

        return currentName.Substring(0, currentName.Length - 1) + nextLevel;
    }
    public CapacityData GetCycledCapacity(CapacityData currentCapacity, int maxLevel = 2)
    {
        string currentName = currentCapacity.name;
        if (currentName.Length > 7)
        {
            currentName = currentName.Remove(7);
        }

        char playerId = currentName[3];
        char affinityId = currentName[5];
        char levelChar = currentName[6];

        if (!char.IsDigit(levelChar)) return currentCapacity;
        int level = levelChar - '0';
        int nextLevel = (level + 1) % (maxLevel + 1);
        string newName = currentName.Substring(0, currentName.Length - 1) + nextLevel;

        CapacityData[] CptArray = Resources.LoadAll<CapacityData>("Data/Entity/Capacity");
        CapacityData newCapacity = CptArray.FirstOrDefault(d => d.name == $"{newName}");

        //string path = $"Data/Entity/Capacity/Players/Players{playerId}/Affinity{affinityId}/Niveau {nextLevel}/{newName}";

        if (newCapacity == null)
        {
            Debug.LogWarning($"Capacité introuvable");
            return currentCapacity;
        }

        return newCapacity;
    }
    public CapacityData GetBaseCapacity(CapacityData currentCapacity)
    {
        string currentName = currentCapacity.name;
        if (currentName.Length > 7)
        {
            currentName = currentName.Remove(7);
        }
        char levelChar = currentName[6];

        if (!char.IsDigit(levelChar)|| levelChar == '0') return currentCapacity;
        string newName = currentName.Remove(6)+'0';

        CapacityData[] CptArray = Resources.LoadAll<CapacityData>("Data/Entity/Capacity");
        CapacityData newCapacity = CptArray.FirstOrDefault(d => d.name == $"{newName}");

        //string path = $"Data/Entity/Capacity/Players/Players{playerId}/Affinity{affinityId}/Niveau {nextLevel}/{newName}";

        if (newCapacity == null)
        {
            Debug.LogWarning($"Capacité introuvable");
            return currentCapacity;
        }

        return newCapacity;
    }

    public void ResetAllCapacities()
    {
        DataEntity player = currentTurnOrder[0];
        List<CapacityData> PCapacities = new List<CapacityData> { player._CapacityData1, player._CapacityData2, player._CapacityData3, player._CapacityDataUltimate };
        for (int i = 0; i < PCapacities.Count; i++)
        {
            CapacityData baseC = GetBaseCapacity(PCapacities[i]);

            switch (i)
            {
                case 0: player._CapacityData1 = baseC; break;
                case 1: player._CapacityData2 = baseC; break;
                case 2: player._CapacityData3 = baseC; break;
                case 3: player._CapacityDataUltimate = baseC; break;
            }

            UpdateSlot(player, i, baseC);
        }
    }

    public void ResetListener(int index, GameObject button = null)
    {
        DataEntity player = currentTurnOrder[0];
        CapacityData CData = null;

        switch (index)
        {
            case 0: CData = player._CapacityData1; break;
            case 1: CData = player._CapacityData2; break;
            case 2: CData = player._CapacityData3; break;
            case 3: CData = player._CapacityDataUltimate; break;
        }
        List<CapacityData> AllCDataLevels = new List<CapacityData>() { CData, GetCycledCapacity(CData), GetCycledCapacity(GetCycledCapacity(CData)) };
        bool CoolDown = false;
        foreach (CapacityData C in AllCDataLevels)
        {
            if (IsCapacityOnCooldown(player, C)) CoolDown = true;
        }
        if (currentSelectedButton == button && currentSelectedButton != null && !CoolDown)
        {
            UseCapacity(CData);
        }
    }
    public void UpgradeCpt(int i, GameObject button = null)
    {
        if (button != currentInterractingButton)
        {
            currentInterractingButton = button;
            ResetAllCapacities();
        }
        DataEntity player = currentTurnOrder[0];
        List<CapacityData> PCapacities = new List<CapacityData> { player._CapacityData1, player._CapacityData2, player._CapacityData3, player._CapacityDataUltimate };
        CapacityData CData = PCapacities[i];
        CData = GetCycledCapacity(CData);
        switch (i)
        {
            case 0: player._CapacityData1 = CData; break;
            case 1: player._CapacityData2 = CData; break;
            case 2: player._CapacityData3 = CData; break;
            case 3: player._CapacityDataUltimate = CData; break;
        }
        UpdateSlot(player,i,CData);
        capacityAnimButtons[i].onClick.RemoveAllListeners();
        capacityAnimButtons[i].onClick.AddListener(() => UseCapacity(CData));
    }

    #endregion

    #region SetupUI

    private void SetupCapacityButtons(DataEntity player)
    {
        entiityManager.UpdateSpellData(player);
        UpdatePage(player);

        for (int i = 0; i < Banderoles.Count(); i++)
        {
            Banderoles[i].SetActive(false);
        }
        if (player.Affinity > 0)
        {
            Banderoles[player.Affinity - 1].SetActive(true);
        }
        for (int i = 0; i < capacityButtons.Length; i++)
        {
            capacityButtons[i].gameObject.SetActive(false);
            capacityAnimButtons[i].onClick.RemoveAllListeners();
            capacityAnimButtons[i].animator.SetTrigger("Normal");
        }

        if (player._CapacityData1 != null)
        {
            SetupButtonFunction(player, 0, player._CapacityData1, player.capacity1);
        }
        else
            Debug.LogWarning("CapacityData1 is null!");

        if (player._CapacityData2 != null)
        {
            SetupButtonFunction(player, 1, player._CapacityData2, player.capacity2);
        }
        else
            Debug.LogWarning("CapacityData2 is null!");

        if (player._CapacityData3 != null)
        {
            SetupButtonFunction(player, 2, player._CapacityData3, player.capacity3);
        }
        else
            Debug.LogWarning("CapacityData3 is null!");

        if (player._CapacityDataUltimate != null)
        {
            capacityButtons[3].gameObject.SetActive(true);
            capacityButtons[3].GetComponent<Image>().sprite = player.Ultimate;
            ultimateScript.SyncSliderWithEntity();
            capacityAnimButtons[3].GetComponent<Button>().interactable = false;
            if (player.UltimateSlider <= 0)
            {
                capacityAnimButtons[3].GetComponent<Button>().interactable = true;
                capacityAnimButtons[3].onClick.AddListener(() => ultimateScript.QTE_Start(player, capacityAnimButtons[3]));
            }
        }
        else
            Debug.LogWarning("CapacityDataUltimate is null!");

    }

    public void SetUltimate()
    {
        entiityManager.UpdateSpellData(currentPlayer);
        GlobalVars.currentSelectedCapacity = currentPlayer._CapacityDataUltimate;
    }
    private void SetupButtonFunction(DataEntity player, int i, CapacityData CData, Sprite CSprite)
    {
        DataEntity caster = player;
        DataEntity.CooldownData? cooldownData = caster.ActiveCooldowns.Find(cd => cd.capacity.name == CData.name);
        if (cooldownData.HasValue)
        {
            if (cooldownData.HasValue && cooldownData.Value.remainingCooldown > 0)
            {
                int remainingCooldown = cooldownData.Value.remainingCooldown;
    
                CoolDownTexts[i].SetText($"{remainingCooldown}");
                capacityButtons[i].gameObject.SetActive(true);
                capacityButtons[i].GetComponent<Image>().sprite = CSprite;
                capacityButtons[i].GetComponent<Image>().material = GreyScale;
                capacityAnimButtons[i].GetComponent<Button>().interactable = false;
                capacityAnimButtons[i].GetComponent<Animator>().SetTrigger("Normal");
            }
            else
            {
                CoolDownTexts[i].SetText("");  // rien à afficher
                capacityButtons[i].gameObject.SetActive(true);
                capacityButtons[i].GetComponent<Image>().sprite = CSprite;
                capacityButtons[i].GetComponent<Image>().material = null;
                capacityAnimButtons[i].GetComponent<Button>().interactable = true;
                capacityAnimButtons[i].GetComponent<Animator>().SetTrigger("Normal");
                capacityAnimButtons[i].onClick.AddListener(() => UseCapacity(CData));
            }
        }
        else
        {
            capacityButtons[i].gameObject.SetActive(true);
            capacityButtons[i].GetComponent<Image>().sprite = CSprite;
            capacityButtons[i].GetComponent<Image>().material = null;
            capacityAnimButtons[i].GetComponent<Button>().interactable = true;
            capacityAnimButtons[i].GetComponent<Animator>().SetTrigger("Normal");
            CoolDownTexts[i].SetText("");
        }
    }

    private void UpdatePage(DataEntity player)
    {
        currentSelectedButton = null;
        List<CapacityData> PCapacities = new List<CapacityData> { player._CapacityData1, player._CapacityData2, player._CapacityData3, player._CapacityDataUltimate };
        Transform EncartAffinity = null;
        if (player.ChargePower >= player.UltChargePowerCost)
        {
            Ultimate.SINGLETON.GainUltimateCharge(100);
            Ultimate.SINGLETON.SliderManager();
        }

        for (int i = 0; i<4; i++)

        {
            UpdateSlot(player, i, PCapacities[i]);
            EncartAffinity = capacityPageAffinity[i].GetComponent<Transform>();
            EncartAffinity.gameObject.SetActive(false);
            EncartAffinity.GetChild(1).GameObject().SetActive(false);
        }
    }
    private void UpdateSlot(DataEntity player, int i, CapacityData CData)
    {
        List<CapacityData> PCapacities = new List<CapacityData> { player._CapacityData1, player._CapacityData2, player._CapacityData3, player._CapacityDataUltimate };
        //Reset de tout
        Transform EncartCpt = capacityPage[i].GetComponent<Transform>();
        Transform EncartAffinity = capacityPageAffinity[i].GetComponent<Transform>();
        Transform Text = EncartCpt.GetChild(4);
        Sprite Target = TargetType[CData.TargetType];
        Sprite PictoType = Pictos[CData.PictoType];
        TextMeshProUGUI Description = EncartCpt.GetChild(1).GetComponent<TextMeshProUGUI>();
        String Sbuff = "";
        Text.GetChild(0).GameObject().SetActive(false);
        EncartCpt.GetChild(3).GetChild(0).GameObject().SetActive(false);

        //MAJ de la data de base
        EncartCpt.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(CData.Name);
        Description.SetText(CData.Description);
        EncartCpt.GetChild(2).GetComponent<Image>().sprite = Target;
        EncartCpt.GetChild(3).GetChild(1).GetComponent<Image>().sprite = PictoType;


        //MAJ de la data si affinity
        if (player.Affinity != 0 && EncartAffinity!= null)
        {
            Sprite Picto = null;
            String EffectValue = "";
            if (CData.Shield > 0)
            {
                Picto = PictosAffinity[0];
                EffectValue = $"{CData.Shield}";
            }
            if (CData.ShieldRatioAtk > 0)
            {
                Picto = PictosAffinity[0];
                EffectValue = $"{CData.ShieldRatioAtk}%";
            }
            if (CData.Shock > 0)
            {
                Picto = PictosAffinity[1];
                EffectValue = $"{CData.Shock}";
            }

            if (CData.Necrosis > 0)
            {
                Picto = PictosAffinity[2];
                EffectValue = $"{CData.Necrosis}";
            }
            if (Picto != null)
            {
                EncartAffinity.GetChild(2).gameObject.SetActive(true);
                EncartAffinity.GetChild(2).GetComponent<Image>().sprite = Picto;
            }
            else
            {
                EncartAffinity.GetChild(2).gameObject.SetActive(false);
            }
            EncartAffinity.GetChild(3).GetComponent<TextMeshProUGUI>().text = EffectValue;
            EncartAffinity.GetChild(0).GetComponent<TextMeshProUGUI>().text = CData.AffinityDescription;


        }

        if (CData.buffType != 0)
        {
            GameObject BuffIcon = null;
            GameObject BuffValue = null;

            if (!CData.BuffFromAffinity)
            {
                BuffIcon = EncartCpt.GetChild(3).GetChild(0).GameObject();
                Text.GetChild(0).GameObject().SetActive(true);
                BuffValue = Text.GetChild(0).GameObject();
            }
            else
            {
                BuffIcon = EncartAffinity.GetChild(1).GameObject();
                EncartAffinity.GetChild(3).GameObject().SetActive(true);
                BuffValue = EncartAffinity.GetChild(3).GameObject();
            }

            BuffIcon.SetActive(true);

            string arrow;
            if (CData.buffValue > 1)
            {
                BuffIcon.GetComponent<TextMeshProUGUI>().color = new Color32(0, 39, 11, 255);
                arrow = "▲";
                Sbuff = $"+{Mathf.RoundToInt((CData.buffValue - 1) * 100)}%";
            }
            else
            {
                BuffIcon.GetComponent<TextMeshProUGUI>().color = new Color32(155, 0, 0, 255);
                arrow = "▼";
                Sbuff = $"-{Mathf.RoundToInt((1 - CData.buffValue) * 100)}%";
            }

            switch (CData.buffType)
            {
                case 1:
                    BuffIcon.GetComponent<TextMeshProUGUI>().SetText($"ATQ{arrow}");
                    break;
                case 2:
                    BuffIcon.GetComponent<TextMeshProUGUI>().SetText($"DEF{arrow}");
                    break;
                case 3:
                    BuffIcon.GetComponent<TextMeshProUGUI>().SetText($"VIT{arrow}");
                    break;
                case 4:
                    BuffIcon.GetComponent<TextMeshProUGUI>().SetText($"PRE{arrow}");
                    break;
            }
            if (CData.specialType == SpecialCapacityType.UltGarde)
            {
                player.CptUltlvl = player.UltLvl_1 + player.UltLvl_2 + player.UltLvl_3 + player.UltLvl_4;
                Sbuff = $"+{Mathf.RoundToInt((CData.buffValue-1) * 100 + (0.05f * player.CptUltlvl))}%";
            }
            BuffValue.GetComponent<TextMeshProUGUI>().SetText(Sbuff);

        }

        int Value = 0;
        Value = Math.Max(CData.atk, CData.heal);
        if (CData.heal > 0)
        {
            Value = Mathf.RoundToInt((Mathf.Sqrt(2 * player.UnitAtk) + CData.heal));
        }
        if (CData.atk > 0)
        {
            Value = Mathf.RoundToInt(CData.atk * (player.UnitAtk + 20) / 20);
        }
        if (CData.specialType == SpecialCapacityType.UltMoine)
        {
            player.CptUltlvl = player.UltLvl_1 + player.UltLvl_2 + player.UltLvl_3 + player.UltLvl_4;
            Value += Mathf.RoundToInt(0.05f * player.CptUltlvl);
        }
        if (CData.specialType == SpecialCapacityType.UltPriso)
        {
            player.CptUltlvl = player.UltLvl_1 + player.UltLvl_2 + player.UltLvl_3 + player.UltLvl_4;
            Value +=Mathf.RoundToInt(2*player.CptUltlvl);
        }
        Text.GetChild(1).GetComponent<TextMeshProUGUI>().SetText($"{Value}");
        Text.GetChild(2).GetComponent<TextMeshProUGUI>().SetText($"{CData.précision}%");
        Text.GetChild(3).GetComponent<TextMeshProUGUI>().SetText($"{CData.critique}%");
        if (CData.MultipleAttack)
        {
            EncartCpt.GetChild(5).GetComponent<TextMeshProUGUI>().SetText($"{CData.cooldown-1} tour");

        }
        else
        {
            EncartCpt.GetChild(5).GetComponent<TextMeshProUGUI>().SetText($"{CData.cooldown} tour");

        }
    }

    void ShowTargetIndicators(CapacityData capacity)
    {
        HideTargetIndicators();

        if (capacity.atk > 0 && !capacity.MultipleAttack)
        {
            int activeIndex = 0;

            for (int i = 0; i < entityHandler.ennemies.Count; i++)
            {
                DataEntity enemy = entityHandler.ennemies[i];
                if (enemy == null || enemy.UnitLife <= 0)
                    continue;
                if (activeIndex < circlesEnnemy.Count && circlesEnnemy[activeIndex] != null)
                {
                    circlesEnnemy[activeIndex].SetActive(true);
                }
                if (enemy.instance != null)
                {
                    Collider2D enemyColl = enemy.instance.GetComponent<Collider2D>();
                    if (enemyColl != null)
                    {
                        enemyColl.enabled = true;
                    }
                    else
                    {
                        Debug.LogWarning($"Pas de Collider2D sur {enemy.namE}");
                    }
                }
                activeIndex++;
            }
        }
        else if (capacity.heal > 0 && !capacity.MultipleHeal || capacity.Provocation == true)
        {
            for (int i = 0; i < entityHandler.players.Count && i < circlesPlayer.Count; i++)
            {
                DataEntity player = entityHandler.players[i];

                if (player == null || player.UnitLife <= 0)
                    continue;
                circlesPlayer[i].SetActive(true);
                if (player.instance != null)
                {
                    Collider2D playerColl = player.instance.GetComponent<Collider2D>();
                    if (playerColl != null)
                    {
                        playerColl.enabled = true;
                    }
                    else
                    {
                        Debug.LogWarning($"Pas de Collider2D sur {player.namE}");
                    }
                }
            }
        }
    }
    #endregion

    #region StatusEffects

    public void GiveBuff(CapacityData capacity, DataEntity target, float ultGarde = 0)
    {
        // buffType; 1 = Atk, 2 = Def, 3 = Speed, 4 = Précision
        if (capacity.buffType > 0 && capacity.DoubleEffect == false)
        {
            ActiveBuff existingBuff = target.ActiveBuffs.Find(b => b.type == capacity.buffType);
            if (existingBuff != null)
            {
                existingBuff.value *= capacity.buffValue+ultGarde;
                existingBuff.duration = Mathf.Max(existingBuff.duration, capacity.buffDuration);
            }
            else
            {
                target.ActiveBuffs.Add(new ActiveBuff(capacity.buffType, capacity.buffValue+ultGarde, capacity.buffDuration));
            }

            RecalculateStats(target);
        }
        if (capacity.buffType > 0 && capacity.DoubleEffect == true)
        {
            ActiveBuff existingBuff = target.ActiveBuffs.Find(b => b.type == capacity.secondaryBuffType);
            if (existingBuff != null)
            {
                existingBuff.value *= capacity.buffValue+ultGarde;
                existingBuff.duration = Mathf.Max(existingBuff.duration, capacity.secondaryBuffDuration);
            }
            else
            {
                target.ActiveBuffs.Add(new ActiveBuff(capacity.secondaryBuffType, capacity.secondaryBuffValue+ultGarde, capacity.secondaryBuffDuration));
            }

            RecalculateStats(target);
        }
    }

    public void ShockProc(CapacityData capacity, DataEntity target,bool enemyHasShield)
    {
        DataEntity caster = currentTurnOrder[0];
        if (capacity.Shock > 0)
        {
            target.ShockMark += capacity.Shock;
            Debug.Log($"{caster.name} a appliqué {capacity.Shock} marque(s) à {target.namE}");
            if (target.ShockMark >= 1 && target.ShockMark <= 4)
            {
                int visualIndex = entityHandler.players.Contains(target)
                    ? entityHandler.players.IndexOf(target)
                    : entityHandler.players.Count + entityHandler.ennemies.IndexOf(target);
                EffectsManager.SINGLETON.AfficherAttaqueFoudre(target.ShockMark, visualIndex, target);
            }
            if (target.ShockMark >= 4)
            {
                float calculatedDamage = (caster.UnitSpeed - 20) / 2;
                if (enemyHasShield) calculatedDamage = calculatedDamage * 150 / 100;
                int icalculatedDamage = Mathf.RoundToInt(calculatedDamage);
                if (target.UnitShield > 0)
                {
                    if (target.UnitShield < icalculatedDamage)
                    {
                        icalculatedDamage -= target.UnitShield;
                        target.UnitShield = 0;
                        Debug.Log($"{caster.namE} a brisé le shield de {target.namE} grâce au choc");
                    }
                    else
                    {
                        target.UnitShield -= icalculatedDamage;
                        icalculatedDamage = 0;
                        Debug.Log($"{caster.namE} inflige {icalculatedDamage} dégâts au bouclier de {target.namE} grâce au choc");
                    }

                }
                if (icalculatedDamage > 0)
                {
                    target.UnitLife -= icalculatedDamage;
                    Debug.Log($"{caster.namE} inflige {icalculatedDamage} dégâts à {target.namE} grâce au choc");
                }
                target.ShockMark = 0;
            }
        }
    }

    public float GetBonusRageDamage(DataEntity caster)
    {
        if (caster.Affinity != 4 || caster.RageTick < 3) return 0f;

        int level = caster.RageTick >= 12 ? 3 :
                    caster.RageTick >= 9 ? 2 :
                    caster.RageTick >= 6 ? 1 :
                    0;

        if (level == 0) return 0f;

        float[] baseDamages = level switch
        {
            1 => new float[] { 1f, 1f, 1f, 3f },
            2 => new float[] { 1f, 1f, 2f, 4f },
            3 => new float[] { 1f, 2f, 3f, 5f },
            _ => new float[4]
        };

        int activeTicks = Mathf.Min(caster.RageTick, 12);
        int rageStageCount = activeTicks / 3;

        float bonus = 0f;
        for (int i = 0; i < rageStageCount; i++)
        {
            bonus += baseDamages[i];
        }

        // Ajout du bonus d'attaque : +0.5 par 10 points d'atk de base
        float atkBonus = Mathf.Floor(caster.BaseAtk / 10f) * 0.5f * rageStageCount;

        return bonus + atkBonus;
    }

    public void ApplyRage(DataEntity target)
    {
        if (target.Affinity != 4) return;

        bool isPlayer = entityHandler.players.Contains(target);
        List<DataEntity> team = isPlayer ? entityHandler.players : entityHandler.ennemies;

        int sameAffinityCount = team.Count(entity => entity.Affinity == 4);
        target.RageTick = Mathf.Min(target.RageTick + sameAffinityCount + 1, 12);

        int visualIndex = isPlayer
            ? entityHandler.players.IndexOf(target)
            : entityHandler.players.Count + entityHandler.ennemies.IndexOf(target);

        EffectsManager.SINGLETON.AfficherRageSlider(target.RageTick, visualIndex);
    }


    public void ApplyNecrosis(DataEntity target, int levelToAdd)
    {
        if (target.necrosis == null || target.necrosis.Count == 0)
        {
            target.necrosis = new List<Necrosis> { new Necrosis(levelToAdd) };
        }
        else
        {
            var effect = target.necrosis[0];
            int oldLevel = effect.level;
            effect.level = Mathf.Clamp(effect.level + levelToAdd, 1, 5);
            Debug.Log($"[NÉCROSE] Niveau augmenté de {oldLevel} à {effect.level} (tours restants : {effect.remainingTurns})");
        }

        var necrosis = target.necrosis[0];
        Debug.Log($"{target.namE} est maintenant affecté par la nécrose niveau {necrosis.level}, {necrosis.remainingTurns} tours restants");
    }


    public void TickNecrosisEffect(DataEntity target)
    {
        if (target.necrosis?.Count > 0)
        {
            int[] baseDamage = { 0, 1, 2, 3, 4, 5 };
            float[] speedPercents = { 0f, 0.05f, 0.010f, 0.15f, 0.20f, 0.25f };
            var necrosisEffect = target.necrosis[0];
            int level = necrosisEffect.level;
            int speedDamage = Mathf.RoundToInt(target.UnitSpeed * speedPercents[level]);
            int damage = baseDamage[level] + speedDamage;

            target.UnitLife -= damage;

            Debug.Log($"{target.namE} subit {damage} dégâts de nécrose (niveau {necrosisEffect.level})");

            necrosisEffect.remainingTurns--;

            if (target.necrosis != null && target.necrosis.Count > 0 && target.necrosis[0].IsExpired)
            {
                target.necrosis.Clear();
                Debug.Log($"{target.namE} n'est plus affecté par la nécrose");
            }
        }
    }
    #endregion

    #region Miscellaneous
    public float lancer(float valeur, float above, float under)
    {
        int lancer = UnityEngine.Random.Range(0, 100);
        if (lancer > valeur)
        {
            return above;
        }
        return under;
    }

    public void RecalculateStats(DataEntity target)
    {
        float atkMultiplier = 1f;
        float defMultiplier = 1f;
        float speedMultiplier = 1f;
        float aimMultiplier = 1f;

        foreach (var buff in target.ActiveBuffs)
        {
            switch (buff.type)
            {
                case 1: atkMultiplier *= buff.value; break;
                case 2: defMultiplier *= buff.value; break;
                case 3: speedMultiplier *= buff.value; break;
                case 4: aimMultiplier *= buff.value; break;
            }
        }
        target.UnitAtk = Mathf.RoundToInt(target.BaseAtk * atkMultiplier);
        target.UnitDef = Mathf.RoundToInt(target.BaseDef * defMultiplier);
        target.UnitSpeed = Mathf.RoundToInt(target.BaseSpeed * speedMultiplier);
        target.UnitAim = Mathf.RoundToInt(target.BaseAim * aimMultiplier);
    }
    public void DecrementBuffDurations(DataEntity target)
    {
        for (int i = target.ActiveBuffs.Count - 1; i >= 0; i--)
        {
            target.ActiveBuffs[i].duration--;
            if (target.ActiveBuffs[i].duration <= 0)
                target.ActiveBuffs.RemoveAt(i);
        }

        RecalculateStats(target);
    }
    public void SetupNewAffinity(int NewAffinity)
    {
        currentPlayer.Affinity = NewAffinity;
        entiityManager.UpdateSpellData(currentPlayer);
    }

    public bool IsCapacityOnCooldown(DataEntity caster, CapacityData capacity)
    {
        foreach (var cd in caster.ActiveCooldowns)
        {
            if (cd.capacity.name == capacity.name)
                return true; // en cooldown
        }
        return false; // disponible
    }
    public void DecrementCooldowns(DataEntity caster)
    {
        for (int i = caster.ActiveCooldowns.Count - 1; i >= 0; i--)
        {
            CooldownData data = caster.ActiveCooldowns[i];
            data.remainingCooldown--;

            if (data.remainingCooldown <= 0)
            {
                caster.ActiveCooldowns.RemoveAt(i);
                RefreshButtonState(caster, data.capacity);
            }
            else
            {
                caster.ActiveCooldowns[i] = data;
            }
        }
    }
    private void RefreshButtonState(DataEntity caster, CapacityData capacity)
    {
        for (int i = 0; i < capacityButtons.Count(); i++)
        {
            if (capacityAnimButtons[i].onClick.GetPersistentEventCount() > 0)
            {
                if (capacityAnimButtons[i].onClick.GetPersistentMethodName(0) == "UseCapacity")
                {
                    capacityButtons[i].GetComponent<Image>().material = null;
                    capacityAnimButtons[i].GetComponent<Button>().interactable = true;
                    CoolDownTexts[i].SetText("");
                }
            }
        }
    }
    private void ExecuteDelayedActions(DataEntity entity)
    {
        for (int i = entity.delayedActions.Count - 1; i >= 0; i--)
        {
            var delayedAction = entity.delayedActions[i];

            ApplyNormalCapacity(delayedAction.capacity, entity, delayedAction.target, 1f);

            entity.delayedActions.RemoveAt(i);
        }
    }
    public void ActivateAffinityPage(GameObject AffinityPage)
    {
        if(GlobalVars.currentPlayer.Affinity != 0)
        {
            AffinityPage.SetActive(true);
        }
    }
    public void ActivateAffinityPage2(GameObject AffinityPage)
    {
        if (GlobalVars.currentPlayer.Affinity != 0)
        {
            AffinityPage.SetActive(false);
        }
    }
    #endregion
}