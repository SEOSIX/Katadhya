using System.Collections;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    public GameObject blackScreen;   // L'écran noir au début
    public GameObject introPanel;    // Le panel de l'intro
    public GameObject menuPanel;     // Le panel du menu
    public float blackScreenDuration = 2f;  // Durée de l'écran noir
    public float delayBeforeMenu = 5f;     // Durée de l'intro avant transition vers le menu
    public float fadeDuration = 1f;        // Durée du fondu entre intro et menu

    private CanvasGroup introGroup;
    private CanvasGroup menuGroup;

    void Start()
    {
        introGroup = introPanel.GetComponent<CanvasGroup>();
        menuGroup = menuPanel.GetComponent<CanvasGroup>();

        // Initialise les états
        introPanel.SetActive(true);
        menuPanel.SetActive(true); // On active le menu pour permettre le fade-in mais on rend invisible
        introGroup.alpha = 1f;
        menuGroup.alpha = 0f;

        blackScreen.SetActive(true); // Affiche l'écran noir
        StartCoroutine(ShowIntro());
    }

    IEnumerator ShowIntro()
    {
        // Attendre la durée de l'écran noir
        yield return new WaitForSeconds(blackScreenDuration);

        // Masque l'écran noir
        blackScreen.SetActive(false);

        // Attendre que l'intro soit terminée avant de passer au menu
        yield return new WaitForSeconds(delayBeforeMenu);

        // Fondu de l'intro vers 0 et du menu vers 1
        float timer = 0f;
        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            introGroup.alpha = 1f - t;
            menuGroup.alpha = t;
            timer += Time.deltaTime;
            yield return null;
        }

        // Assurer les états finaux
        introGroup.alpha = 0f;
        introPanel.SetActive(false);
        menuGroup.alpha = 1f;
    }
}