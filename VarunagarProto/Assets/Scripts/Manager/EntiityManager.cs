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
    
    [Header("entityHandler")]
    public EntityHandler entityHandler;


    private int currentLifeValue;
    public bool Clickable = true;

    private void UpdateIndexes()
    {
        DataEntity PrefabData = null;
        string name = gameObject.name;
        print(name);
        if (GameManager.SINGLETON.prefabDictionary.ContainsKey(name))
        {
            //GameObject prefab = GameManager.SINGLETON.prefabDictionary[name];
            DataEntity[] allCapacityData = Resources.LoadAll<DataEntity>("Data/Entity");
            PrefabData = allCapacityData.FirstOrDefault(d => d.name == $"{name}{GameManager.SINGLETON.EnemyPackIndex}");
        }
        if (PrefabData != null)
        {
            if (entityHandler.ennemies.Contains(PrefabData))
            {
                PrefabData.index = (entityHandler.players.Count() + entityHandler.ennemies.IndexOf(PrefabData));
                playerIndex = (entityHandler.players.Count() + entityHandler.ennemies.IndexOf(PrefabData));
            }
            if (entityHandler.players.Contains(PrefabData))
            {
                PrefabData.index = entityHandler.players.IndexOf(PrefabData);
                playerIndex = entityHandler.players.IndexOf(PrefabData);
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
            
            int visualIndex = CombatManager.SINGLETON.GetEntityVisualIndex(enemy);
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
            
            int visualIndex = CombatManager.SINGLETON.GetEntityVisualIndex(player);
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
        if (entityHandler.players.Count > 1)
        {
            UpdateSpellData(entityHandler.players[1]);
            UpdateSpellData(entityHandler.players[0]);
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
        }
    }

    void OnMouseDown()
    {
        if (!Clickable || GlobalVars.currentSelectedCapacity == null)
            return;
        Debug.Log($"Tentative de sélection. Index: {playerIndex}");


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

        CapacityData[] allData = Resources.LoadAll<CapacityData>("Data");
        player._CapacityData1 = allData.FirstOrDefault(d => d.name == $"Cpt{player.index}a{player.Affinity}");
        player._CapacityData2 = allData.FirstOrDefault(d => d.name == $"Cpt{player.index}b{player.Affinity}");
        player._CapacityData3 = allData.FirstOrDefault(d => d.name == $"Cpt{player.index}c{player.Affinity}");
        player._CapacityDataUltimate = allData.FirstOrDefault(d => d.name == $"Cpt{player.index}d{player.Affinity}");
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
