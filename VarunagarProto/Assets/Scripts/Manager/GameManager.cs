using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EnemyPack
{
    public GameObject enemyPrefab1;
    public GameObject enemyPrefab2;
}

[System.Serializable]
public class PlayerPrefabData
{
    public string playerName;
    public GameObject prefab;
}

public class GameManager : MonoBehaviour
{
    public static GameManager SINGLETON { get; private set; }

    [Header("Combat Settings")]
    public bool isCombatEnabled = true;

    [Header("Spawn Positions")]
    public List<Transform> playerSpawnPoints;
    public List<Transform> enemySpawnPoints;

    [Header("Entity Handler")]
    [SerializeField] private EntityHandler entityHandler;

    [Header("Player Prefabs")]
    public List<PlayerPrefabData> playerPrefabs;

    [Header("Enemy Packs")]
    public int EnemyPackIndex = 0;
    public List<EnemyPack> enemyPacks = new List<EnemyPack>();
    public List<DataEntity> allEnemiesEncountered = new List<DataEntity>();

    [Header("Parameters")]
    [SerializeField] private float sizeChara = 1f;

    [SerializeField] private Vector3 circleOffset;

    public Dictionary<string, GameObject> prefabDictionary;

    private void Awake()
    {
        if (SINGLETON != null)
        {
            Destroy(gameObject);
            return;
        }
        SINGLETON = this;
    }

    private void Start()
    {
        if (entityHandler == null)
        {
            return;
        }

        prefabDictionary = playerPrefabs.ToDictionary(p => p.playerName, p => p.prefab);
        entityHandler.ennemies.Clear();

        SpawnPlayers();

        if (isCombatEnabled)
        {
            CombatManager.SINGLETON.ResetPlayersBeforeCombat();
            SpawnEnemies();
            CombatManager.SINGLETON.ResetEnemies();
            CombatManager.SINGLETON.currentTurnOrder = CombatManager.SINGLETON.GetUnitTurn();
            CombatManager.SINGLETON.InitializeStaticUI();
            CombatManager.SINGLETON.StartUnitTurn();
        }
    }

  public void SpawnPlayers()
{
    if (entityHandler.players == null || entityHandler.players.Count == 0)
        return;
    List<GameObject> playerObjects = new List<GameObject>();
    List<Vector3> targetPositions = new List<Vector3>();
    for (int i = 0; i < entityHandler.players.Count && i < playerSpawnPoints.Count; i++)
    {
        DataEntity data = entityHandler.players[i];

        if (!prefabDictionary.TryGetValue(data.namE, out GameObject prefab))
            continue;

        GameObject playerObj = Instantiate(prefab, playerSpawnPoints[i].position, Quaternion.identity);
        playerObj.name = data.namE;
        data.instance = playerObj;

        SpriteRenderer renderer = playerObj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.sprite = data.portrait;
            renderer.sortingOrder = 3;
            playerObj.transform.localScale = Vector3.one * data.size;
        }

        playerObjects.Add(playerObj);
        targetPositions.Add(playerSpawnPoints[i].position);
    }
    StartingScene.MoveFromLeft(playerObjects.ToArray(), targetPositions.ToArray());
    foreach (var oldCircle in CombatManager.SINGLETON.circlesPlayer)
    {
        if (oldCircle != null)
            Object.Destroy(oldCircle);
    }
    CombatManager.SINGLETON.circlesPlayer.Clear();
    Slider[] healthSliders = LifeEntity.SINGLETON.PlayerSliders;

    for (int i = 0; i < entityHandler.players.Count; i++)
    {
        if (i >= healthSliders.Length || healthSliders[i] == null)
            continue;

        Camera uiCamera = Camera.main;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(healthSliders[i].transform.position);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            CombatManager.SINGLETON.circleParentUI,
            screenPos,
            uiCamera, 
            out localPoint
        );

        RectTransform newCircle = Object.Instantiate(
            CombatManager.SINGLETON.circlePrefab,
            CombatManager.SINGLETON.circleParentUI
        ).GetComponent<RectTransform>();

