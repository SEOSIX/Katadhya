using System.Collections;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    public GameObject blackScreen;   // L'�cran noir au d�but
    public GameObject introPanel;    // Le panel de l'intro
    public GameObject menuPanel;     // Le panel du menu
    public float blackScreenDuration = 2f;  // Dur�e de l'�cran noir
    public float delayBeforeMenu = 5f;     // Dur�e de l'intro avant transition vers le menu
    public float fadeDuration = 1f;        // Dur�e du fondu entre intro et menu

    private CanvasGroup introGroup;
    private CanvasGroup menuGroup;

    void Start()
    {
        introGroup = introPanel.GetComponent<CanvasGroup>();
        menuGroup = menuPanel.GetComponent<CanvasGroup>();

        // Initialise les �tats
        introPanel.SetActive(true);
        menuPanel.SetActive(true); // On active le menu pour permettre le fade-in mais on rend invisible
        introGroup.alpha = 1f;
        menuGroup.alpha = 0f;

        blackScreen.SetActive(true); // Affiche l'�cran noir
        StartCoroutine(ShowIntro());
    }

    IEnumerator ShowIntro()
    {
        // Attendre la dur�e de l'�cran noir
        yield return new WaitForSeconds(blackScreenDuration);

        // Masque l'�cran noir
        blackScreen.SetActive(false);

        // Attendre que l'intro soit termin�e avant de passer au menu
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

        // Assurer les �tats finaux
        introGroup.alpha = 0f;
        introPanel.SetActive(false);
        menuGroup.alpha = 1f;
    }
}