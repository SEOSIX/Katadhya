using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public static class ReseterData
{
    public static void ResetEnemies(EntityHandler handler)
    {
        foreach (var enemy in handler.ennemies)
        {
            ResetEntity(enemy);
            enemy.UnitLife = enemy.BaseLife;
        }
    }

    public static void ResetPlayersComplete(EntityHandler handler, EntiityManager entiityManager)
    {
        foreach (var player in handler.players)
        {
            ResetEntity(player);
            player.UltimateSlider = 100;
            player.Affinity = 0;
            player.UltLvlHit = 0;
            entiityManager.UpdateSpellData(player);
            player.AtkLevel = 0;
            player.DefLevel = 0;
            player.SpeedLevel = 0;
            player.LifeLevel = 0;

        }
    }

    public static void ResetPlayersBeforeCombat(EntityHandler handler, EntiityManager entiityManager)
    {
        foreach (var player in handler.players)
        {
            ResetEntity(player);
            player.UltimateSlider = 100;
            player.Affinity = 0;
            player.UltLvlHit = 0;
            entiityManager.UpdateSpellData(player);
        }
    }

    public static void ResetSinglePlayer(DataEntity player)
    {
        ResetEntity(player);
        player.UltimateSlider = 100;
        player.Affinity = 0;
        player.UltLvlHit = 0;
    }

    private static void ResetEntity(DataEntity entity)
    {
        //entity.UnitLife = entity.BaseLife;
        entity.UnitAtk = entity.BaseAtk;
        entity.UnitDef = entity.BaseDef;
        entity.UnitSpeed = entity.BaseSpeed;
        entity.UnitAim = entity.BaseAim;
        entity.ActiveBuffs.Clear();
        entity.ActiveCooldowns.Clear();
        entity.skipNextTurn = false;
        entity.delayedActions.Clear();
        entity.ShockMark = 0;
        entity.RageTick = 0;
        entity.LastRageTick = 0;
        entity.necrosis = null;
        entity.beenHurtThisTurn = false;
        entity.provoking = false;
        entity.provokingDuration = 0;
        entity.ChargePower = 0;
    }
}

public static class HealingPlayer
{
    public static void HealByPercent(DataEntity entity, float lifePercent)
    {
        lifePercent = Mathf.Clamp01(lifePercent);
        int healAmount = Mathf.FloorToInt(entity.BaseLife * lifePercent);
        entity.UnitLife = Mathf.Min(entity.BaseLife, entity.UnitLife + healAmount);
    }

    public static void HealAllPlayersByPercent(EntityHandler handler, float lifePercent)
    {
        foreach (var player in handler.players)
        {
            if (player != null)
            {
                HealByPercent(player, lifePercent);
            }
        }
    }
}