using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using JetBrains.Annotations;
using System.Linq;
using static CombatManager;
using System.IO.IsolatedStorage;
using Object = UnityEngine.Object;


public class EntiityManager : MonoBehaviour
{
    
    [Header("entityHandler")]
    public EntityHandler entityHandler;


    private int currentLifeValue;
    public bool Clickable = true;
    
    public void DestroyDeadEnemies()
    {
        enemiesAlreadyDestroyed.Clear();
        List<int> indicesToRemove = new List<int>();

        for (int i = entityHandler.ennemies.Count - 1; i >= 0; i--)
        {
            var enemy = entityHandler.ennemies[i];
            if (enemy == null || enemy.UnitLife > 0)
                continue;

            int visualIndex = enemy.index;
            EffectsManager.SINGLETON.ClearEffectsForEntity(visualIndex);
            CombatManager.SINGLETON.RemoveUnitFromList(enemy);

            if (i < CombatManager.SINGLETON.circlesEnnemy.Count)
            {
                GameObject deadCircle = CombatManager.SINGLETON.circlesEnnemy[i];
                if (deadCircle != null)
                {
                    indicesToRemove.Add(i);
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

            enemiesAlreadyDestroyed.Add(i);
        }

        for (int j = indicesToRemove.Count - 1; j >= 0; j--)
        {
            int index = indicesToRemove[j];
            if (index < CombatManager.SINGLETON.circlesEnnemy.Count)
            {
                GameObject deadCircle = CombatManager.SINGLETON.circlesEnnemy[index];
                if (deadCircle != null)
                {
                    Object.Destroy(deadCircle);
                    CombatManager.SINGLETON.circlesEnnemy[index] = null;
                    if (CombatManager.SINGLETON.circlesEnnemy[index] == null)
                    {
                        continue;
                    }
                }
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

    private HashSet<int> enemiesAlreadyDestroyed = new HashSet<int>();
    private HashSet<int> playersAlreadyDestroyed = new HashSet<int>();
    void Update()
    {
        for (int i = 0; i < entityHandler.ennemies.Count; i++)
        {
            var enemy = entityHandler.ennemies[i];
            if (enemy != null && enemy.UnitLife <= 0 && !enemiesAlreadyDestroyed.Contains(i))
            {
                DestroyDeadEnemies();
                break;
            }
        }

        for (int i = 0; i < entityHandler.ennemies.Count; i++)
        {
            var players = entityHandler.players[i];
            if (players != null && players.UnitLife <= 0 && !playersAlreadyDestroyed.Contains(i))
            {
                DestroyDeadPlayers();
                break; 
            }
        }

        LifeEntity.SINGLETON.LifeManage();

        bool anyPlayerAlive = entityHandler.players.Any(p => p != null && p.UnitLife > 0);
        bool anyEnemyAlive = entityHandler.ennemies.Any(e => e != null && e.UnitLife > 0 );
        bool isLastWave = GameManager.SINGLETON.EnemyPackIndex >= GameManager.SINGLETON.currentCombat.WaveList.Count - 1;

        if (!anyPlayerAlive)
        {
            Debug.Log("Game Over");
            CombatManager.SINGLETON.EndGlobalTurn();
            CombatManager.SINGLETON.EndGameOver.SetActive(true);
        }
        else if (!anyEnemyAlive)
        {
            if (GameManager.SINGLETON.EnemyPackIndex+1 < GameManager.SINGLETON.currentCombat.WaveList.Count)
            {
                entityHandler.ennemies.RemoveAll(e => e == null || e.UnitLife <= 0);
                GameManager.SINGLETON.EnemyPackIndex += 1;
                CombatManager.SINGLETON.entityHandler.ennemies.Clear();
                GameManager.SINGLETON.SpawnEnemies();
                LifeEntity.SINGLETON.LifeManage();
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

    /*void OnMouseDown()
    {
        if (!Clickable || GlobalVars.currentSelectedCapacity == null)
            return;
        DataEntity caster = CombatManager.currentTurnOrder[0];
        List<DataEntity> pool = null;
        if (entityHandler.players.Contains(caster))
        {
            pool = entityHandler.ennemies;
        }
        else
        {
            pool = entityHandler.players;
        }
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
    }*/
    void OnMouseDown()
    {
        if (!Clickable || GlobalVars.currentSelectedCapacity == null)
            return;

        DataEntity caster = CombatManager.SINGLETON.currentTurnOrder[0];
        List<DataEntity> allies = entityHandler.players.Contains(caster) ? entityHandler.players : entityHandler.ennemies;
        List<DataEntity> enemies = entityHandler.players.Contains(caster) ? entityHandler.ennemies : entityHandler.players;

        if (GlobalVars.currentSelectedCapacity.TargetingAlly)
        {
            int index = allies.FindIndex(p => p.instance == this.gameObject);
            if (index != -1)
            {
                CombatManager.SINGLETON.ClickedAPlayer = true;
                CapacityData Cpt = GlobalVars.currentSelectedCapacity;
                if(!Cpt.MultipleHeal || !Cpt.MultipleBuff) SINGLETON.SelectAlly(index);

            }
        }
        else
        {
            int index = enemies.FindIndex(e => e.instance == this.gameObject);
            if (index != -1)
            {
                CombatManager.SINGLETON.ClickedAnEnemy = true;
                CapacityData Cpt = GlobalVars.currentSelectedCapacity;
                if (!Cpt.MultipleAttack) SINGLETON.SelectEnemy(allies.Count + index);
            }
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
    
  private void OnMouseEnter()
{
    if (!Clickable || GlobalVars.currentSelectedCapacity == null)
        return;

    DataEntity caster = CombatManager.SINGLETON.currentTurnOrder[0];
    bool casterIsPlayer = entityHandler.players.Contains(caster);
    List<DataEntity> allies = casterIsPlayer ? entityHandler.players : entityHandler.ennemies;
    List<DataEntity> enemies = casterIsPlayer ? entityHandler.ennemies : entityHandler.players;

    CapacityData capacity = GlobalVars.currentSelectedCapacity;

    if (capacity.TargetingAlly)
    {
        if (capacity.MultipleHeal || capacity.MultipleBuff)
        {
            for (int i = 0; i < allies.Count && i < CombatManager.SINGLETON.circlesPlayer.Count; i++)
            {
                if (allies[i]?.UnitLife > 0)
                {
                    Image img = CombatManager.SINGLETON.circlesPlayer[i].GetComponent<Image>();
                    if (img != null && CombatManager.SINGLETON.hoverSprite != null)
                        img.sprite = CombatManager.SINGLETON.hoverSprite;
                }
            }
        }
        else
        {
            int index = allies.FindIndex(e => e.instance == this.gameObject);
            if (index != -1 && index < CombatManager.SINGLETON.circlesPlayer.Count)
            {
                Image img = CombatManager.SINGLETON.circlesPlayer[index].GetComponent<Image>();
                if (img != null && CombatManager.SINGLETON.hoverSprite != null)
                    img.sprite = CombatManager.SINGLETON.hoverSprite;
            }
        }
    }
    else
    {
        if (capacity.MultipleAttack)
        {
            for (int i = 0; i < enemies.Count && i < CombatManager.SINGLETON.circlesEnnemy.Count; i++)
            {
                if (enemies[i]?.UnitLife > 0)
                {
                    Image img = CombatManager.SINGLETON.circlesEnnemy[i].GetComponent<Image>();
                    if (img != null && CombatManager.SINGLETON.hoverSprite != null)
                        img.sprite = CombatManager.SINGLETON.hoverSprite;
                }
            }
        }
        else
        {
            int index = enemies.FindIndex(e => e.instance == this.gameObject);
            if (index != -1 && index < CombatManager.SINGLETON.circlesEnnemy.Count)
            {
                Image img = CombatManager.SINGLETON.circlesEnnemy[index].GetComponent<Image>();
                if (img != null && CombatManager.SINGLETON.hoverSprite != null)
                    img.sprite = CombatManager.SINGLETON.hoverSprite;
            }
        }
    }
}

private void OnMouseExit()
{
    if (!Clickable || GlobalVars.currentSelectedCapacity == null)
        return;

    DataEntity caster = CombatManager.SINGLETON.currentTurnOrder[0];
    bool casterIsPlayer = entityHandler.players.Contains(caster);
    List<DataEntity> allies = casterIsPlayer ? entityHandler.players : entityHandler.ennemies;
    List<DataEntity> enemies = casterIsPlayer ? entityHandler.ennemies : entityHandler.players;

    CapacityData capacity = GlobalVars.currentSelectedCapacity;

    if (capacity.TargetingAlly)
    {
        if (capacity.MultipleHeal || capacity.MultipleBuff)
        {
            for (int i = 0; i < allies.Count && i < CombatManager.SINGLETON.circlesPlayer.Count; i++)
            {
                Image img = CombatManager.SINGLETON.circlesPlayer[i].GetComponent<Image>();
                if (img != null && CombatManager.SINGLETON.defaultSprite != null)
                    img.sprite = CombatManager.SINGLETON.defaultSprite;
            }
        }
        else
        {
            for (int i = 0; i < allies.Count; i++)
            {
                if (allies[i]?.instance == this.gameObject && i < CombatManager.SINGLETON.circlesPlayer.Count)
                {
                    Image img = CombatManager.SINGLETON.circlesPlayer[i].GetComponent<Image>();
                    if (img != null && CombatManager.SINGLETON.defaultSprite != null)
                        img.sprite = CombatManager.SINGLETON.defaultSprite;
                    break;
                }
            }
        }
    }
    else
    {
        if (capacity.MultipleAttack)
        {
            for (int i = 0; i < enemies.Count && i < CombatManager.SINGLETON.circlesEnnemy.Count; i++)
            {
                Image img = CombatManager.SINGLETON.circlesEnnemy[i].GetComponent<Image>();
                if (img != null && CombatManager.SINGLETON.defaultSprite != null)
                    img.sprite = CombatManager.SINGLETON.defaultSprite;
            }
        }
        else
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i]?.instance == this.gameObject && i < CombatManager.SINGLETON.circlesEnnemy.Count)
                {
                    Image img = CombatManager.SINGLETON.circlesEnnemy[i].GetComponent<Image>();
                    if (img != null && CombatManager.SINGLETON.defaultSprite != null)
                        img.sprite = CombatManager.SINGLETON.defaultSprite;
                    break;
                }
            }
        }
    }
}


}
