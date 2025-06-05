using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI_Loader : MonoBehaviour
{
      public void OnButtonPress()
    {
        if (ExplorationManager.SINGLETON)
        {
            ExplorationManager.SINGLETON.LoadNextCombatChoice();
        }
    }
}
