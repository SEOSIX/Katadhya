using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public static CombatManager SINGLETON { get; private set; }

    public EntiityManager entityManager;

    [Header("Entity Handler")]
    [SerializeField]
    public EntityHandler entityHandler;

    [Header("Entity UI")]
    public TextMeshProUGUI textplayer;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI atck;
    public TextMeshProUGUI def;
    public Image playerPortrait;
    public Image[] ImagePortrait; // Ces portraits restent fixes maintenant
    public Image capacity1CM;
    public Image capacity2CM;
    public Image capacity3CM;
    public Image UltimateCM;
    
    public Button[] capacityButtons;
    private DataEntity currentPlayer;

    [Header("Turn Management")]
    public Button endTurnButton;
    public Slider LifePlayers;

    [Header("TurnObject")]
    public GameObject ennemyTurn;
    public GameObject TurnUI;

    [Header("Selected One")] 
    public GameObject[] circles;

    [HideInInspector] public List<DataEntity> currentTurnOrder = new List<DataEntity>();
    [HideInInspector] public List<DataEntity> unitPlayedThisTurn = new List<DataEntity>();
    private System.Random r = new System.Random();
    
    private int selectedEnemyIndex = -1;
    private bool isEnnemyTurn = false;

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
    
    private void InitializeStaticUI()
    {
        if (currentTurnOrder == null) return;
        
        DataEntity currentEntity = currentTurnOrder[0];
        
        textplayer.text = currentEntity.name;
        speed.text = "Speed :" + currentEntity.UnitSpeed;
        def.text = "Defence :" + currentEntity.UnitDef;
        atck.text = "Attack :" + currentEntity.UnitAtk;
        playerPortrait.sprite = currentEntity.bandeauUI;
        LifePlayers.maxValue = currentEntity.BaseLife;
        LifePlayers.value = currentEntity.UnitLife;
        
        int i = 0;
        foreach (var enemy in entityHandler.ennemies)
        {
            if (i >= ImagePortrait.Length) break;
            ImagePortrait[i].enabled = true;
            ImagePortrait[i].sprite = enemy.portraitUI;
            i++;
        }

        foreach (var player in entityHandler.players)
        {
            if (i >= ImagePortrait.Length) break;
            ImagePortrait[i].enabled = true;
            ImagePortrait[i].sprite = player.portraitUI;
            i++;
        }

    }

    public List<DataEntity> GetUnitTurn()
    {
        List<DataEntity> speedValue = new List<DataEntity>();
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

    public void StartUnitTurn()
    {
        DetectEnnemyTurn();
        
        if (!entityHandler.ennemies.Contains(currentTurnOrder[0]))
        {
            currentPlayer = currentTurnOrder[0];
            SetupCapacityButtons(currentPlayer);
        }
    }

    public void EndGlobalTurn()
    {
        currentTurnOrder.AddRange(unitPlayedThisTurn);
        unitPlayedThisTurn.Clear();
    }

    public void RemoveUnitFromList(DataEntity _currentUnit)
    {
        if (_currentUnit == null || !currentTurnOrder.Contains(_currentUnit)) return;

        currentTurnOrder.Remove(_currentUnit);
    }

    // UpdateUi() supprimé car l'UI ne change plus
    
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

    public void SelectEnemy(int enemyIndex)
    {
        if (enemyIndex < 0 || enemyIndex >= entityHandler.ennemies.Length) 
        {
            Debug.LogError("Index invalide pour la sélection d'ennemi !");
            return;
        }

        selectedEnemyIndex = enemyIndex;
        DataEntity selectedEnemy = entityHandler.ennemies[enemyIndex];
        
        foreach (var circle in circles)
        {
            circle.SetActive(false);
        }
        
        if (enemyIndex < circles.Length)
        {
            circles[enemyIndex].SetActive(true);
        }
        Debug.Log($"L'ennemi sélectionné est {selectedEnemy.namE}");
    }
    
    private void SetupCapacityButtons(DataEntity player)
    {
        foreach (var button in capacityButtons)
        {
            button.gameObject.SetActive(false);
        }
        
        if (player.capacity1 != null)
        {
            capacityButtons[0].gameObject.SetActive(true);
            capacityButtons[0].GetComponent<Image>().sprite = player.capacity1;
            capacityButtons[0].onClick.RemoveAllListeners();
            capacityButtons[0].onClick.AddListener(() => UseCapacity(player._CapacityData1));
        }

        if (player.capacity2 != null)
        {
            capacityButtons[1].gameObject.SetActive(true);
            capacityButtons[1].GetComponent<Image>().sprite = player.capacity2;
            capacityButtons[1].onClick.RemoveAllListeners();
            capacityButtons[1].onClick.AddListener(() => UseCapacity(player._CapacityData2));
        }
        
        if (player.capacity3 != null)
        {
            capacityButtons[2].gameObject.SetActive(true);
            capacityButtons[2].GetComponent<Image>().sprite = player.capacity3;
            capacityButtons[2].onClick.RemoveAllListeners();
            capacityButtons[2].onClick.AddListener(() => UseCapacity(player._CapacityData3));
        }

        if (player.Ultimate != null)
        {
            capacityButtons[3].gameObject.SetActive(true);
            capacityButtons[3].GetComponent<Image>().sprite = player.Ultimate;
            capacityButtons[3].onClick.RemoveAllListeners();
            capacityButtons[3].onClick.AddListener(() => UseCapacity(player._CapacityDataUltimate));
        }
    }

    public void UseCapacity(CapacityData cpt)
    {
        if (selectedEnemyIndex == -1)
        {
            Debug.Log("Aucun ennemi sélectionné !");
            return;
        }
        if (cpt.atk > 0)
        {
            AttackDamage(cpt);
        }
    }
    
    public void AttackDamage(CapacityData capacity)
    {
        if (selectedEnemyIndex == -1)
        {
            Debug.Log("Aucun ennemi sélectionné !");
            return;
        }

        DataEntity currentEntity = currentTurnOrder[0];
        DataEntity target = entityHandler.ennemies[selectedEnemyIndex];
        if (capacity.atk > 0)
        {
            int calculatedDamage = capacity.atk;
            target.UnitLife -= calculatedDamage;
            Debug.Log($"{currentEntity.namE} inflige {calculatedDamage} dégâts à {target.namE}");
        }
        InitializeStaticUI();
    }
}