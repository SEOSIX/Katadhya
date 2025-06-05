using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CinematicManagerWithFade : MonoBehaviour
{
    [SerializeField] private List<CanvasGroup> panels;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private string nextSceneName = "Level1";

    private int currentPanelIndex = 0;
    private bool isTransitioning = false;

    void Start()
    {
        // Assure que tous les panels sauf le premier sont invisibles
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].alpha = 0f;
            panels[i].interactable = false;
            panels[i].blocksRaycasts = false;
        }

        // Lancer le fade-in du premier panel
        StartCoroutine(FadeInPanel(0));
    }

    public void OnNextClicked()
    {
        if (isTransitioning) return;

        if (currentPanelIndex < panels.Count - 1)
        {
            StartCoroutine(FadeToNextPanel(currentPanelIndex, currentPanelIndex + 1));
            currentPanelIndex++;
        }
        else
        {
            // Dernier panel → Charger la scène
            SceneManager.LoadScene(nextSceneName);
        }
    }
    IEnumerator FadeInPanel(int index)
    {
        isTransitioning = true;
        CanvasGroup panel = panels[index];
        float timer = 0f;

        panel.gameObject.SetActive(true);

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            panel.alpha = Mathf.Lerp(0f, 1f, t);
            timer += Time.deltaTime;
            yield return null;
        }

        panel.alpha = 1f;
        panel.interactable = true;
        panel.blocksRaycasts = true;
        isTransitioning = false;
    }
    IEnumerator FadeToNextPanel(int fromIndex, int toIndex)
    {
        isTransitioning = true;

        CanvasGroup fromPanel = panels[fromIndex];
        CanvasGroup toPanel = panels[toIndex];

        float timer = 0f;

        // Activation du panel cible
        toPanel.gameObject.SetActive(true);

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            fromPanel.alpha = 1f - t;
            toPanel.alpha = t;
            timer += Time.deltaTime;
            yield return null;
        }

        // Finalisation
        fromPanel.alpha = 0f;
        fromPanel.interactable = false;
        fromPanel.blocksRaycasts = false;

        toPanel.alpha = 1f;
        toPanel.interactable = true;
        toPanel.blocksRaycasts = true;

        isTransitioning = false;
    }
}