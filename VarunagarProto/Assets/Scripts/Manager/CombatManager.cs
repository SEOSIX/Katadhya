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
    public TextMeshProUGUI def;
    public Image playerPortrait;
    public Image[] ImagePortrait;
    public Image capacity1CM;
    public Image capacity2CM;
    public Image capacity3CM;
    public Image UltimateCM;

    [Header("Turn Management")]
    public Button endTurnButton;

    public Slider LifePlayers;

    
    [Header("TurnObject")]
    public GameObject ennemyTurn;
    public GameObject TurnUI;

    [Header("Selected One")] 
    public GameObject[] circles;

    [HideInInspector]public List<DataEntity> currentTurnOrder = new List<DataEntity>();
    [HideInInspector]public List<DataEntity> unitPlayedThisTurn = new List<DataEntity>();
    private System.Random r = new System.Random();
    
    private int selectedEnemyIndex = -1;

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
        StartUnitTurn();
    }
    

    private List<DataEntity> GetUnitTurn()
    {
        List<DataEntity> speedValue = new List<DataEntity>();
        speedValue.AddRange(entityHandler.ennemies);
        speedValue.AddRange(entityHandler.players);
        return speedValue.OrderBy(x => x.UnitSpeed).ToList();
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

    private void StartUnitTurn()
    {
        UpdateUi();
        DetectEnnemyTurn();
    }

    private void EndGlobalTurn()
    {
        currentTurnOrder.AddRange(unitPlayedThisTurn);
        unitPlayedThisTurn.Clear();
    }

    public void RemoveUnitFromList(DataEntity _currentUnit)
    {
        if (_currentUnit == null || !currentTurnOrder.Contains(_currentUnit)) return;

        currentTurnOrder.Remove(_currentUnit);
        UpdateUi();
    }

    private void UpdateUi()
    {
        if (currentTurnOrder.Count == 0) return;

        DataEntity currentEntity = currentTurnOrder[0];
        
        textplayer.text = currentEntity.name;
        speed.text = "Speed :" + currentEntity.UnitSpeed;
        def.text = "Defence :" + currentEntity.UnitDef;
        playerPortrait.sprite = currentEntity.bandeauUI;
        capacity1CM.sprite = currentEntity.capacity1;
        capacity2CM.sprite = currentEntity.capacity2;
        capacity3CM.sprite = currentEntity.capacity3;
        UltimateCM.sprite = currentEntity.Ultimate;
        LifePlayers.maxValue = currentEntity.UnitLife;
        LifePlayers.value = LifePlayers.maxValue;
        
        
        for (int i = 0; i < ImagePortrait.Length; i++)
        {
            if (i < currentTurnOrder.Count)
            {
                ImagePortrait[i].enabled = true;
                ImagePortrait[i].sprite = currentTurnOrder[i].portraitUI;
            }
            else
            {
                ImagePortrait[i].enabled = false;
                ImagePortrait[i].sprite = null;
            }
        }
        Debug.Log($"{currentEntity.namE} à {LifePlayers.maxValue} points de vie");
    }
    
    private void DetectEnnemyTurn()
    {
        DataEntity currentEntity = currentTurnOrder[0];
        if (entityHandler.ennemies.Contains(currentEntity))
        {
            Debug.Log($"🔴 C'est au tour de l'ennemi {currentEntity.namE} !");
            TurnUI.SetActive(false);
            ennemyTurn.SetActive(true);
            //Attaque de l'ennemy
            AI.SINGLETON.Attack(currentEntity, 10);
        }
        else
        {
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

    public void UseCapacity(CapacityData cpt)
    {
        if (selectedEnemyIndex == -1)
        {
            Debug.Log("Aucun ennemi sélectionné !");
            UseCapacity(cpt);
        }
        AttackDamage(cpt.atk);
    }
    
    public void AttackDamage(int damage)
    {
        DataEntity currentEntity = currentTurnOrder[0];
        int calculatedDamage = (((currentEntity.UnitAtk / 100) * damage) * 100 / (100 + 2 * entityHandler.ennemies[selectedEnemyIndex].UnitDef));
        entityHandler.ennemies[selectedEnemyIndex].UnitLife -= calculatedDamage;
        Debug.Log($"attaque {calculatedDamage}");
    }
}  