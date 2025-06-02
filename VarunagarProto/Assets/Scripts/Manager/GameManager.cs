using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EnemyPack
{
    [Header("Wave : ")]
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
    public bool salleSpeciale = false;

    [Header("Spawn Positions")]
    public List<Transform> playerSpawnPoints;
    public List<Transform> enemySpawnPoints;
    public List<RectTransform> objectsToSpawn;

    [Header("Entity Handler")]
    public EntityHandler entityHandler;

    [Header("Player Prefabs")]
    public List<PlayerPrefabData> playerPrefabs;

    [Header("Enemy Packs")]
    public int EnemyPackIndex = 0;
    public CombatEncounters currentCombat;
    public List<DataEntity> allEnemiesEncountered = new List<DataEntity>();

    [Header("Parameters")]
    [SerializeField] private float sizeChara = 1f;
    
    
    public bool ispressed;

    [SerializeField] private Vector3 circleOffset;

    public Dictionary<string, GameObject> prefabDictionary;
        
    private Dictionary<RectTransform, Vector3> originalPositions = new Dictionary<RectTransform, Vector3>();

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
            ReseterData.ResetPlayersBeforeCombat(entityHandler,CombatManager.SINGLETON.entiityManager);
            SpawnEnemies();
            ReseterData.ResetEnemies(entityHandler);
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

            Animator anim = playerObj.GetComponent<Animator>();
            if (anim != null)
                anim.applyRootMotion = false;

            playerObjects.Add(playerObj);
            targetPositions.Add(playerSpawnPoints[i].position);
        }

        StartingScene.MoveFromLeft(playerObjects.ToArray(), targetPositions.ToArray());

        if (CombatManager.SINGLETON == null || 
            CombatManager.SINGLETON.circleParentUI == null || 
            CombatManager.SINGLETON.circlePrefab == null || 
            CombatManager.SINGLETON.circlesPlayer == null)
        {
            return;
        }

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
        List<EnemyPack> enemyPacks = currentCombat.Combat;

        if (enemyPacks == null || enemyPacks.Count <= EnemyPackIndex || enemyPacks[EnemyPackIndex] == null)
        {
            Debug.LogError("Aucun EnemyPack valide à l'index " + EnemyPackIndex);
            return;
        }

        if (CombatManager.SINGLETON == null)
        {
            Debug.LogWarning("CombatManager.SINGLETON est null.");
            return;
        }

        entityHandler.ennemies.Clear();
        var pack = enemyPacks[EnemyPackIndex];
        GameObject E1 = pack.enemyPrefab1;
        GameObject E2 = pack.enemyPrefab2;

        DataEntity[] enemyDataArray = Resources.LoadAll<DataEntity>("Data/Entity/Ennemy");
        DataEntity data1 = enemyDataArray.FirstOrDefault(d => d.name == $"{E1.name}");
        DataEntity data2 = enemyDataArray.FirstOrDefault(d => d.name == $"{E2.name}");
        Debug.Log($"{data1?.name} {data2?.name}");

        if (data1 != null) entityHandler.ennemies.Add(data1);
        if (data2 != null) entityHandler.ennemies.Add(data2);

        AddEnemyToEncountered(data1);
        AddEnemyToEncountered(data2);

        Slider[] healthSliders = LifeEntity.SINGLETON.enemySliders;
        Slider[] shieldSliders = LifeEntity.SINGLETON.enemyShieldSliders;
        TextMeshProUGUI[] PVTexts = LifeEntity.SINGLETON.enemyPVTexts;

        List<GameObject> enemyObjects = new List<GameObject>();
        List<Vector3> enemyTargetPositions = new List<Vector3>();

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
            if (i < PVTexts.Length) PVTexts[i].gameObject.SetActive(true);

            Animator anim = enemyObj.GetComponent<Animator>();
            if (anim != null)
                anim.applyRootMotion = false;

            enemyObjects.Add(enemyObj);
            enemyTargetPositions.Add(enemySpawnPoints[i].position);
        }

        StartingScene.MoveFromRight(enemyObjects.ToArray(), enemyTargetPositions.ToArray());

        if (CombatManager.SINGLETON.circlesEnnemy != null)
        {
            foreach (var oldCircle in CombatManager.SINGLETON.circlesEnnemy)
            {
                if (oldCircle != null) Object.Destroy(oldCircle);
            }
            CombatManager.SINGLETON.circlesEnnemy.Clear();
        }

        if (CombatManager.SINGLETON.circleParentUI == null || CombatManager.SINGLETON.circlePrefab == null)
        {
            Debug.LogWarning("UI des cercles ennemis manquante.");
            return;
        }

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
                CombatManager.SINGLETON.originalCircleEnemysPositions.Add(worldPos);
            else
                worldPos = CombatManager.SINGLETON.originalCircleEnemysPositions[i];

            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                CombatManager.SINGLETON.circleParentUI,
                screenPos,
                Camera.main,
                out Vector2 anchoredPos))
            {
                RectTransform newCircle = Object.Instantiate(
                    CombatManager.SINGLETON.circlePrefab,
                    CombatManager.SINGLETON.circleParentUI
                ).GetComponent<RectTransform>();

                newCircle.anchoredPosition = anchoredPos;
                CombatManager.SINGLETON.circlesEnnemy.Add(newCircle.gameObject);
            }
        }
    }

    [SerializeField] private float offsetX = 10f;

    public void SpawnObjectFinal()
    {
        if (objectsToSpawn == null || objectsToSpawn.Count == 0)
        {
            return;
        }

        if (!ispressed)
        {
            MoveUIFromRight();
            ispressed = true;
        }
        else
        {
            MoveUIBackToStart();
            ispressed = false;
        }
    }
    
    private void MoveUIFromRight()
    {
        foreach (RectTransform rt in objectsToSpawn)
        {
            if (rt == null) continue;

            Vector3 targetPosition = rt.anchoredPosition3D;

            if (!originalPositions.ContainsKey(rt))
            {
                originalPositions[rt] = targetPosition;
            }

            Vector3 startPosition = targetPosition + new Vector3(offsetX, 0, 0);
            rt.anchoredPosition3D = startPosition;

            StartCoroutine(AnimateUIElement(rt, startPosition, targetPosition));
        }
    }

    private void MoveUIBackToStart()
    {
        foreach (RectTransform rt in objectsToSpawn)
        {
            if (rt == null || !originalPositions.ContainsKey(rt)) continue;

            Vector3 startPosition = rt.anchoredPosition3D;
            Vector3 targetPosition = originalPositions[rt];

            StartCoroutine(AnimateUIElement(rt, startPosition, targetPosition));
        }
    }
    
    private System.Collections.IEnumerator AnimateUIElement(RectTransform element, 
        Vector3 startPos, 
        Vector3 targetPos, 
        float duration = 0.5f)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            element.anchoredPosition3D = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        element.anchoredPosition3D = targetPos;
    }


    private void AddEnemyToEncountered(DataEntity data)
    {
        if (data != null && !allEnemiesEncountered.Contains(data))
        {
            allEnemiesEncountered.Add(data);
        }
    }
}
