using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager SINGLETON { get; private set; }
    
    [Header("Spawn Positions")]
    public List<Transform> playerSpawnPoints;
    public List<Transform> enemySpawnPoints; 
    [Header("Entity Handler")]
    [SerializeField] private EntityHandler entityHandler;

    [Header("Player Prefabs")]
    public List<PlayerPrefabData> playerPrefabs;

    [Header("Custom")] 
    [SerializeField] private float sizeChara;

    private Dictionary<string, GameObject> prefabDictionary;

    private void Start()
    {
        if (entityHandler == null)
        {
            Debug.LogError("EntityHandler n'est pas assigné !");
            return;
        }

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
    }
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

    void SpawnPlayers()
    {
        if (entityHandler.players == null || entityHandler.players.Length == 0)
        {
            Debug.LogError("Aucun joueur dans EntityHandler !");
            return;
        }

        for (int i = 0; i < entityHandler.players.Length; i++)
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

            SpriteRenderer spriteRenderer = newPlayer.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = data.portrait;
                spriteRenderer.sortingOrder = 3;
                newPlayer.transform.localScale = new Vector3(sizeChara, sizeChara, sizeChara);
            }
        }
    }

    void SpawnEnemies()
    {
        if (entityHandler.ennemies == null || entityHandler.ennemies.Length == 0)
        {
            Debug.LogError("Aucun ennemi dans EntityHandler !");
            return;
        }

        for (int i = 0; i < entityHandler.ennemies.Length; i++)
        {
            if (i >= enemySpawnPoints.Count)
            {
                Debug.LogWarning("Pas assez de points de spawn définis pour tous les ennemis !");
                break;
            }

            Transform spawnPoint = enemySpawnPoints[i];
            DataEntity dataEnnemy = entityHandler.ennemies[i];

            if (!prefabDictionary.TryGetValue(dataEnnemy.namE, out GameObject prefab))
            {
                Debug.LogWarning($"Aucun prefab trouvé pour {dataEnnemy.namE}, ennemi ignoré.");
                continue;
            }

            GameObject newEnemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            newEnemy.name = dataEnnemy.namE;

            
            dataEnnemy.instance = newEnemy;
            SpriteRenderer spriteRenderer = newEnemy.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = dataEnnemy.portrait;
                spriteRenderer.sortingOrder = 3;
                newEnemy.transform.localScale = new Vector3(sizeChara, sizeChara, sizeChara);
            }
        }
    }
}

[System.Serializable]
public class PlayerPrefabData
{
    public string playerName;
    public GameObject prefab;
}
