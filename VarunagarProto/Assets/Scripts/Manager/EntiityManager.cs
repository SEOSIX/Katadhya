using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using JetBrains.Annotations;
using System.Linq;
using static CombatManager;
using System.IO.IsolatedStorage;


public class EntiityManager : MonoBehaviour
{
    public void Awake()
    {
        UpdateIndexes();
    }
    public int playerIndex;
    private int initialIndex;
    private bool hasStoredInitialIndex = false;
    
    [Header("entityHandler")]
    public EntityHandler entityHandler;


    private int currentLifeValue;
    public bool Clickable = true;

    public void UpdateIndexes()
    {
        DataEntity PrefabData = null;
        string name = gameObject.name;
        //print(name);
        if (GameManager.SINGLETON.prefabDictionary.ContainsKey(name))
        {
            DataEntity[] allCapacityData = Resources.LoadAll<DataEntity>("Data/Entity");
            PrefabData = allCapacityData.FirstOrDefault(d => d.name == $"{name}{GameManager.SINGLETON.EnemyPackIndex}");
        }
        if (PrefabData != null)
        {
            int computedIndex = -1;

            if (entityHandler.ennemies.Contains(PrefabData))
            {
                computedIndex = entityHandler.players.Count + entityHandler.ennemies.IndexOf(PrefabData);
            }
            else if (entityHandler.players.Contains(PrefabData))
            {
                computedIndex = entityHandler.players.IndexOf(PrefabData);
            }

            if (computedIndex != -1)
            {
                PrefabData.index = computedIndex;
                playerIndex = computedIndex;
                if (!hasStoredInitialIndex)
                {
                    initialIndex = playerIndex;
                    hasStoredInitialIndex = true;
                }
            }
        }
    }
    
    public void DestroyDeadEnemies()
    {
        for (int i = entityHandler.ennemies.Count - 1; i >= 0; i--)
        {
            var enemy = entityHandler.ennemies[i];
            if (enemy == null || enemy.UnitLife > 0)
                continue;

            int visualIndex = enemy.index;
            EffectsManager.SINGLETON.ClearEffectsForEntity(visualIndex);

            Debug.Log($"L'ennemi {enemy.namE} est mort et va être désactivé.");
            CombatManager.SINGLETON.RemoveUnitFromList(enemy);

            if (i < CombatManager.SINGLETON.circlesEnnemy.Count)
            {
                CombatManager.SINGLETON.circlesEnnemy[i].SetActive(false);
            }

            if (i < LifeEntity.SINGLETON.enemySliders.Length)
            {
                LifeEntity.SINGLETON.enemySliders[i].gameObject.SetActive(false);
                LifeEntity.SINGLETON.enemyShieldSliders[i].gameObject.SetActive(false);
                UpdateIndexes();
            }

            if (enemy.instance != null)
            {
                enemy.instance.SetActive(false);
                enemy.UnitLife = -1;
                if (enemy.index == 0)
                {
                    CombatManager.SINGLETON.circlesEnnemy[1] = CombatManager.SINGLETON.circlesEnnemy[0];
                }
                if (enemy.index == 1)
                {
                    CombatManager.SINGLETON.circlesEnnemy[0] = CombatManager.SINGLETON.circlesEnnemy[1];
                }
            }
            entityHandler.ennemies.RemoveAt(i);
            UpdateIndexes();
        }
    }
    public void DestroyDeadPlayers()
    {
        for (int i = entityHandler.ennemies.Count - 1; i >= 0; i--)
        {
            var player = entityHandler.players[i];
            if (player == null || player.UnitLife > 0)
                continue;

            int visualIndex = player.index;
            EffectsManager.SINGLETON.ClearEffectsForEntity(visualIndex);

            Debug.Log($"L'ennemi {player.namE} est mort et va être désactivé.");
            CombatManager.SINGLETON.RemoveUnitFromList(player);

            if (i < CombatManager.SINGLETON.circlesPlayer.Count)
            {
                CombatManager.SINGLETON.circlesPlayer[i].SetActive(false);
            }

            if (i < LifeEntity.SINGLETON.PlayerSliders.Length)
            {
                LifeEntity.SINGLETON.PlayerSliders[i].gameObject.SetActive(false);
                LifeEntity.SINGLETON.PlayerShieldSliders[i].gameObject.SetActive(false);
                UpdateIndexes();
            }
            if (player.instance != null)
            {
                player.instance.SetActive(false);
                player.UnitLife = -1;
                if (player.index == 0)
                {
                    CombatManager.SINGLETON.circlesPlayer[1] = CombatManager.SINGLETON.circlesPlayer[0];
                }
            }
            entityHandler.players.RemoveAt(i);
            UpdateIndexes();
        }
    }

