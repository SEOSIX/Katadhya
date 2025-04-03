using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class AI : MonoBehaviour
{
    public static AI SINGLETON { get; private set; }

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
    
    
    public void Attack(DataEntity attacker, int damages)
    {
        if (CombatManager.SINGLETON == null || 
            CombatManager.SINGLETON.entityHandler == null || 
            CombatManager.SINGLETON.entityHandler.players == null || 
            CombatManager.SINGLETON.entityHandler.players.Length == 0)
        {
            Debug.LogWarning("Aucun joueur disponible pour l'attaque.");
            return;
        }

        int randomIndex = Random.Range(0, CombatManager.SINGLETON.entityHandler.players.Length);
        DataEntity targetedPlayer = CombatManager.SINGLETON.entityHandler.players[randomIndex];
        
        Debug.Log($"{attacker.namE} prépare une attaque contre {targetedPlayer.namE} (HP: {targetedPlayer.UnitLife}/{targetedPlayer.BaseLife})");

        StartCoroutine(Attacking(attacker, targetedPlayer, damages));
    }
    
    IEnumerator Attacking(DataEntity attacker ,DataEntity target, int damage)
    {
        yield return new WaitForSeconds(2f);
        target.UnitLife -= damage;
        target.UnitLife = Mathf.Max(0, target.UnitLife);
        
        Debug.Log($"{attacker.namE} a infligé {damage} dégâts à {target.namE} (PV restants: {target.UnitLife})");
        if (target.UnitLife <= 0)
        {
            Debug.Log($"{target.namE} a été vaincu !");
            //Logique qui fait disparaitre ou fait partir le joueur
        }

        CombatManager.SINGLETON.EndUnitTurn();
    }
}
