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
            CombatManager.SINGLETON.entityHandler.players.Count == 0)
        {
            Debug.LogWarning("Aucun joueur disponible pour l'attaque.");
            return;
        }

        int randomIndex = Random.Range(0, CombatManager.SINGLETON.entityHandler.players.Count);
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
        CapacityData Cpt = SelectSpell(attacker);
        CombatManager.SINGLETON.ApplyCapacityToTarget(Cpt,targetedPlayer);
        CombatManager.SINGLETON.DecrementBuffDurations(attacker);
        StartCoroutine(Attacking(attacker, targetedPlayer, damages));
    }
    
    IEnumerator Attacking(DataEntity attacker ,DataEntity target, int damages)
    {
        Animation animationTarget1 = playerTarget1.GetComponent<Animation>();
        Animation animationTarget2 = playerTarget2.GetComponent<Animation>();

        yield return new WaitForSeconds(1f);

        int randomIndex = Random.Range(0, CombatManager.SINGLETON.entityHandler.players.Count);
        
        //Debug.Log($"{attacker.namE} a infligé {damage} dégâts à {target.namE} (PV restants: {target.UnitLife})");

        CombatManager.SINGLETON.EndUnitTurn();
        
        //retire le target du dernier player
        if (!CombatManager.SINGLETON.isEnnemyTurn)
        {
            playerTarget1.SetActive(false); 
            playerTarget2.SetActive(false);   
        }
    }

    public CapacityData SelectSpell(DataEntity enemy)
    {
        int Value1 = enemy._CapacityData1.ValueAI;
        int Value2 =enemy._CapacityData2.ValueAI;
        int randomInt = Random.Range(0,Value1+Value2);
        if (randomInt <=Value1)
        {
            return enemy._CapacityData1;
        }
        else 
        {
            return enemy._CapacityData2;
        }
    }
}