using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public static CombatManager SINGLETON { get; private set; }

    [Header("Entity Handler")]
    [SerializeField]
    public EntityHandler entityHandler;

    [Header("Entity UI")]
    public TextMeshProUGUI textplayer;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI atck;
    public TextMeshProUGUI def;
    public TextMeshProUGUI lifeText;
    public Image playerPortrait;
    public Image[] ImagePortrait;

    public Button[] capacityButtons;
    private DataEntity currentPlayer;

    [Header("Turn Management")]
    public Button endTurnButton;
    public Slider LifePlayers;

    [Header("TurnObject")]
    public GameObject ennemyTurn;
    public GameObject TurnUI;

    [Header("Target Indicators")]
    public GameObject[] circlesEnnemy;
    public GameObject[] circlesPlayer;

    [Header("Ultimate")]
    public Ultimate ultimateScript;

    [HideInInspector] public List<DataEntity> currentTurnOrder = new List<DataEntity>();
    [HideInInspector] public List<DataEntity> unitPlayedThisTurn = new List<DataEntity>();
    private System.Random r = new System.Random();

    private CapacityData currentSelectedCapacity;
    public bool isEnnemyTurn;

    void Awake()
    {
        if (SINGLETON != null)
        {
            Debug.LogError("Trying to instantiate another CombatManager SINGLETON");
            Destroy(gameObject);
            return;
        }
        SINGLETON = this;
    }

    void Start()
    {
        currentTurnOrder = GetUnitTurn();
        InitializeStaticUI();
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
                ImagePortrait[i].transform.parent.gameObject.SetActive(true);
            }
            else
            {
                ImagePortrait[i].enabled = false;
                ImagePortrait[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public List<DataEntity> GetUnitTurn()
    {
        var speedValue = new List<DataEntity>();
        speedValue.AddRange(entityHandler.ennemies);
        speedValue.AddRange(entityHandler.players);
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
        StartUnitTurn();
    }

    public void EndGlobalTurn()
    {
        currentTurnOrder.AddRange(unitPlayedThisTurn);
        unitPlayedThisTurn.Clear();
    }

    public void StartUnitTurn()
    {
        DetectEnnemyTurn();

        if (!entityHandler.ennemies.Contains(currentTurnOrder[0]))
        {
            currentPlayer = currentTurnOrder[0];
            SetupCapacityButtons(currentPlayer);
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
        if (entityHandler.ennemies.Contains(currentEntity))
        {
            isEnnemyTurn = true;
            Debug.Log($"🔴 C'est au tour de l'ennemi {currentEntity.namE} !");
            TurnUI.SetActive(false);
            ennemyTurn.SetActive(true);
            AI.SINGLETON.Attack(currentEntity, 10);
        }
        else
        {
            isEnnemyTurn = false;
            TurnUI.SetActive(true);
            ennemyTurn.SetActive(false);
        }
    }

    public void UseCapacity(CapacityData cpt)
    {
        StartTargetSelectionMode(cpt);
    }

    public void StartTargetSelectionMode(CapacityData capacity)
    {
        HideTargetIndicators();
        currentSelectedCapacity = capacity;
        if (capacity.heal > 0)
        {
            foreach (var ally in entityHandler.players)
            {
                if (ally.UnitLife > 0)
                {
                    ApplyCapacityToTarget(capacity, ally);
                }
            }

            currentSelectedCapacity = null;
            Debug.Log("Capacité de soin appliquée à tous les alliés !");
            return;
        }
        Debug.Log("Sélectionnez une ou plusieurs cibles pour " + capacity.name);
        ShowTargetIndicators(capacity);
    }

    public void SelectEnemy(int enemyIndex)
    {
        if (currentSelectedCapacity == null)
        {
            Debug.Log("Aucune capacité sélectionnée !");
            return;
        }

        if (enemyIndex < 0 || enemyIndex >= entityHandler.ennemies.Length)
        {
            Debug.LogError("Index d'ennemi invalide !");
            return;
        }

        DataEntity target = entityHandler.ennemies[enemyIndex];

        ApplyCapacityToTarget(currentSelectedCapacity, target);
        currentSelectedCapacity = null;
        HideTargetIndicators();
        EndUnitTurn();
    }

    public void SelectAlly(int allyIndex)
    {
        if (currentSelectedCapacity == null)
        {
            Debug.Log("Aucune capacité sélectionnée !");
            return;
        }

        if (allyIndex < 0 || allyIndex >= entityHandler.players.Length)
        {
            Debug.LogError("Index d'allié invalide !");
            return;
        }

        DataEntity target = entityHandler.players[allyIndex];

        ApplyCapacityToTarget(currentSelectedCapacity, target);
        currentSelectedCapacity = null;
        HideTargetIndicators();
        EndUnitTurn();
    }

    public void ApplyCapacityToTarget(CapacityData capacity, DataEntity target)
    {
        DataEntity caster = currentTurnOrder[0];

        if (capacity.atk > 0)
        {
            float calculatedDamage = (((float)caster.UnitAtk / 100) * capacity.atk) * 100 / (100 + 2 * target.UnitDef);
            int icalculatedDamage = Mathf.RoundToInt(calculatedDamage);
            target.UnitLife -= icalculatedDamage;
            Debug.Log($"{caster.namE} inflige {icalculatedDamage} dégâts à {target.namE}");
        }
        if (capacity.heal > 0)
        {
            int healAmount = Mathf.RoundToInt((caster.UnitAtk / 100f) * capacity.heal);
            target.UnitLife = Mathf.Min(target.UnitLife + healAmount, target.BaseLife);
            Debug.Log($"{caster.namE} soigne {target.namE} pour {healAmount} PV");
        }
    

        InitializeStaticUI();
    }

    private void SetupCapacityButtons(DataEntity player)
    {
        for (int i = 0; i < capacityButtons.Length; i++)
        {
            capacityButtons[i].gameObject.SetActive(false);
            capacityButtons[i].onClick.RemoveAllListeners();
        }

        if (player.capacity1 != null)
        {
            capacityButtons[0].gameObject.SetActive(true);
            capacityButtons[0].GetComponent<Image>().sprite = player.capacity1;
            capacityButtons[0].onClick.AddListener(() => UseCapacity(player._CapacityData1));
        }

        if (player.capacity2 != null)
        {
            capacityButtons[1].gameObject.SetActive(true);
            capacityButtons[1].GetComponent<Image>().sprite = player.capacity2;
            capacityButtons[1].onClick.AddListener(() => UseCapacity(player._CapacityData2));
        }

        if (player.capacity3 != null)
        {
            capacityButtons[2].gameObject.SetActive(true);
            capacityButtons[2].GetComponent<Image>().sprite = player.capacity3;
            capacityButtons[2].onClick.AddListener(() => UseCapacity(player._CapacityData3));
        }

        if (player.Ultimate != null)
        {
            capacityButtons[3].gameObject.SetActive(true);
            capacityButtons[3].GetComponent<Image>().sprite = player.Ultimate;
            capacityButtons[3].onClick.AddListener(() => ultimateScript.QTE_Start());
        }
    }

    void ShowTargetIndicators(CapacityData capacity)
    {
        if (capacity.atk > 0)
        {
            if (!capacity.CanHeal)
            {
                for (int i = 0; i < entityHandler.ennemies.Length && i < circlesEnnemy.Length; i++)
                {
                    if (entityHandler.ennemies[i].UnitLife > 0)
                        circlesEnnemy[i].SetActive(true);
                }   
            }
        }
        
        else if (capacity.heal > 0)
        {
            if (capacity.CanHeal)
            {
                for (int i = 0; i < entityHandler.players.Length && i < circlesPlayer.Length; i++)
                {
                    if (entityHandler.players[i].UnitLife > 0)
                        circlesPlayer[i].SetActive(true);
                }
            }
        }
    }

    void HideTargetIndicators()
    {
        foreach (var circle in circlesEnnemy)
        {
            circle.SetActive(false);
        }
        foreach (var circle in circlesPlayer)
        {
            circle.SetActive(false);
        }
    }
}