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
    public int playerIndex;
    
    [Header("entityHandler")]
    public EntityHandler entityHandler;


    private int currentLifeValue;
    public bool Clickable = true;

    public void DestroyDeadEnemies()
    {
        for (int i = entityHandler.ennemies.Count - 1; i >= 0; i--)
        {
            var enemy = entityHandler.ennemies[i];
            if (enemy == null || enemy.UnitLife > 0)
                continue;
            SINGLETON.RemoveUnitFromList(enemy);
            if (i < SINGLETON.circlesEnnemy.Count)
            {
                SINGLETON.circlesEnnemy[i].SetActive(false);
            }

            if (i < LifeEntity.SINGLETON.enemySliders.Length)
            {
                LifeEntity.SINGLETON.enemySliders[i].gameObject.SetActive(false);
            }
            if (i < LifeEntity.SINGLETON.enemyShieldSliders.Length)
            {
                LifeEntity.SINGLETON.enemyShieldSliders[i].gameObject.SetActive(false);
            }

            if (enemy.instance != null)
            {
                enemy.instance.SetActive(false);
                enemy.UnitLife = -1;
            }
        }
    }
    public void DestroyDeadPlayers()
    {
        for (int i = entityHandler.players.Count - 1; i >= 0; i--)
        {
            var player = entityHandler.players[i];
            if (player == null || player.UnitLife > 0)
                continue;

            Debug.Log($"Le joueur {player.namE} est mort et va être détruit.");

            GameObject playerInstance = player.instance;
            SINGLETON.RemoveUnitFromList(player);

            if (playerInstance != null)
                Destroy(playerInstance);

            if (i < LifeEntity.SINGLETON.PlayerSliders.Length)
            {
                LifeEntity.SINGLETON.PlayerSliders[i].gameObject.SetActive(false);
            }

            entityHandler.players.RemoveAt(i);
        }
        AssignPlayerIndices();
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
        if (entityHandler.ennemies.Any(e => e != null &&  e.UnitLife <= 0))
            DestroyDeadEnemies();

        if (entityHandler.players.Any(p => p != null && p.UnitLife <= 0))
            DestroyDeadPlayers();

        LifeEntity.SINGLETON.LifeManage();

        bool anyPlayerAlive = entityHandler.players.Any(p => p.UnitLife <= 0);
        bool anyEnemyAlive = entityHandler.ennemies.Any(e => e.UnitLife <= 0);

        if (anyPlayerAlive)
        {
            Debug.Log("Les ennemis ont gagné !");
        }
        if (anyEnemyAlive)
        {
            LoadingScene.SINGLETON.LoadNextSceneAsync();
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
            if (playerIndex < entityHandler.ennemies.Count && entityHandler.ennemies[playerIndex] != null)
                SINGLETON.SelectEnemy(playerIndex);
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
                    manager.playerIndex = i;
                }
                else
                {
                    Debug.LogWarning($"Aucun EntiityManager trouvé sur {enemy.namE}");
                }
            }
        }
    }
}
