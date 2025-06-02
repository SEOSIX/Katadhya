using TMPro;
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
                GameObject deadCircle = CombatManager.SINGLETON.circlesEnnemy[i];
                if (deadCircle != null)
                {
                    Object.Destroy(deadCircle);
                    CombatManager.SINGLETON.circlesEnnemy.RemoveAt(i);
                }
            }

            if (i < LifeEntity.SINGLETON.enemySliders.Length)
            {
                LifeEntity.SINGLETON.enemySliders[i].gameObject.SetActive(false);
                LifeEntity.SINGLETON.enemyShieldSliders[i].gameObject.SetActive(false);
                LifeEntity.SINGLETON.enemyPVTexts[i].gameObject.SetActive(false);

            }

            if (enemy.instance != null)
            {
                enemy.instance.SetActive(false);
                enemy.UnitLife = -1;
            }
            entityHandler.ennemies.RemoveAt(i);

        }
    }
    public void DestroyDeadPlayers()
    {
        for (int i = entityHandler.players.Count - 1; i >= 0; i--)
        {
            var player = entityHandler.players[i];
            if (player == null || player.UnitLife > 0)
                continue;

            int visualIndex = player.index;
            EffectsManager.SINGLETON.ClearEffectsForEntity(visualIndex);

            Debug.Log($"Le joueur {player.namE} est mort et va être désactivé.");
            CombatManager.SINGLETON.RemoveUnitFromList(player);

            if (i < CombatManager.SINGLETON.circlesPlayer.Count)
            {
                GameObject deadCircle = CombatManager.SINGLETON.circlesPlayer[i];
                if (deadCircle != null)
                {
                    Object.Destroy(deadCircle);
                    CombatManager.SINGLETON.circlesPlayer.RemoveAt(i);
                }
            }

            if (i < LifeEntity.SINGLETON.PlayerSliders.Length)
            {
                LifeEntity.SINGLETON.PlayerSliders[i].gameObject.SetActive(false);
                LifeEntity.SINGLETON.PlayerShieldSliders[i].gameObject.SetActive(false);
                LifeEntity.SINGLETON.PlayerPVTexts[i].gameObject.SetActive(false);

            }

            if (player.instance != null)
            {
                player.instance.SetActive(false);
                player.UnitLife = -1;
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
        bool isLastWave = GameManager.SINGLETON.EnemyPackIndex >= GameManager.SINGLETON.currentCombat.Combat.Count - 1;

        if (!anyPlayerAlive)
        {
            Debug.Log("Game Over");
        }
        else if (!anyEnemyAlive)
        {
            if (GameManager.SINGLETON.EnemyPackIndex+1 < GameManager.SINGLETON.currentCombat.Combat.Count)
            {
                GameManager.SINGLETON.EnemyPackIndex += 1;
                CombatManager.SINGLETON.entityHandler.ennemies.Clear();
                GameManager.SINGLETON.SpawnEnemies();
                ReseterData.ResetEnemies(entityHandler);
                CombatManager.SINGLETON.currentTurnOrder = CombatManager.SINGLETON.GetUnitTurn();
                CombatManager.SINGLETON.unitPlayedThisTurn.Clear();
                CombatManager.SINGLETON.StartUnitTurn();
            }
            else
            {
                isLastWave = true;
            }
     
        }
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
        CapacityData[] allData = Resources.LoadAll<CapacityData>("Data/Entity/Capacity");
        player._CapacityData1 = allData.FirstOrDefault(data =>
            data.name == $"Cpt{player.ID}a{player.Affinity}{player.Cpt1lvl}");

        player._CapacityData2 = allData.FirstOrDefault(data =>
            data.name == $"Cpt{player.ID}b{player.Affinity}{player.Cpt2lvl}");

        player._CapacityData3 = allData.FirstOrDefault(data =>
            data.name == $"Cpt{player.ID}c{player.Affinity}{player.Cpt3lvl}");

        player._CapacityDataUltimate = allData.FirstOrDefault(data =>
            data.name == $"Cpt{player.ID}d{player.Affinity}{player.UltLvlHit}");
    }
}
