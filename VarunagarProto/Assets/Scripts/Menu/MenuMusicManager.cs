using UnityEngine;

public class MenuMusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioIntro;
    [SerializeField] private AudioSource audioLoop;
    [SerializeField] private AudioClip menuIntroClip;
    [SerializeField] private AudioClip menuLoopClip;

    [SerializeField] private float crossfadeDuration = 4f; // Durée du fondu
    [SerializeField] private float startLoopBeforeEnd = 4f; // Quand démarrer la loop avant la fin de l’intro

    private static bool introAlreadyPlayed = false;

    void Start()
    {
        if (!introAlreadyPlayed)
        {
            PlayIntroThenLoopWithCrossfade();
            introAlreadyPlayed = true;
        }
        else
        {
            PlayLoopOnly();
        }
    }

    void PlayIntroThenLoopWithCrossfade()
    {
        audioIntro.clip = menuIntroClip;
        audioIntro.volume = 0.7f;
        audioIntro.Play();

        audioLoop.clip = menuLoopClip;
        audioLoop.volume = 0f;
        audioLoop.loop = true;

        float timeToStartLoop = menuIntroClip.length - startLoopBeforeEnd;
        Invoke(nameof(StartLoopWithFade), timeToStartLoop);
    }

    public void StartLoopWithFade()
    {
        audioLoop.Play();
        StartCoroutine(FadeInLoopAndFadeOutIntro());
    }

    System.Collections.IEnumerator FadeInLoopAndFadeOutIntro()
    {
        float timer = 0f;

        while (timer < crossfadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / crossfadeDuration;

            audioLoop.volume = Mathf.Lerp(0f, 1f, t);
            audioIntro.volume = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        audioLoop.volume = 1f;
        audioIntro.Stop();
    }

    void PlayLoopOnly()
    {
        audioLoop.clip = menuLoopClip;
        audioLoop.loop = true;
        audioLoop.volume = 1f;
        audioLoop.Play();
    }
}