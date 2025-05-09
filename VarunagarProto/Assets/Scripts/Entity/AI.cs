using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using static UnityEngine.GraphicsBuffer;


public class AI : MonoBehaviour
{
    public static AI SINGLETON { get; private set; }

    public GameObject playerTarget1;
    public GameObject playerTarget2;
    
    private int enemyTurnCounter = 0;

    void Awake()
    {
        if (SINGLETON != null)
        {
            Destroy(SINGLETON.gameObject);
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

        enemyTurnCounter++;

        if (enemyTurnCounter % 2 == 0)
        {
            Debug.Log(" Attaque multiple !");
            AttackMultipleTargets(attacker, damages);
        }
        else
        {
            Debug.Log(" Attaque simple !");
            AttackSingleTarget(attacker, damages);
        }
    }

    
    private void AttackSingleTarget(DataEntity attacker, int damages)
    {
        StartCoroutine(SingleAttackCoroutine(attacker, damages));
    }

    private IEnumerator SingleAttackCoroutine(DataEntity attacker, int damages)
    {
        int randomIndex = Random.Range(0, CombatManager.SINGLETON.entityHandler.players.Count);
        DataEntity targetedPlayer = CombatManager.SINGLETON.entityHandler.players[randomIndex];

        yield return new WaitForSeconds(1.5f);
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
        CombatManager.SINGLETON.ApplyCapacityToTarget(Cpt, targetedPlayer);
        Animator anim = targetedPlayer.Animator.GetComponent<Animator>();
        if (anim != null)
        {
            Debug.Log(anim.isInitialized);
            Debug.Log(anim.isActiveAndEnabled);
            Debug.Log(anim.runtimeAnimatorController);
            anim.SetTrigger("TakeDamage");
        }
        CombatManager.SINGLETON.DecrementBuffDurations(attacker);
        if (attacker.beenHurtThisTurn == false && attacker.RageTick > 0)
        {
            attacker.RageTick -= 1;
        }
        attacker.beenHurtThisTurn = false;
        Debug.Log(attacker.necrosis?.Count);
        if (attacker.necrosis?.Count > 0)
        {
            CombatManager.SINGLETON.TickNecrosisEffect(attacker);
        }
        CombatManager.SINGLETON.RecalculateStats(attacker);

        yield return new WaitForSeconds(1.5f);
        if (anim != null)
        {
            anim.SetTrigger("idle");
        }
        CombatManager.SINGLETON.EndUnitTurn();

        playerTarget1.SetActive(false);
        playerTarget2.SetActive(false);
    }


    private void AttackMultipleTargets(DataEntity attacker, int damages)
    {
        StartCoroutine(MultiAttackCoroutine(attacker, damages));
    }

    private IEnumerator MultiAttackCoroutine(DataEntity attacker, int damages)
    {
        int numberOfTargets = Random.Range(1, CombatManager.SINGLETON.entityHandler.players.Count + 1);
        List<DataEntity> potentialTargets = new List<DataEntity>(CombatManager.SINGLETON.entityHandler.players);

        for (int i = 0; i < numberOfTargets; i++)
        {
            if (potentialTargets.Count == 0) break;

            int randomIndex = Random.Range(0, potentialTargets.Count);
            DataEntity targetedPlayer = potentialTargets[randomIndex];
            potentialTargets.RemoveAt(randomIndex);

            if (i == 0)
            {
                playerTarget1.SetActive(true);
                playerTarget2.SetActive(false);
            }
            else if (i == 1)
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
            CombatManager.SINGLETON.ApplyCapacityToTarget(Cpt, targetedPlayer);

            yield return new WaitForSeconds(0.8f);
        }

        CombatManager.SINGLETON.DecrementBuffDurations(attacker);
        if (attacker.beenHurtThisTurn == false && attacker.RageTick > 0)
        {
            attacker.RageTick -= 1;
        }
        attacker.beenHurtThisTurn = false;
        Debug.Log(attacker.necrosis?.Count);
        if (attacker.necrosis?.Count > 0)
        {
            CombatManager.SINGLETON.TickNecrosisEffect(attacker);
        }
        CombatManager.SINGLETON.RecalculateStats(attacker);

        yield return new WaitForSeconds(1.2f);

        CombatManager.SINGLETON.EndUnitTurn();

        playerTarget1.SetActive(false);
        playerTarget2.SetActive(false);
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