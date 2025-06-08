using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI_Loader : MonoBehaviour
{
    public LevelDesign levelDesign;
      public void OnButtonPress()
    {
        if (ExplorationManager.SINGLETON)
        {
            Combat LD = levelDesign.CombatList[ExplorationManager.SINGLETON.CombatIndex];
            if (LD.TwoRoomsInARow && ExplorationManager.SINGLETON.FirstRoom)
            {
                ExplorationManager.SINGLETON.LoadChoicesFromList(LD.SecondRoomOptions);
                ExplorationManager.SINGLETON.FirstRoom = false;
            }
            else
            {
                ExplorationManager.SINGLETON.LoadNextCombatChoice();
                ExplorationManager.SINGLETON.FirstRoom = true;

            }
        }
    }
}
