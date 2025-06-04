using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;

public class VictoryDefeatUI : MonoBehaviour
{
    public static VictoryDefeatUI SINGLETON { get; private set; }

    [Header("UI de Fin")]
    public GameObject endCombatPanel;
    public TextMeshProUGUI resultText;

    [Header("Slots Ennemis")]
    public List<TextMeshProUGUI> CaurisCountsSpé;

    [Header("Slots Ennemis")]
    public List<Image> enemyPortraits;
    public List<TextMeshProUGUI> enemyNames;

    private bool combatEnded = false;
    [SerializeField] public bool IsTheEnd;


    private void Awake()
    {
        if (SINGLETON != null && SINGLETON != this)
        {
            Destroy(this.gameObject);
            return;
        }
        SINGLETON = this;
    }
    
    

   public void DisplayEndCombat(bool playerWon, List<DataEntity> allEnemies)
{
    if (combatEnded) return;
    combatEnded = true;
    StartCoroutine(DelayedDisplay(playerWon, allEnemies));
}

private IEnumerator DelayedDisplay(bool playerWon, List<DataEntity> allEnemies)
{
    yield return new WaitForSeconds(1f);

    CombatManager.SINGLETON.canvasGroup.alpha = 0f;
    CombatManager.SINGLETON.canvasGroup.interactable = false;
    CombatManager.SINGLETON.canvasGroup.blocksRaycasts = false;

    CombatManager.SINGLETON.enabled = false;
    foreach (var btn in CombatManager.SINGLETON.capacityAnimButtons)
        btn.interactable = false;

    resultText.text = playerWon ? "VICTOIRE" : "DÉFAITE";
    resultText.color = playerWon ? Color.yellow : Color.red;
        if (!IsTheEnd)
        {
            for (int i = 0; i < 4; i++)
            {
                CaurisCountsSpé[i].text = $"{Playtest_Version_Manager.SINGLETON.CaurisSpe[Playtest_Version_Manager.SINGLETON.BigData.Combat].values[i]}";
            }

            int maxDisplay = Mathf.Min(enemyPortraits.Count, allEnemies.Count);
        }
    endCombatPanel.SetActive(true);
    ExplorationManager.SINGLETON.combatUI.SetActive(false);
    Combat currentCombat = GameManager.SINGLETON.currentCombat;
    ExplorationManager.SINGLETON.caurisBasic.text = currentCombat.CaurisDor.ToString();
    ExplorationManager.SINGLETON.cauris1.text = currentCombat.CaurisSpe1.ToString();
    ExplorationManager.SINGLETON.cauris2.text = currentCombat.CaurisSpe2.ToString();
    ExplorationManager.SINGLETON.cauris3.text = currentCombat.CaurisSpe3.ToString();
    ExplorationManager.SINGLETON.cauris4.text = currentCombat.CaurisSpe4.ToString();
}
}
