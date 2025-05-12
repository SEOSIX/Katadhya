using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryDefeatUI : MonoBehaviour
{
    public static VictoryDefeatUI SINGLETON { get; private set; }

    [Header("UI de Fin")]
    public GameObject endCombatPanel;
    public TextMeshProUGUI resultText;

    [Header("Slots Ennemis")]
    public List<Image> enemyPortraits;
    public List<TextMeshProUGUI> enemyNames;

    private bool combatEnded = false;

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
        yield return new WaitForSeconds(1f); // Donne le temps aux animations de finir

        // Désactiver l’UI de combat
        CombatManager.SINGLETON.canvasGroup.alpha = 0f;
        CombatManager.SINGLETON.canvasGroup.interactable = false;
        CombatManager.SINGLETON.canvasGroup.blocksRaycasts = false;

        CombatManager.SINGLETON.enabled = false;
        foreach (var btn in CombatManager.SINGLETON.capacityAnimButtons)
            btn.interactable = false;

        // Texte de résultat
        resultText.text = playerWon ? "VICTOIRE" : "DÉFAITE";
        resultText.color = playerWon ? Color.green : Color.red;

        // Affichage des ennemis
        for (int i = 0; i < enemyPortraits.Count; i++)
        {
            if (i < allEnemies.Count)
            {
                DataEntity enemy = allEnemies[i];
                enemyPortraits[i].gameObject.SetActive(true);
                enemyNames[i].gameObject.SetActive(true);

                enemyPortraits[i].sprite = enemy.portraitUI;
                enemyPortraits[i].color = (enemy.UnitLife > 0) ? Color.white : Color.gray;
                enemyNames[i].text = enemy.namE;
            }
            else
            {
                enemyPortraits[i].gameObject.SetActive(false);
                enemyNames[i].gameObject.SetActive(false);
            }
        }

        endCombatPanel.SetActive(true);
    }
}
