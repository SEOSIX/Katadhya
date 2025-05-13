using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static DataEntity;
using static UnityEngine.EventSystems.EventTrigger;
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
    public GameObject[] Banderoles;
    public TextMeshProUGUI[] CoolDownTexts;
    public Sprite[] Pictos;
    public Sprite[] TargetType;
    public Sprite[] PictoBuffs;
    public Material GreyScale;
    private DataEntity currentPlayer;

    [Header("Turn Management")]
    public Button endTurnButton;
    public Slider LifePlayers;

    [Header("TurnObject")]
    public GameObject ennemyTurn;
    public GameObject TurnUI;
    public CanvasGroup canvasGroup;
    [Header("Turn Indicators")]
    public GameObject[] playerTurnIndicators;

    [Header("Target Indicators")]
    public List<GameObject> circlesEnnemy;
    public List<GameObject> circlesPlayer;

    [Header("Ultimate")]
    public Ultimate ultimateScript;

    [HideInInspector] public List<DataEntity> currentTurnOrder = new List<DataEntity>();
    [HideInInspector] public List<DataEntity> unitPlayedThisTurn = new List<DataEntity>();
    private System.Random r = new System.Random();
    [HideInInspector] public bool PlayerClickable;
    [HideInInspector] public bool EnemyClickable;

    public static class GlobalVars
    {
        public static CapacityData currentSelectedCapacity;
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

    void Start()
    {
        CombatManager.SINGLETON.currentTurnOrder = CombatManager.SINGLETON.GetUnitTurn();
    }

    void Update()
    {
        InitializeStaticUI();
    }

    public void SetupBaseStat()
    {
        for (int i = 0; i < entityHandler.ennemies.Count; i++)
        {
            entityHandler.ennemies[i].UnitLife = entityHandler.ennemies[i].BaseLife;
            entityHandler.ennemies[i].UnitAtk = entityHandler.ennemies[i].BaseAtk;
            entityHandler.ennemies[i].UnitDef = entityHandler.ennemies[i].BaseDef;
            entityHandler.ennemies[i].UnitSpeed = entityHandler.ennemies[i].BaseSpeed;
            entityHandler.ennemies[i].UnitAim = entityHandler.ennemies[i].BaseAim;
            entityHandler.ennemies[i].ActiveBuffs.Clear();
            entityHandler.ennemies[i].ActiveCooldowns.Clear();
            entityHandler.ennemies[i].skipNextTurn = false;
            entityHandler.ennemies[i].delayedActions.Clear();
            entityHandler.ennemies[i].ShockMark = 0;
            entityHandler.ennemies[i].RageTick = 0;
            entityHandler.ennemies[i].LastRageTick = 0;
            entityHandler.ennemies[i].necrosis = null;
            entityHandler.ennemies[i].beenHurtThisTurn = false;
        }
        for (int i = 0; i < entityHandler.players.Count; i++)
        {
            entityHandler.players[i].UnitLife = entityHandler.players[i].BaseLife;
            entityHandler.players[i].UnitAtk = entityHandler.players[i].BaseAtk;
            entityHandler.players[i].UnitDef = entityHandler.players[i].BaseDef;
            entityHandler.players[i].UnitSpeed = entityHandler.players[i].BaseSpeed;
            entityHandler.players[i].UnitAim = entityHandler.players[i].BaseAim;
            entityHandler.players[i].ActiveBuffs.Clear();
            entityHandler.players[i].ActiveCooldowns.Clear();
            entityHandler.players[i].skipNextTurn = false;
            entityHandler.players[i].delayedActions.Clear();
            entityHandler.players[i].ShockMark = 0;
            entityHandler.players[i].RageTick = 0;
            entityHandler.players[i].LastRageTick = 0;
            entityHandler.players[i].necrosis = null;
            entityHandler.players[i].UltimateSlider = 100;
            entityHandler.players[i].Affinity = 0;
            entityHandler.players[i].beenHurtThisTurn = false;
        }
    }
    public void InitializeStaticUI()
    {
        if (currentTurnOrder == null || currentTurnOrder.Count == 0) return;

        DataEntity currentEntity = currentTurnOrder[0];

        textplayer.text = currentEntity.name;
        speed.text = "Speed :" + currentEntity.UnitSpeed;
        def.text = "Defence :" + currentEntity.UnitDef;
        atck.text = "Attack :" + currentEntity.UnitAtk;
        lifeText.text = currentEntity.UnitLife + "/" + currentEntity.BaseLife;
        playerPortrait.sprite = currentEntity.bandeauUI;
        LifePlayers.maxValue = currentEntity.BaseLife;
        LifePlayers.value = currentEntity.UnitLife;

        List<DataEntity> initialTurnOrder = GetUnitTurn();
        for (int i = 0; i < ImagePortrait.Length; i++)
        {
            if (i < initialTurnOrder.Count && initialTurnOrder[i].UnitLife > 0)
            {
                ImagePortrait[i].enabled = true;
                ImagePortrait[i].sprite = initialTurnOrder[i].portraitUI;
                targetSizes = new Vector2[ImagePortrait.Length];
                if (initialTurnOrder[i] == currentEntity)
                {
                    targetSizes[i] = new Vector2(90, 105);
                }
                else
                {
                    targetSizes[i] = new Vector2(75, 93);
                }
                if (ImagePortrait[i] != null && ImagePortrait[i].enabled)
                {
                    ImagePortrait[i].rectTransform.sizeDelta = Vector2.Lerp(
                        ImagePortrait[i].rectTransform.sizeDelta,
                        targetSizes[i],
                        Time.deltaTime * 10f
                    );
                }
            }
            else
            {
                ImagePortrait[i].enabled = false;
            }
        }
    }

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
        currentTurnOrder.AddRange(unitPlayedThisTurn);
        unitPlayedThisTurn.Clear();
        currentTurnOrder = currentTurnOrder.OrderByDescending(x => x.UnitSpeed).ToList();
    }

    public void StartUnitTurn()
    {
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
    
        if (current.skipNextTurn)
        {
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
        StartTargetSelectionMode(cpt);
    }

    public void StartTargetSelectionMode(CapacityData capacity)
    {
        DataEntity caster = currentTurnOrder[0]; 
        HideTargetIndicators();
        GlobalVars.currentSelectedCapacity = capacity;
        print(capacity);
        if (capacity.MultipleHeal)
        {
            foreach (var ally in entityHandler.players)
            {
                if (ally.UnitLife > 0)
                {
                    ApplyCapacityToTarget(capacity, ally);
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
            foreach (var enemy in entityHandler.ennemies)
            {
                if (enemy.UnitLife > 0)
                {
                    ApplyCapacityToTarget(capacity, enemy);
                }
            }
            DecrementBuffDurations(currentTurnOrder[0]);
            DecrementCooldowns(currentTurnOrder[0]);
            GlobalVars.currentSelectedCapacity = null;
            Debug.Log("Capacité de zone appliquée à tous les ennemis !");
            EndUnitTurn();
            return;
        }
        ShowTargetIndicators(capacity);
        DecrementBuffDurations(caster);
        DecrementCooldowns(caster);
        if (caster.beenHurtThisTurn == false && caster.RageTick > 0)
        {
            caster.RageTick -= 1;
        }
        if (caster.necrosis != null && caster.necrosis.Count > 0)
        {
            TickNecrosisEffect(caster);
        }
        caster.beenHurtThisTurn = false;
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
        print(target);
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

    public void ApplyCapacityToTarget(CapacityData capacity, DataEntity target)
    {
        DataEntity caster = currentTurnOrder[0];
        int globalAim = Mathf.RoundToInt(capacity.précision * caster.UnitAim);
        float réussite = lancer(globalAim, 2f, 1f);

        if (réussite == 2)
        {
            Debug.Log("Échec de la compétence");
            return;
        }

        float modifier = lancer(capacity.critique, 1f, 1.5f);

        if (caster.Affinity == 3)
        {
            if (caster.RageTick >= 12)
            {
                RageProc(caster);
            }
                
        }

        if (capacity.specialType != SpecialCapacityType.None)
        {
            ApplySpecialCapacity(capacity, caster, target, modifier);
        }
        if (capacity.DoubleEffect && entityHandler.ennemies.Contains(target))
        {
            ApplySecondaryCapacity(capacity, caster, target, modifier);
        }
        else
        {
            ApplyNormalCapacity(capacity, caster, target, modifier);
        }

        if (capacity.cooldown > 0)
        {
            bool alreadyInCooldown = caster.ActiveCooldowns.Exists(cd => cd.capacity == capacity);

            if (!alreadyInCooldown)
            {
                caster.ActiveCooldowns.Add(new CooldownData(capacity, capacity.cooldown));
            }
        }
        if (Ultimate.SINGLETON != null)
        {
            Ultimate.SINGLETON.GainUltimateCharge(capacity.chargeUlti);
        }
        InitializeStaticUI();

    }


 public void ApplyNormalCapacity(CapacityData capacity, DataEntity caster, DataEntity target, float modifier)
{
    int DamageDone = 0;
    int visualIndex = GetEntityVisualIndex(target);

    // ATTAQUE
    if (capacity.atk > 0)
    {
        float calculatedDamage = (((caster.UnitAtk + 1) * capacity.atk * modifier) / (2 + caster.UnitAtk + target.UnitDef));
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
        }

        if (icalculatedDamage > 0)
        {
            target.UnitLife -= icalculatedDamage;
        }

        target.beenHurtThisTurn = true;
        if (caster.Affinity == 1)
        {
            EffectsManager.SINGLETON.AfficherAttaqueBouclier(visualIndex, DamageDone);
        }
        else
            EffectsManager.SINGLETON.AfficherAttaqueSimple(visualIndex, DamageDone);
    }
    if (capacity.heal > 0)
    {
        int healAmount = Mathf.RoundToInt((((caster.UnitAtk) + capacity.heal) / 2) * modifier);
        target.UnitLife = Mathf.Min(target.UnitLife + healAmount, target.BaseLife);

        EffectsManager.SINGLETON.AfficherHeal(visualIndex, healAmount);
    }
    if (capacity.Shield > 0)
    {
        target.UnitShield += Mathf.RoundToInt(capacity.Shield * modifier);
    }
    if (capacity.Provocation == true)
    {
        target.provoking = true;
    }
    if (capacity.buffType > 0)
    {
        GiveBuff(capacity, target);
        EffectsManager.SINGLETON.AfficherPictoBuff(visualIndex);
    }
    if (capacity.Shock > 0)
    {
        ShockProc(capacity, target);
    }
    if (capacity.ShieldRatioAtk > 0)
    {
        int Shielding = Mathf.RoundToInt(((float)capacity.ShieldRatioAtk / 100) * DamageDone);
        caster.UnitShield += Shielding;
    }
    if (target.Affinity == 3)
    {
        RageApplication(target);
    }

    if (caster.Affinity == 4)
    {
        ApplyNecrosis(target);
    }
}


    private void ApplySpecialCapacity(CapacityData capacity, DataEntity caster, DataEntity target, float modifier)
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
            default:
                Debug.LogWarning("Special capacity type not handled.");
                break;
        }
    }
    public void ApplySecondaryCapacity(CapacityData capacity, DataEntity caster, DataEntity target, float modifier)
    {
        int DamageDone = 0;
        int visualIndex = GetEntityVisualIndex(target);

        // ATTAQUE
        if (capacity.atk > 0)
        {
            float calculatedDamage = (((caster.UnitAtk + 1) * capacity.secondaryAtk * modifier) / (2 + caster.UnitAtk + target.UnitDef));
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
            }

            if (icalculatedDamage > 0)
            {
                target.UnitLife -= icalculatedDamage;
            }

            target.beenHurtThisTurn = true;
            EffectsManager.SINGLETON.AfficherAttaqueSimple(visualIndex, icalculatedDamage);
        }
        if (capacity.secondaryHeal > 0)
        {
            int healAmount = Mathf.RoundToInt((((caster.UnitAtk) + capacity.secondaryHeal) / 2) * modifier);
            target.UnitLife = Mathf.Min(target.UnitLife + healAmount, target.BaseLife);

            // (Ajoute un effet de soin ici si souhaité)
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
            EffectsManager.SINGLETON.AfficherPictoBuff(visualIndex);
        }
        if (capacity.Shock > 0)
        {
            ShockProc(capacity, target);
        }
        if (capacity.ShieldRatioAtk > 0)
        {
            int Shielding = Mathf.RoundToInt(((float)capacity.ShieldRatioAtk / 100) * DamageDone);
            caster.UnitShield += Shielding;
        }
        if (target.Affinity == 3)
        {
            RageApplication(target);
        }

        if (caster.Affinity == 4)
        {
            ApplyNecrosis(target);
        }
    }
    private void SetupCapacityButtons(DataEntity player)
    {
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
            SetupButtonFunction(0, player._CapacityData1, player.capacity1);
        }

        if (player._CapacityData2 != null)
        {
            SetupButtonFunction(1, player._CapacityData2, player.capacity2);
        }

        if (player._CapacityData3 != null)
        {
            SetupButtonFunction(2, player._CapacityData3, player.capacity3);
        }

        if (player._CapacityDataUltimate != null)
        {
            capacityButtons[3].gameObject.SetActive(true);
            capacityButtons[3].GetComponent<Image>().sprite = player.Ultimate;
            capacityAnimButtons[3].GetComponent<Button>().interactable = false;
            if (player.UltimateSlider <= 0)
            {
                capacityAnimButtons[3].GetComponent<Button>().interactable = true;
                capacityAnimButtons[3].onClick.AddListener(() => ultimateScript.QTE_Start());
            }
        }
        UpdatePage(player);
    }
    public void SetUltimate()
    {
        GlobalVars.currentSelectedCapacity = currentPlayer._CapacityDataUltimate;
    }
    private void SetupButtonFunction(int i, CapacityData CData, Sprite CSprite)
    {
        DataEntity caster = currentTurnOrder[0];
        // Chercher si la capacité est déjà dans ActiveCooldowns du caster
        DataEntity.CooldownData? cooldownData = caster.ActiveCooldowns.Find(cd => cd.capacity == CData);

        // Si la capacité a un cooldown actif
        if (cooldownData.HasValue)
        {
            // On récupère le cooldown restant
            int remainingCooldown = cooldownData.Value.remainingCooldown;

            // Si le cooldown est encore actif (plus grand que zéro)
            if (remainingCooldown > 0)
            {
                // Afficher le cooldown restant sur le texte associé au bouton
                CoolDownTexts[i].SetText($"{remainingCooldown}");

                // Désactiver le bouton et l'afficher en gris
                capacityButtons[i].gameObject.SetActive(true);
                capacityButtons[i].GetComponent<Image>().sprite = CSprite;
                capacityButtons[i].GetComponent<Image>().material = GreyScale;
                capacityAnimButtons[i].GetComponent<Button>().interactable = false;
                capacityAnimButtons[i].GetComponent<Animator>().SetTrigger("Normal");
            }
            else
            {
                // Si le cooldown est terminé, activer le bouton pour pouvoir utiliser la capacité
                capacityButtons[i].gameObject.SetActive(true);
                capacityButtons[i].GetComponent<Image>().sprite = CSprite;
                capacityButtons[i].GetComponent<Image>().material = null;
                capacityAnimButtons[i].GetComponent<Button>().interactable = true;
                capacityAnimButtons[i].GetComponent<Animator>().SetTrigger("Normal");
                capacityAnimButtons[i].onClick.AddListener(() => UseCapacity(CData));

                // Masquer le cooldown restant
                CoolDownTexts[i].SetText("");
            }
        }
        else
        {
            // Si la capacité n'est pas dans ActiveCooldowns, c'est qu'elle est prête à être utilisée
            capacityButtons[i].gameObject.SetActive(true);
            capacityButtons[i].GetComponent<Image>().sprite = CSprite;
            capacityButtons[i].GetComponent<Image>().material = null;
            capacityAnimButtons[i].GetComponent<Button>().interactable = true;
            capacityAnimButtons[i].GetComponent<Animator>().SetTrigger("Normal");


            // Masquer le cooldown restant
            CoolDownTexts[i].SetText("");
        }
    }
    private void UpdatePage(DataEntity player)
    {
        List<CapacityData> PCapacities = new List<CapacityData> { player._CapacityData1, player._CapacityData2, player._CapacityData3, player._CapacityDataUltimate };

        for (int i = 0; i < PCapacities.Count(); i++)
        {
            CapacityData CData = PCapacities[i];
            Transform Parent = capacityPage[i].GetComponent<Transform>();
            Transform Text = Parent.GetChild(4);
            Sprite Target = TargetType[CData.TargetType];
            Sprite PictoType = Pictos[CData.PictoType];
            TextMeshProUGUI Description = Parent.GetChild(1).GetComponent<TextMeshProUGUI>();
            String Sbuff = "";
            Text.GetChild(0).GameObject().SetActive(false);
            Parent.GetChild(3).GetChild(0).GameObject().SetActive(false);
            Parent.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(CData.Name);
            Description.SetText(CData.Description);
            Parent.GetChild(2).GetComponent<Image>().sprite = Target;
            Parent.GetChild(3).GetChild(1).GetComponent<Image>().sprite = PictoType;
            
            if (CData.buffType != 0)
            {
                Parent.GetChild(3).GetChild(0).GameObject().SetActive(true);
                Text.GetChild(0).GameObject().SetActive(true);
                string arrow = "";
                if (CData.buffValue > 1)
                {
                    Parent.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(0, 39, 11, 255);
                    arrow = "▲";
                    Sbuff = $"{Mathf.RoundToInt((CData.buffValue - 1) * 100)}";
                }
                else
                {
                    Parent.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(155, 0, 0, 255);

                    arrow = "▼";
                    Sbuff = $"-{Mathf.RoundToInt((1 - CData.buffValue) * 100)}%";
                }
                if (CData.buffType == 1)
                {
                    Parent.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().SetText($"ATK{arrow}");
                }
                if (CData.buffType == 2)
                {
                    Parent.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().SetText($"DEF{arrow}");
                }
                if (CData.buffType == 3)
                {
                    Parent.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().SetText($"VIT{arrow}");
                }
                if (CData.buffType == 4)
                {
                    Parent.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().SetText($"PRE{arrow}");
                }
                Text.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(Sbuff);
            }
            int Value = 0;
            Value = Math.Max(CData.atk, CData.heal);
            if (CData.heal > 0)
            {
                Value = ((CData.heal + player.UnitAtk) / 2);
            }
            Text.GetChild(1).GetComponent<TextMeshProUGUI>().SetText($"{Value}");
            Text.GetChild(2).GetComponent<TextMeshProUGUI>().SetText($"{CData.précision}%");
            Text.GetChild(3).GetComponent<TextMeshProUGUI>().SetText($"{CData.critique}%");
            if (CData.MultipleAttack)
            {
                Parent.GetChild(5).GetComponent<TextMeshProUGUI>().SetText($"CD: {CData.cooldown-1}");

            }
            else
            {
                Parent.GetChild(5).GetComponent<TextMeshProUGUI>().SetText($"CD: {CData.cooldown}");

            }

        }
    }

    void ShowTargetIndicators(CapacityData capacity)
    {
        HideTargetIndicators();

        if (capacity.atk > 0 && !capacity.MultipleAttack)
        {
            for (int i = 0; i < entityHandler.ennemies.Count && i < circlesEnnemy.Count; i++)
            {
                DataEntity enemy = entityHandler.ennemies[i];
                if (entityHandler.ennemies.Count == 1)
                {
                    i = 1;
                }
                // Vérifie si l'ennemi est valide et vivant
                if (enemy == null || enemy.UnitLife <= 0)
                    continue;
                circlesEnnemy[i].SetActive(true);

                // Active le collider si le GameObject existe
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
            }
        }
        else if (capacity.heal > 0 && !capacity.MultipleHeal)
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
    
    public void GiveBuff(CapacityData capacity, DataEntity target)
    {
        // buffType; 1 = Atk, 2 = Def, 3 = Speed, 4 = Précision
        if (capacity.buffType > 0 && capacity.DoubleEffect == false)
        {
            ActiveBuff existingBuff = target.ActiveBuffs.Find(b => b.type == capacity.buffType);
            if (existingBuff != null)
            {
                existingBuff.value *= capacity.buffValue;
                existingBuff.duration = Mathf.Max(existingBuff.duration, capacity.buffDuration);
            }
            else
            {
                target.ActiveBuffs.Add(new ActiveBuff(capacity.buffType, capacity.buffValue, capacity.buffDuration));
            }

            RecalculateStats(target);
        }
        if (capacity.buffType > 0 && capacity.DoubleEffect == true)
        {
            ActiveBuff existingBuff = target.ActiveBuffs.Find(b => b.type == capacity.secondaryBuffType);
            if (existingBuff != null)
            {
                existingBuff.value *= capacity.buffValue;
                existingBuff.duration = Mathf.Max(existingBuff.duration, capacity.secondaryBuffDuration);
            }
            else
            {
                target.ActiveBuffs.Add(new ActiveBuff(capacity.secondaryBuffType, capacity.secondaryBuffValue, capacity.secondaryBuffDuration));
            }

            RecalculateStats(target);
        }
    }

    public void ShockProc(CapacityData capacity, DataEntity target)
    {
        DataEntity caster = currentTurnOrder[0];
        if (capacity.Shock > 0)
        {
            target.ShockMark += capacity.Shock;
            Debug.Log($"{caster.name} a appliqué {capacity.Shock} marque(s) à {target.namE}");
            if (target.ShockMark >= 1 && target.ShockMark <= 4)
            {
                int visualIndex = GetEntityVisualIndex(target);
                EffectsManager.SINGLETON.AfficherAttaqueFoudre(target.ShockMark, visualIndex);
            }
            if (target.ShockMark >= 4)
            {
                float calculatedDamage = (caster.UnitSpeed - 20) / 2;
                int icalculatedDamage = Mathf.RoundToInt(calculatedDamage);
                float fshieldDamage = calculatedDamage * 150 / 100;
                int ishieldDamage = Mathf.RoundToInt(fshieldDamage);
                if (target.UnitShield > 0)
                {
                    if (target.UnitShield < ishieldDamage)
                    {
                        ishieldDamage -= target.UnitShield;
                        target.UnitShield = 0;
                        Debug.Log($"{caster.namE} a brisé le shield de {target.namE} grâce au choc");
                    }
                    else
                    {
                        target.UnitShield -= ishieldDamage;
                        ishieldDamage = 0;
                        Debug.Log($"{caster.namE} inflige {ishieldDamage} dégâts au bouclier de {target.namE} grâce au choc");
                    }

                }
                if (ishieldDamage > 0)
                {
                    target.UnitLife -= icalculatedDamage;
                    Debug.Log($"{caster.namE} inflige {icalculatedDamage} dégâts à {target.namE} grâce au choc");
                }
                target.ShockMark = 0;

            }
        }
    }

    public void RageApplication(DataEntity target)
    {
        if (target.Affinity == 3)
        {
            bool isPlayer = entityHandler.players.Contains(target);
            List<DataEntity> team = isPlayer ? entityHandler.players : entityHandler.ennemies;
            int affinityCount = team.Count(entity => entity.Affinity == 3);
            target.RageTick += Mathf.Clamp(affinityCount + 1,0,12);
            RecalculateStats(target);
        }
    }
    
    public void RageProc(DataEntity caster)
    {
        if (caster.Affinity == 3 && caster.RageTick >= 12)
        {
            caster.UnitAtk = Mathf.RoundToInt(caster.UnitAtk * 1.5f);
            caster.RageTick = 0;
            RecalculateStats(caster);
        }

    }

    /*public void RageBoost(DataEntity target, int qteBoost = 1)
    {
        int oldTier = target.LastRageTick / 3;
        int newTier = target.RageTick / 3;

        if (oldTier != newTier)
        {
            int diff = (newTier - oldTier) * qteBoost;
            target.RageAtkBonus += diff;
            Debug.Log($"[RAGE] {target.namE} passe de palier {oldTier} à {newTier} → Bonus RageAtk = {target.RageAtkBonus}");
        }

        target.LastRageTick = target.RageTick;
    }*/

    public void ApplyNecrosis(DataEntity target, int levelToAdd = 1)
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
            float[] speedPercents = { 0f, 0.04f, 0.08f, 0.11f, 0.13f, 0.15f };
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

    public float lancer(int valeur, float above, float under)
    {
        int lancer = UnityEngine.Random.Range(0, 101);
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

        target.UnitAtk = Mathf.RoundToInt(target.BaseAtk * atkMultiplier) + target.RageTick / 3;
        target.UnitDef = Mathf.RoundToInt(target.BaseDef * defMultiplier);
        target.UnitSpeed = Mathf.RoundToInt(target.BaseSpeed * speedMultiplier);
        target.UnitAim = Mathf.RoundToInt(target.BaseAim * aimMultiplier);
    }
    public void DecrementBuffDurations(DataEntity target)
    {
        for (int i = target.ActiveBuffs.Count - 1; i >= 0; i--)
        {
            Debug.Log($"{target.ActiveBuffs[i]}");
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
            if (cd.capacity == capacity)
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
    
    public int GetEntityVisualIndex(DataEntity entity)
    {
        if (entity == null)
        {
            Debug.LogWarning("Tentative de récupérer l'index d'une entité null !");
            return -1;
        }
        for (int i = 0; i < entityHandler.players.Count; i++)
        {
            if (entityHandler.players[i] == entity)
            {
                return i;
            }
        }
        for (int i = 0; i < entityHandler.ennemies.Count; i++)
        {
            if (entityHandler.ennemies[i] == entity)
            {
                return entityHandler.players.Count + i;
            }
        }

        Debug.LogWarning($"Entité {entity.namE} non trouvée dans les listes !");
        return -1;
    }

}