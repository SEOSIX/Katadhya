using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndButtonLink : MonoBehaviour
{
    public void CombatEndPress()
    {
        ExplorationManager.SINGLETON.Recompenses();
        ExplorationManager.SINGLETON.LoadChoicesAfterCombat();
    }
}
