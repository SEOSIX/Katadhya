using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EnemyPack
{
    public GameObject enemyPrefab1;
    public GameObject enemyPrefab2;
}
public class GameManager : MonoBehaviour
{
    public static GameManager SINGLETON { get; set; }
    
    [Header("Spawn Positions")]
    public List<Transform> playerSpawnPoints;
    public List<Transform> enemySpawnPoints; 
    [Header("Entity Handler")]
    [SerializeField] private EntityHandler entityHandler;

    [Header("Player Prefabs")]
    public List<PlayerPrefabData> playerPrefabs;

    [Header("Custom")] 
    [SerializeField] private float sizeChara;
    public int EnemyPackIndex = 0;
    public List<EnemyPack> enemyPacks = new List<EnemyPack>();
    public List<DataEntity> allEnemiesEncountered = new List<DataEntity>();




    public Dictionary<string, GameObject> prefabDictionary;

    private void Start()
    {
        if (entityHandler == null)
        {
            Debug.LogError("EntityHandler n'est pas assigné !");
            return;
        }
        entityHandler.ennemies.Clear();
        prefabDictionary = new Dictionary<string, GameObject>();
        foreach (var prefabData in playerPrefabs)
        {
            if (!prefabDictionary.ContainsKey(prefabData.playerName))
            {
                prefabDictionary.Add(prefabData.playerName, prefabData.prefab);
            }
        }

        SpawnPlayers();
        SpawnEnemies();
        CombatManager.SINGLETON.SetupBaseStat();
        CombatManager.SINGLETON.currentTurnOrder = CombatManager.SINGLETON.GetUnitTurn();
        CombatManager.SINGLETON.InitializeStaticUI();
        CombatManager.SINGLETON.StartUnitTurn();
    }
    void Awake()
    {
        if (SINGLETON != null)
        {
            Destroy(SINGLETON.gameObject); 
            return;
        }
        SINGLETON = this;
    }

    public void SpawnPlayers()
    {
        if (entityHandler.players == null || entityHandler.players.Count == 0)
        {
            Debug.LogError("Aucun joueur dans EntityHandler !");
            return;
        }

        for (int i = 0; i < entityHandler.players.Count; i++)
        {
            if (i >= playerSpawnPoints.Count)
            {
                Debug.LogWarning("Pas assez de points de spawn définis pour tous les joueurs !");
                break;
            }

            Transform spawnPoint = playerSpawnPoints[i];
            DataEntity data = entityHandler.players[i];

            if (!prefabDictionary.TryGetValue(data.namE, out GameObject prefab))
            {
                Debug.LogWarning($"Aucun prefab trouvé pour {data.namE}, joueur ignoré.");
                continue;
            }

            GameObject newPlayer = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            newPlayer.name = data.namE;
            data.instance = newPlayer;
            
            SpriteRenderer spriteRenderer = newPlayer.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = data.portrait;
                spriteRenderer.sortingOrder = 3;
                newPlayer.transform.localScale = new Vector3(data.size, data.size, data.size);
            }
        }
    }


   public void SpawnEnemies()
{
    if (enemyPacks[EnemyPackIndex] == null)
    {
        return;
    }
    entityHandler.ennemies.Clear();

    GameObject E1 = enemyPacks[EnemyPackIndex].enemyPrefab1;
    GameObject E2 = enemyPacks[EnemyPackIndex].enemyPrefab2;
    DataEntity[] allCapacityData = Resources.LoadAll<DataEntity>("Data/Entity/Ennemy");

    DataEntity EData1 = allCapacityData.FirstOrDefault(d => d.name == $"{E1.name}{EnemyPackIndex}");
    DataEntity EData2 = allCapacityData.FirstOrDefault(d => d.name == $"{E2.name}{EnemyPackIndex}");
    Debug.Log($"je v te toucher {E2} {EData2}");

    entityHandler.ennemies.Add(EData1);
    entityHandler.ennemies.Add(EData2);
    AddEnemyToEncountered(EData1);
    AddEnemyToEncountered(EData2);

    Slider[] ESlider = LifeEntity.SINGLETON.enemySliders;
    Slider[] EShieldSlider = LifeEntity.SINGLETON.enemyShieldSliders;

    for (int k = 0; k < ESlider.Length; k++)
    {
        ESlider[k].gameObject.SetActive(true);
        EShieldSlider[k].gameObject.SetActive(true);
    }

    if (entityHandler.ennemies == null || entityHandler.ennemies.Count == 0)
    {
        Debug.LogError("Aucun ennemi dans EntityHandler !");
        return;
    }

    for (int i = 0; i < entityHandler.ennemies.Count; i++)
    {
        if (i >= enemySpawnPoints.Count)
        {
            Debug.LogWarning("Pas assez de points de spawn définis pour tous les ennemis !");
            break;
        }

        Transform spawnPoint = enemySpawnPoints[i];
        DataEntity dataEnnemy = entityHandler.ennemies[i];
        GameObject prefab = (E1.name == dataEnnemy.namE) ? E1 : E2;

        GameObject newEnemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        newEnemy.name = dataEnnemy.namE;

        dataEnnemy.instance = newEnemy;
        VictoryDefeatUI.SINGLETON.RegisterEnemy(dataEnnemy);
        

        SpriteRenderer spriteRenderer = newEnemy.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = dataEnnemy.portrait;
            spriteRenderer.sortingOrder = 3;
            newEnemy.transform.localScale = new Vector3(sizeChara, sizeChara, sizeChara);
        }
    }

    CombatManager.SINGLETON.currentTurnOrder = CombatManager.SINGLETON.GetUnitTurn();
    CombatManager.SINGLETON.InitializeStaticUI();
    CombatManager.SINGLETON.StartUnitTurn();
}

    private void AddEnemyToEncountered(DataEntity data)
    {
        if (data != null && !allEnemiesEncountered.Contains(data))
        {
            allEnemiesEncountered.Add(data);
        }
    }
    public void RefreshAllIndices()
    {
        for (int i = 0; i < entityHandler.players.Count; i++)
        {
            entityHandler.players[i].index = i;
        }
        for (int i = 0; i < entityHandler.ennemies.Count; i++)
        {
            entityHandler.ennemies[i].index = entityHandler.players.Count + i;
        }
    }

}

[System.Serializable]
public class PlayerPrefabData
{
    public string playerName;
    public GameObject prefab;
}