        newCircle.anchoredPosition = localPoint;

        CombatManager.SINGLETON.circlesPlayer.Add(newCircle.gameObject);
    }
}




    public void SpawnEnemies()
{
    if (enemyPacks == null || enemyPacks.Count <= EnemyPackIndex || enemyPacks[EnemyPackIndex] == null)
    {
        Debug.LogError("Aucun EnemyPack valide à l'index " + EnemyPackIndex);
        return;
    }
    entityHandler.ennemies.Clear();

    var pack = enemyPacks[EnemyPackIndex];
    GameObject E1 = pack.enemyPrefab1;
    GameObject E2 = pack.enemyPrefab2;

    DataEntity[] enemyDataArray = Resources.LoadAll<DataEntity>("Data/Entity/Ennemy");
    DataEntity data1 = enemyDataArray.FirstOrDefault(d => d.name == $"{E1.name}");
    DataEntity data2 = enemyDataArray.FirstOrDefault(d => d.name == $"{E2.name}");
        Debug.Log($"{data1.name} {data2.name}");

    if (data1 != null) entityHandler.ennemies.Add(data1);
    if (data2 != null) entityHandler.ennemies.Add(data2);

    AddEnemyToEncountered(data1);
    AddEnemyToEncountered(data2);

    Slider[] healthSliders = LifeEntity.SINGLETON.enemySliders;
    Slider[] shieldSliders = LifeEntity.SINGLETON.enemyShieldSliders;
    
    for (int i = 0; i < entityHandler.ennemies.Count && i < enemySpawnPoints.Count; i++)
    {
        DataEntity data = entityHandler.ennemies[i];
        GameObject prefab = (data.namE == E1.name) ? E1 : E2;
        GameObject enemyObj = Instantiate(prefab, enemySpawnPoints[i].position, Quaternion.identity);
        enemyObj.name = data.namE;
        data.instance = enemyObj;

        VictoryDefeatUI.SINGLETON.RegisterEnemy(data);

        SpriteRenderer renderer = enemyObj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.sprite = data.portrait;
            renderer.sortingOrder = 3;
            enemyObj.transform.localScale = Vector3.one * sizeChara;
        }

        if (i < healthSliders.Length) healthSliders[i].gameObject.SetActive(true);
        if (i < shieldSliders.Length) shieldSliders[i].gameObject.SetActive(true);
    }
    
    foreach (var oldCircle in CombatManager.SINGLETON.circlesEnnemy)
    {
        if (oldCircle != null)
            Object.Destroy(oldCircle);
    }
    CombatManager.SINGLETON.circlesEnnemy.Clear();
    
    for (int i = 0; i < CombatManager.SINGLETON.entityHandler.ennemies.Count; i++)
    {
        DataEntity enemy = CombatManager.SINGLETON.entityHandler.ennemies[i];

        if (enemy?.instance == null)
        {
            Debug.LogWarning($"L'ennemi à l'index {i} n'a pas d'instance !");
            continue;
        }

        Vector3 worldPos = enemy.instance.transform.position + new Vector3(-0.52f, 2f, 0);

        if (CombatManager.SINGLETON.originalCircleEnemysPositions.Count <= i)
        {
            CombatManager.SINGLETON.originalCircleEnemysPositions.Add(worldPos);
        }
        else
        {
            worldPos = CombatManager.SINGLETON.originalCircleEnemysPositions[i];
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            CombatManager.SINGLETON.circleParentUI,
            screenPos,
            Camera.main,
            out anchoredPos
        );

        RectTransform newCircle = Object.Instantiate(
            CombatManager.SINGLETON.circlePrefab,
            CombatManager.SINGLETON.circleParentUI
        ).GetComponent<RectTransform>();

        newCircle.anchoredPosition = anchoredPos;

        CombatManager.SINGLETON.circlesEnnemy.Add(newCircle.gameObject);
    }
}

    private void AddEnemyToEncountered(DataEntity data)
    {
        if (data != null && !allEnemiesEncountered.Contains(data))
        {
            allEnemiesEncountered.Add(data);
        }
    }
}
