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
            }

            if (enemy.instance != null)
            {
                enemy.instance.SetActive(false);
                enemy.UnitLife = -1;
                CombatManager.SINGLETON.ennemyDead++;
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
            CombatManager.SINGLETON.SetupBaseStat();
            CombatManager.SINGLETON.currentTurnOrder = CombatManager.SINGLETON.GetUnitTurn();
            CombatManager.SINGLETON.unitPlayedThisTurn.Clear();
            CombatManager.SINGLETON.StartUnitTurn();
            CombatManager.SINGLETON.ennemyDead = 0;
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
            int index = entityHandler.players.IndexOf(entityHandler.players.FirstOrDefault(p => p.instance == this.gameObject));
            if (index != -1) SINGLETON.SelectAlly(index);
        }
        else
        {
            int index = entityHandler.ennemies.IndexOf(entityHandler.ennemies.FirstOrDefault(e => e.instance == this.gameObject));
            if (index != -1) SINGLETON.SelectEnemy(entityHandler.players.Count + index);
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
}