    private void RestoreEnemiesLife()
    {
        foreach (var enemy in entityHandler.ennemies)
        {
            if (enemy != null)
                enemy.UnitLife = enemy.BaseLife;
        }
    }

    private void RestoreShield()
    {
        foreach (var enemy in entityHandler.ennemies)
        {
            if (enemy != null)
                enemy.UnitShield = 0;
        }

        foreach (var player in entityHandler.players)
        {
            if (player != null)
                player.UnitShield = 0;
        }
    }

    void Start()
    {
        foreach(var player in entityHandler.players.Where(p => p != null))
        {
            UpdateSpellData(player);
        }

        LifeEntity.SINGLETON.LifeManage();
        RestoreEnemiesLife();
        RestoreShield();
        AssignPlayerIndices();
    }

    void Update()
    {
        if (entityHandler.ennemies.Any(e => e != null && e.UnitLife <= 0))
            DestroyDeadEnemies();

        if (entityHandler.players.Any(p => p != null && p.UnitLife <= 0))
            DestroyDeadPlayers();

        LifeEntity.SINGLETON.LifeManage();

        bool anyPlayerAlive = entityHandler.players.Any(p => p != null);
        bool anyEnemyAlive = entityHandler.ennemies.Any(e => e != null);

        if (!anyPlayerAlive)
        {
            Debug.Log("Les ennemis ont gagné !");
        }
        else if (!anyEnemyAlive)
        {
            Debug.Log("Les joueurs ont gagné !");
            GameManager.SINGLETON.EnemyPackIndex += 1;
            GameManager.SINGLETON.SpawnEnemies();
            playerIndex = initialIndex;
        }
        bool isLastWave = GameManager.SINGLETON.EnemyPackIndex >= GameManager.SINGLETON.enemyPacks.Count - 1;
        bool isDefeat = !anyPlayerAlive;
        bool isVictory = anyPlayerAlive && !anyEnemyAlive && isLastWave;

        if (isVictory)
        {
            VictoryDefeatUI.SINGLETON.DisplayEndCombat(isVictory, GameManager.SINGLETON.allEnemiesEncountered);
        }
    }

    void OnMouseDown()
    {
        if (!Clickable || GlobalVars.currentSelectedCapacity == null)
            return;
        //Debug.Log($"Tentative de sélection. Index: {playerIndex}");


        if (GlobalVars.currentSelectedCapacity.TargetingAlly)
        {
            if (playerIndex < entityHandler.players.Count && entityHandler.players[playerIndex] != null)
                SINGLETON.SelectAlly(playerIndex);
        }
        else
        {
            if (playerIndex <= entityHandler.ennemies.Count+entityHandler.players.Count)
            {
                SINGLETON.SelectEnemy(playerIndex);
            }
            else
            {
                Debug.Log("ntm");
            }
        }
    }

    public void UpdateSpellData(DataEntity player)
    {
        //if(player._CapacityData1 != null && player._CapacityData2 != null && player._CapacityData3 != null && player._CapacityDataUltimate) 
            //return;
        CapacityData[] allData = Resources.LoadAll<CapacityData>("Data/Entity/Capacity");
        player._CapacityData1 = allData.FirstOrDefault(d => d.name == $"Cpt{player.ID}a{player.Affinity}");
        player._CapacityData2 = allData.FirstOrDefault(d => d.name == $"Cpt{player.ID}b{player.Affinity}");
        player._CapacityData3 = allData.FirstOrDefault(d => d.name == $"Cpt{player.ID}c{player.Affinity}");
        player._CapacityDataUltimate = allData.FirstOrDefault(d => d.name == $"Cpt{player.ID}d{player.Affinity}{player.UltLvlHit}");
    }

    private void AssignPlayerIndices()
    {
        for (int i = 0; i < entityHandler.players.Count; i++)
        {
            var player = entityHandler.players[i];
            if (player?.instance != null)
            {
                EntiityManager manager = player.instance.GetComponent<EntiityManager>();
                if (manager != null)
                {
                    manager.playerIndex = i;
                }
                else
                {
                    Debug.LogWarning($"Aucun EntiityManager trouvé sur {player.namE}");
                }
            }
        }

        for (int i = 0; i < entityHandler.ennemies.Count; i++)
        {
            var enemy = entityHandler.ennemies[i];
            if (enemy?.instance != null)
            {
                EntiityManager manager = enemy.instance.GetComponent<EntiityManager>();
                if (manager != null)
                {
                    manager.playerIndex = (entityHandler.players.Count() + i); ;
                }
                else
                {
                    Debug.LogWarning($"Aucun EntiityManager trouvé sur {enemy.namE}");
                }
            }
        }
    }
}
