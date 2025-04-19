using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class AI : MonoBehaviour
{
    public static AI SINGLETON { get; private set; }

    public GameObject playerTarget1;
    public GameObject playerTarget2;

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

        if (randomIndex == 1)
        {
            playerTarget1.SetActive(true);  
            playerTarget2.SetActive(false);   
        }
        else if (randomIndex == 0)
        {
            playerTarget1.SetActive(false); 
            playerTarget2.SetActive(true);   
        }
        else
        {
            playerTarget1.SetActive(false); 
            playerTarget2.SetActive(false);   
        }
        
        Debug.Log($"{attacker.namE} prépare une attaque contre {targetedPlayer.namE} (HP: {targetedPlayer.UnitLife}/{targetedPlayer.BaseLife})");

        StartCoroutine(Attacking(attacker, targetedPlayer, damages));
    }
    
    IEnumerator Attacking(DataEntity attacker ,DataEntity target, int damage)
    {
        Animation animationTarget1 = playerTarget1.GetComponent<Animation>();
        Animation animationTarget2 = playerTarget2.GetComponent<Animation>();
        
        int randomIndex = Random.Range(0, CombatManager.SINGLETON.entityHandler.players.Length);
        
        //Debug.Log($"{attacker.namE} a infligé {damage} dégâts à {target.namE} (PV restants: {target.UnitLife})");
        yield return new WaitForSeconds(2f);
        if (randomIndex == 1)
        {
            animationTarget2.Play();
        }
        else if (randomIndex == 0)
        {
            animationTarget1.Play();
        }
        target.UnitLife -= damage;
        if (target.UnitLife <= 0)
        {
            Debug.Log($"{target.namE} a été vaincu !");
        }
        yield return new WaitForSeconds(1f);
        CombatManager.SINGLETON.EndUnitTurn();
        
        //retire le target du dernier player
        if (!CombatManager.SINGLETON.isEnnemyTurn)
        {
            playerTarget1.SetActive(false); 
            playerTarget2.SetActive(false);   
        }
    }
}