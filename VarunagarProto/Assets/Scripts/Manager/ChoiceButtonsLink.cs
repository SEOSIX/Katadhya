using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceButtonsLink : MonoBehaviour
{
    public string Scene;

    public void OnButtonPress()
    {
        ExplorationManager.SINGLETON.StartLoadScene(Scene);
    }
    public void OnCombatButtonPress()
    {
        ExplorationManager.SINGLETON.StartLoadNextCombatScene();
    }
}
