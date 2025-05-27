
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
            Debug.LogError("EntityHandler n'est pas assigné !");
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
            Debug.LogError("Aucun joueur dans EntityHandler !");
            return;
        }

        List<GameObject> playerObjects = new List<GameObject>();
        List<Vector3> targetPositions = new List<Vector3>();

        for (int i = 0; i < entityHandler.players.Count && i < playerSpawnPoints.Count; i++)
        {
            DataEntity data = entityHandler.players[i];

            if (!prefabDictionary.TryGetValue(data.namE, out GameObject prefab))
            {
                Debug.LogWarning($"Aucun prefab trouvé pour {data.namE}, joueur ignoré.");
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
    // Vérification initiale des packs
    if (enemyPacks == null || enemyPacks.Count <= EnemyPackIndex || enemyPacks[EnemyPackIndex] == null)
    {
        Debug.LogError($"ERREUR: Pack ennemi {EnemyPackIndex} invalide");
        return;
    }

    var pack = enemyPacks[EnemyPackIndex];
    GameObject E1 = pack.enemyPrefab1;
    GameObject E2 = pack.enemyPrefab2;

    // Debug 1 - Liste des spawn points
    Debug.Log($"[SPAWN] Points disponibles ({enemySpawnPoints.Count}):");
    for (int i = 0; i < enemySpawnPoints.Count; i++)
    {
        Debug.Log($" - #{i}: {enemySpawnPoints[i]?.name} ({(enemySpawnPoints[i] != null ? "OK" : "NULL")})");
    }

    entityHandler.ennemies.Clear();

    // Chargement des données
    DataEntity[] enemyDataArray = Resources.LoadAll<DataEntity>("Data/Entity/Ennemy");
    DataEntity data1 = enemyDataArray.FirstOrDefault(d => d.name == E1.name);
    DataEntity data2 = enemyDataArray.FirstOrDefault(d => d.name == E2.name);

    // Debug 2 - Vérification DataEntity
    Debug.Log($"[DATA] E1: {data1?.name ?? "NULL"}, E2: {data2?.name ?? "NULL"}");

    if (data1 != null) entityHandler.ennemies.Add(data1);
    if (data2 != null) entityHandler.ennemies.Add(data2);

    // Debug 3 - Comptage final
    Debug.Log($"[SPAWN] {entityHandler.ennemies.Count} ennemis à instancier");

    for (int i = 0; i < entityHandler.ennemies.Count; i++)
    {
        // Vérification index/spawnpoint
        if (i >= enemySpawnPoints.Count)
        {
            Debug.LogError($"ERREUR: Index {i} > nombre de spawn points ({enemySpawnPoints.Count})");
            continue;
        }

        Transform spawnPoint = enemySpawnPoints[i];
        if (spawnPoint == null)
        {
            Debug.LogError($"ERREUR: SpawnPoint {i} est null");
            continue;
        }

        DataEntity data = entityHandler.ennemies[i];
        GameObject prefab = (data.namE == E1.name) ? E1 : E2;

        // Debug 4 - Instantiation
        Debug.Log($"[INSTANCE] Création #{i} ({data.namE}) sur {spawnPoint.name} ({spawnPoint.position})");

        GameObject enemyObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        enemyObj.name = data.namE;
        data.instance = enemyObj;

        // Configuration visuelle
        SpriteRenderer renderer = enemyObj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.sprite = data.portrait;
            renderer.sortingOrder = 3;
            enemyObj.transform.localScale = Vector3.one * sizeChara;
        }

        // Déplacement contrôlé
        Debug.Log($"[MOUVEMENT] #{i} vers {spawnPoint.position}");
        StartingScene.MoveFromRight(new GameObject[] { enemyObj }, new Vector3[] { spawnPoint.position });

        // UI
        if (i < LifeEntity.SINGLETON.enemySliders.Length) 
            LifeEntity.SINGLETON.enemySliders[i].gameObject.SetActive(true);
        if (i < LifeEntity.SINGLETON.enemyShieldSliders.Length) 
            LifeEntity.SINGLETON.enemyShieldSliders[i].gameObject.SetActive(true);

        VictoryDefeatUI.SINGLETON.RegisterEnemy(data);
    }

    // Gestion des cercles de combat (version compacte)
    foreach (var oldCircle in CombatManager.SINGLETON.circlesEnnemy.Where(c => c != null)) 
        Destroy(oldCircle);
    
    CombatManager.SINGLETON.circlesEnnemy.Clear();

    for (int i = 0; i < CombatManager.SINGLETON.entityHandler.ennemies.Count; i++)
    {
        DataEntity enemy = CombatManager.SINGLETON.entityHandler.ennemies[i];
        if (enemy?.instance == null) continue;

        Vector3 basePos = enemy.instance.transform.position + new Vector3(-0.52f, 2f, 0);
        Vector3 worldPos = CombatManager.SINGLETON.originalCircleEnemysPositions.Count > i ? 
                          CombatManager.SINGLETON.originalCircleEnemysPositions[i] : 
                          basePos;

        // Conversion position UI
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            CombatManager.SINGLETON.circleParentUI,
            screenPos,
            null,
            out Vector2 anchoredPos
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
