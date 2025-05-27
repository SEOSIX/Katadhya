using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    public static AI SINGLETON { get; private set; }
    
    public GameObject playerTarget1;
    public GameObject playerTarget2;
    public GameObject playerTarget3;
    
    private int enemyTurnCounter = 0;

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
        /*if (enemyTurnCounter % 2 == 0)
        {
            Debug.Log("L'ennemi lance une attaque multiple");
            StartCoroutine(MultiAttackCoroutine(attacker));
        }
        else
        {
            Debug.Log("L'ennemi lance une attaque simple");
            StartCoroutine(SingleAttackCoroutine(attacker));
        }*/
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
        var targets = CombatManager.SINGLETON.entityHandler.players.Where(p => p.UnitLife > 0).ToList();
        if (targets.Count == 0) yield break;

        DataEntity target = targets.FirstOrDefault(p => p.provoking) ? targets[Random.Range(0, targets.Count)] : targets[Random.Range(0, targets.Count)];

        yield return new WaitForSeconds(0.5f);

        ToggleTargetIndicator(target);

        CapacityData chosenSpell = SelectSpell(attacker);
        CombatManager.SINGLETON.ApplyCapacityToTarget(chosenSpell, target);

        PostAttackProcessing(attacker);

        yield return new WaitForSeconds(0.5f);
        CombatManager.SINGLETON.EndUnitTurn();

        DisableTargetIndicators();
    }

    private IEnumerator MultiAttackCoroutine(DataEntity attacker)
    {
        var targets = CombatManager.SINGLETON.entityHandler.players.Where(p => p.UnitLife > 0).ToList();
        int count = Random.Range(1, targets.Count + 1);

        for (int i = 0; i < count; i++)
        {
            if (targets.Count == 0) break;

            DataEntity target = targets.FirstOrDefault(p => p.provoking) ?? targets[Random.Range(0, targets.Count)];
            ToggleTargetIndicator(target);

            CapacityData chosenSpell = SelectSpell(attacker);
            CombatManager.SINGLETON.ApplyCapacityToTarget(chosenSpell, target);

            yield return new WaitForSeconds(0.1f);
        }

        PostAttackProcessing(attacker);

        yield return new WaitForSeconds(0.1f);
        CombatManager.SINGLETON.EndUnitTurn();

        DisableTargetIndicators();
    }

    private void PostAttackProcessing(DataEntity attacker)
    {
        CombatManager.SINGLETON.DecrementBuffDurations(attacker);

        if (!attacker.beenHurtThisTurn && attacker.RageTick > 0)
        {
            attacker.RageTick--;
        }

        attacker.beenHurtThisTurn = false;

        if (attacker.necrosis?.Count > 0)
        {
            CombatManager.SINGLETON.TickNecrosisEffect(attacker);
        }

        CombatManager.SINGLETON.RecalculateStats(attacker);
    }

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

        if (total == 0 || enemy._CapacityData1 == null || enemy._CapacityData2 == null)
            return enemy._CapacityData1 ?? enemy._CapacityData2;

        int roll = Random.Range(0, total);
        return roll < value1 ? enemy._CapacityData1 : enemy._CapacityData2;
    }
}
