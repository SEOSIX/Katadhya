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
        {
            return;
        }

        List<GameObject> playerObjects = new List<GameObject>();
        List<Vector3> targetPositions = new List<Vector3>();

        for (int i = 0; i < entityHandler.players.Count && i < playerSpawnPoints.Count; i++)
        {
            DataEntity data = entityHandler.players[i];

            if (!prefabDictionary.TryGetValue(data.namE, out GameObject prefab))
            {
                continue;
            }
            GameObject playerObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
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
    }

    public void SpawnEnemies()
    {
        if (enemyPacks == null || enemyPacks.Count <= EnemyPackIndex || enemyPacks[EnemyPackIndex] == null)
    {
        return;
    }

    var pack = enemyPacks[EnemyPackIndex];
    GameObject E1 = pack.enemyPrefab1;
    GameObject E2 = pack.enemyPrefab2;

    entityHandler.ennemies.Clear();

    DataEntity[] enemyDataArray = Resources.LoadAll<DataEntity>("Data/Entity/Ennemy");
    DataEntity data1 = enemyDataArray.FirstOrDefault(d => d.name == E1.name);
    DataEntity data2 = enemyDataArray.FirstOrDefault(d => d.name == E2.name);

    if (data1 != null) entityHandler.ennemies.Add(data1);
    if (data2 != null) entityHandler.ennemies.Add(data2);

    List<GameObject> enemyObjects = new List<GameObject>();
    List<Vector3> targetPositions = new List<Vector3>();

    for (int i = 0; i < entityHandler.ennemies.Count; i++)
    {
        DataEntity data = entityHandler.ennemies[i];
        GameObject prefab = (data.namE == E1.name) ? E1 : E2;

        Vector3 startPos = new Vector3(5f, 0, 0); 
        GameObject enemyObj = Instantiate(prefab, startPos, Quaternion.identity);
        enemyObj.name = data.namE;
        data.instance = enemyObj;

        SpriteRenderer renderer = enemyObj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.sprite = data.portrait;
            renderer.sortingOrder = 3;
            enemyObj.transform.localScale = Vector3.one * sizeChara;
        }

        enemyObjects.Add(enemyObj);

        if (i < enemySpawnPoints.Count && enemySpawnPoints[i] != null)
        {
            targetPositions.Add(enemySpawnPoints[i].position);
        }
        else
        {
            targetPositions.Add(startPos);
        }

        if (i < LifeEntity.SINGLETON.enemySliders.Length)
            LifeEntity.SINGLETON.enemySliders[i].gameObject.SetActive(true);
        if (i < LifeEntity.SINGLETON.enemyShieldSliders.Length)
            LifeEntity.SINGLETON.enemyShieldSliders[i].gameObject.SetActive(true);

        VictoryDefeatUI.SINGLETON.RegisterEnemy(data);

            if (i < LifeEntity.SINGLETON.enemySliders.Length) 
                LifeEntity.SINGLETON.enemySliders[i].gameObject.SetActive(true);
            if (i < LifeEntity.SINGLETON.enemyShieldSliders.Length) 
                LifeEntity.SINGLETON.enemyShieldSliders[i].gameObject.SetActive(true);

            VictoryDefeatUI.SINGLETON.RegisterEnemy(data);
        }
        foreach (var oldCircle in CombatManager.SINGLETON.circlesEnnemy.Where(c => c != null))
            Destroy(oldCircle);

        CombatManager.SINGLETON.circlesEnnemy.Clear();
        
        for (int i = 0; i < LifeEntity.SINGLETON.enemySliders.Length; i++)
        {
            if (i >= CombatManager.SINGLETON.entityHandler.ennemies.Count)
                break;

            if (!LifeEntity.SINGLETON.enemySliders[i].gameObject.activeSelf)
                continue;

            RectTransform sliderRect = LifeEntity.SINGLETON.enemySliders[i].GetComponent<RectTransform>();

            RectTransform newCircle = Object.Instantiate(
                CombatManager.SINGLETON.circlePrefab,
                CombatManager.SINGLETON.circleParentUI
            ).GetComponent<RectTransform>();

            if (sliderRect.parent == CombatManager.SINGLETON.circleParentUI)
            {
                newCircle.anchoredPosition = sliderRect.anchoredPosition;
            }
            else
            {
                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, sliderRect.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    CombatManager.SINGLETON.circleParentUI,
                    screenPos,
                    null,
                    out Vector2 localPoint
                );
                newCircle.anchoredPosition = localPoint;
            }
            CombatManager.SINGLETON.circlesEnnemy.Add(newCircle.gameObject);
        }
        
        StartingScene.MoveFromRight(enemyObjects.ToArray(), targetPositions.ToArray());
    }

    private void AddEnemyToEncountered(DataEntity data)
    {
        if (data != null && !allEnemiesEncountered.Contains(data))
        {
            allEnemiesEncountered.Add(data);
        }
    }
}
