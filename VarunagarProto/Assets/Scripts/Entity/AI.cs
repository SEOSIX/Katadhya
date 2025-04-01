using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class AI : MonoBehaviour
{
    public static AI SINGLETON { get; private set; }

    
    /*
    private void Start()
    {
        for (int i = 0; i < CombatManager.SINGLETON.entityHandler.players.Length && i < LifeEntity.SINGLETON.PlayerSliders.Length; i++)
        {
            LifeEntity.SINGLETON.PlayerSliders[i].maxValue = CombatManager.SINGLETON.entityHandler.players[i].BaseLife;
            LifeEntity.SINGLETON.PlayerSliders[i].value = LifeEntity.SINGLETON.PlayerSliders[i].maxValue;
        }
    }
    */

    void Awake()
    {
        if (SINGLETON != null)
        {
            Debug.LogError("Trying to instantiate another CombatManager SINGLETON");
            Destroy(gameObject);
            return;
        }
        SINGLETON = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Attack1(int damagesHeal)
    {
        if (CombatManager.SINGLETON.entityHandler == null || CombatManager.SINGLETON.entityHandler.players == null)
        {
            Debug.LogError("EntityHandler ou players non référé !");
            return;
        }
        List<DataEntity> alivePlayers = new List<DataEntity>();

        if (alivePlayers.Count == 0)
        {
            Debug.Log("Tous les joueurs sont morts !");
            CombatManager.SINGLETON.EndUnitTurn();
            return;
        }
        int randomIndex = Random.Range(0, alivePlayers.Count);
        DataEntity targetPlayer = alivePlayers[randomIndex];
        
        targetPlayer.UnitLife -= damagesHeal;
        LifeEntity.SINGLETON.PlayerSliders[randomIndex].value = CombatManager.SINGLETON.entityHandler.players[randomIndex].UnitLife;
        Debug.Log($"{CombatManager.SINGLETON.currentTurnOrder[0].namE} attaque {targetPlayer.namE} et inflige {damagesHeal} dégâts !");
        
        CombatManager.SINGLETON.EndUnitTurn();
    }
}
