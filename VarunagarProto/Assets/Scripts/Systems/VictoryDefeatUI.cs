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
    
    public List<DataEntity> allEnemiesEncountered = new List<DataEntity>();

    public void RegisterEnemy(DataEntity enemy)
    {
        if (!allEnemiesEncountered.Contains(enemy))
            allEnemiesEncountered.Add(enemy);
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
                CaurisCountsSpé[i].text = $"{Playtest_Version_Manager.SINGLETON.CaurisSpé[Playtest_Version_Manager.SINGLETON.BigData.Combat].values[i]}";

            }

            int maxDisplay = Mathf.Min(enemyPortraits.Count, allEnemies.Count);
        }


    /*for (int i = 0; i < maxDisplay; i++)
    {
        DataEntity enemy = allEnemies[i];

        enemyPortraits[i].gameObject.SetActive(true);
        enemyNames[i].gameObject.SetActive(true);

        enemyPortraits[i].sprite = enemy.portraitUI;
        enemyPortraits[i].color = (enemy.UnitLife > 0) ? Color.white : Color.gray;
        enemyNames[i].text = enemy.namE;
    }

    // Hide unused slots
    for (int i = maxDisplay; i < enemyPortraits.Count; i++)
    {
        enemyPortraits[i].gameObject.SetActive(false);
        enemyNames[i].gameObject.SetActive(false);
    }*/

    endCombatPanel.SetActive(true);
}

}
