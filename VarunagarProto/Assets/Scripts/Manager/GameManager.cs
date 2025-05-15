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
            Debug.LogError("EntityHandler n'est pas assigné !");
            return;
        }

        prefabDictionary = playerPrefabs.ToDictionary(p => p.playerName, p => p.prefab);
        entityHandler.ennemies.Clear();

        SpawnPlayers();

        if (isCombatEnabled)
        {
            SpawnEnemies();

            UpdateAllEntityIndexes();

            CombatManager.SINGLETON.SetupBaseStat();
            CombatManager.SINGLETON.currentTurnOrder = CombatManager.SINGLETON.GetUnitTurn();
            CombatManager.SINGLETON.InitializeStaticUI();
            CombatManager.SINGLETON.StartUnitTurn();
        }
        else
        {
            UpdateAllEntityIndexes();
        }
    }

    public void SpawnPlayers()
    {
        if (entityHandler.players == null || entityHandler.players.Count == 0)
        {
            Debug.LogError("Aucun joueur dans EntityHandler !");
            return;
        }

        for (int i = 0; i < entityHandler.players.Count && i < playerSpawnPoints.Count; i++)
        {
            DataEntity data = entityHandler.players[i];

            if (!prefabDictionary.TryGetValue(data.namE, out GameObject prefab))
            {
                Debug.LogWarning($"Aucun prefab trouvé pour {data.namE}, joueur ignoré.");
                continue;
            }

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

        DataEntity data1 = enemyDataArray.FirstOrDefault(d => d.name == $"{E1.name}{EnemyPackIndex}");
        DataEntity data2 = enemyDataArray.FirstOrDefault(d => d.name == $"{E2.name}{EnemyPackIndex}");

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
    }

    private void AddEnemyToEncountered(DataEntity data)
    {
        if (data != null && !allEnemiesEncountered.Contains(data))
        {
            allEnemiesEncountered.Add(data);
        }
    }

    private void UpdateAllEntityIndexes()
    {
        for (int i = 0; i < entityHandler.players.Count; i++)
        {
            var manager = entityHandler.players[i]?.instance?.GetComponent<EntiityManager>();
            if (manager != null) manager.UpdateIndexes();
        }

        for (int i = 0; i < entityHandler.ennemies.Count; i++)
        {
            var manager = entityHandler.ennemies[i]?.instance?.GetComponent<EntiityManager>();
            if (manager != null) manager.UpdateIndexes();
        }
    }
}
