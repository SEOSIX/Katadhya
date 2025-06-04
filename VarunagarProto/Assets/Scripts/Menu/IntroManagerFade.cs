using UnityEngine;
using System.Collections;

public class IntroManagerFullSequence : MonoBehaviour
{
    public CanvasGroup introGroup;
    public CanvasGroup menuGroup;
    public float delayBeforeIntro = 2f;   // Temps avant de montrer l’intro
    public float introDuration = 4f;      // Durée d’affichage de l’intro
    public float fadeDuration = 1.5f;     // Durée des transitions

    void Start()
    {
        // Tout est caché au lancement
        introGroup.alpha = 0f;
        introGroup.interactable = false;
        introGroup.blocksRaycasts = false;

        menuGroup.alpha = 0f;
        menuGroup.interactable = false;
        menuGroup.blocksRaycasts = false;

        StartCoroutine(PlayFullIntroSequence());
    }

    IEnumerator PlayFullIntroSequence()
    {
        // Étape 1 – Attente initiale (écran noir)
        yield return new WaitForSeconds(delayBeforeIntro);

        // Étape 2 – Fade-in de l’intro
        yield return StartCoroutine(FadeCanvasGroup(introGroup, 0f, 1f, fadeDuration));
        introGroup.interactable = true;
        introGroup.blocksRaycasts = true;

        // Étape 3 – Attente pendant l’intro
        yield return new WaitForSeconds(introDuration);

        // Étape 4 – Fade-out de l’intro
        yield return StartCoroutine(FadeCanvasGroup(introGroup, 1f, 0f, fadeDuration));
        introGroup.interactable = false;
        introGroup.blocksRaycasts = false;

        // Étape 5 – Fade-in du menu
        yield return StartCoroutine(FadeCanvasGroup(menuGroup, 0f, 1f, fadeDuration));
        menuGroup.interactable = true;
        menuGroup.blocksRaycasts = true;
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            cg.alpha = Mathf.Lerp(start, end, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
    }
}