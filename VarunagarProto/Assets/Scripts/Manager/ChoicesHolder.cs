using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ChoicesHolder : MonoBehaviour
{
    public static ChoicesHolder SINGLETON { get; private set; }



    private void Awake()
    {
        if (SINGLETON != null && SINGLETON != this)
        {
            Destroy(this.gameObject);
            return;
        }
        SINGLETON = this;
        StartCoroutine(waitforExplo());
    }

    private IEnumerator waitforExplo()
    {
        while (ExplorationManager.SINGLETON == null)
        {
            yield return null;
        }
        ExplorationManager.SINGLETON.LoadChoicesAfterCombat();
    }
    [Header("Cauris amount text")]
    public TextMeshProUGUI caurisBasic;
    public TextMeshProUGUI cauris1;
    public TextMeshProUGUI cauris2;
    public TextMeshProUGUI cauris3;
    public TextMeshProUGUI cauris4;

    [Header("UI"), Space(30)]
    public GameObject combatUI;

    [Header("Buttons")]
    public GameObject CombatSceneButton;
    public GameObject QTESceneButton;
    public GameObject StatSceneButton;
    public GameObject HealingSceneButton;
}
