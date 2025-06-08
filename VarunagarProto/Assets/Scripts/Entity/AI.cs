using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AI : MonoBehaviour
{
    public static AI SINGLETON { get; private set; }
    
    public GameObject playerTarget1;
    public GameObject playerTarget2;
    public GameObject playerTarget3;
    
    private int enemyTurnCounter = 0;
    
    [HideInInspector]
    public CapacityData choosenSpell;

    private void Awake()
    {
        if (SINGLETON != null)
        {
            Destroy(SINGLETON.gameObject);
            return;
        }
        SINGLETON = this;
    }

    public void Attack(DataEntity attacker, int damages)
    {
        if (!IsCombatReady()) return;

        enemyTurnCounter++;
        StartCoroutine(SingleAttackCoroutine(attacker));
    }

    private bool IsCombatReady()
    {
        return CombatManager.SINGLETON != null &&
               CombatManager.SINGLETON.entityHandler != null &&
               CombatManager.SINGLETON.entityHandler.players != null &&
               CombatManager.SINGLETON.entityHandler.players.Count > 0;
    }
    
    private IEnumerator SingleAttackCoroutine(DataEntity attacker)
    {
        var entityHandler = CombatManager.SINGLETON.entityHandler;

        List<DataEntity> possibleTargets;

        if (choosenSpell.TargetingAlly)
        {
            possibleTargets = entityHandler.ennemies
                .Where(e => e.UnitLife > 0 && e.UnitLife < e.BaseLife)
                .ToList();
        }
        else
        {
            possibleTargets = entityHandler.players
                .Where(p => p.UnitLife > 0)
                .ToList();

            var provokers = possibleTargets.Where(p => p.provoking).ToList();
            if (provokers.Count > 0)
                possibleTargets = provokers;
        }

        if (possibleTargets.Count == 0)
        {
            Debug.Log("Aucune cible valide trouvée.");
            CombatManager.SINGLETON.EndUnitTurn();
            yield break;
        }

        DataEntity target = possibleTargets[Random.Range(0, possibleTargets.Count)];

        //Delais entre selction spells et attaque
        yield return new WaitForSeconds(2f);
        
        Debug.Log($"skibidi {attacker.namE} attaque");

        if (choosenSpell.MultipleAttack)
        {
            Debug.Log("[AI] Attaque multiple déclenchée !");
            foreach (var t in possibleTargets)
            {
                StartCoroutine(CombatManager.SINGLETON.ApplyCapacityToTarget(choosenSpell, t));
            }
        }
        else
        {
            StartCoroutine(CombatManager.SINGLETON.ApplyCapacityToTarget(choosenSpell, target));
        }

        yield return new WaitForSeconds(0.2f);
        CombatManager.SINGLETON.EndUnitTurn();
        DisableTargetIndicators();
    }



    private IEnumerator MultiAttackCoroutine(DataEntity attacker)
    {
        var targets = CombatManager.SINGLETON.entityHandler.players.Where(p => p.UnitLife > 0).ToList();
        int count = Random.Range(1, targets.Count + 1);
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < count; i++)
        {
            if (targets.Count == 0) break;

            List<DataEntity> provokers = targets.Where(p => p.provoking).ToList();
            DataEntity target = provokers.Count > 0
                ? provokers[Random.Range(0, provokers.Count)]
                : targets[Random.Range(0, targets.Count)];

            CapacityData chosenSpell = SelectSpell(attacker);
            StartCoroutine(CombatManager.SINGLETON.ApplyCapacityToTarget(chosenSpell, target));

        }
        //PostAttackProcessing(attacker);
        yield return new WaitForSeconds(0.2f);
        CombatManager.SINGLETON.EndUnitTurn();

        DisableTargetIndicators();
    }

    /*private void PostAttackProcessing(DataEntity attacker)
    {
        //CombatManager.SINGLETON.DecrementBuffDurations(attacker);

        if (!attacker.beenHurtThisTurn && attacker.RageTick > 0)
        {
            attacker.RageTick--;
        }

        attacker.beenHurtThisTurn = false;

        if (attacker.necrosis?.Count > 0)
        {
            CombatManager.SINGLETON.TickNecrosisEffect(attacker);
        }

        //CombatManager.SINGLETON.RecalculateStats(attacker);
    }*/

    private void ToggleTargetIndicator(DataEntity target)
    {
        int index = CombatManager.SINGLETON.entityHandler.players.IndexOf(target);
        playerTarget1.SetActive(index == 1);
        playerTarget2.SetActive(index == 0);
        playerTarget3.SetActive(index == 2);
    }

    private void DisableTargetIndicators()
    {
        playerTarget1.SetActive(false);
        playerTarget2.SetActive(false);
        playerTarget3.SetActive(false);
    }

    public CapacityData SelectSpell(DataEntity enemy)
    {
        int value1 = enemy._CapacityData1?.ValueAI ?? 0;
        int value2 = enemy._CapacityData2?.ValueAI ?? 0;
        int total = value1 + value2;

        CapacityData chosen = null;
        if (total == 0 || enemy._CapacityData1 == null || enemy._CapacityData2 == null)
            chosen = enemy._CapacityData1 ?? enemy._CapacityData2;
        else
            chosen = Random.Range(0, total) < value1 ? enemy._CapacityData1 : enemy._CapacityData2;

        Debug.Log($"[AI] Choix de la capacit� : {chosen.name}, TargetingAlly = {chosen.TargetingAlly}");

        return chosen;
    }
    public CapacityData SelectSpellFromList(List<CapacityData> spells)
    {
        if (spells == null || spells.Count == 0)
            return null;

        int totalWeight = spells.Sum(s => s.ValueAI);

        if (totalWeight == 0)
        {
            return spells[Random.Range(0, spells.Count)];
        }

        int randomValue = Random.Range(0, totalWeight);
        int cumulative = 0;

        foreach (var spell in spells)
        {
            cumulative += spell.ValueAI;
            if (randomValue < cumulative)
                return spell;
        }

        return spells[0]; 
    }


}
