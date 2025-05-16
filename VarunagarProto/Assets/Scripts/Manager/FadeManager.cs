using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    [SerializeField] private GameObject fadeImagePrefab;
    [SerializeField] private float fadeDuration = 1f;

    private Image fadeImage;
    private bool isFading = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitFadeImage();
    }

    private void Start()
    {
        FadeIn();
    }

    private void InitFadeImage()
    {
        if (fadeImagePrefab == null) return;
        if (fadeImage != null) return;

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        GameObject fadeObj = Instantiate(fadeImagePrefab, canvas.transform);
        fadeImage = fadeObj.GetComponent<Image>();
        fadeImage.raycastTarget = true;
        fadeImage.gameObject.SetActive(false);
    }

    public void FadeIn(Action onFadeComplete = null) => StartCoroutine(Fade(1f, 0f, onFadeComplete));
    public void FadeOut(Action onFadeComplete = null) => StartCoroutine(Fade(0f, 1f, onFadeComplete));

    private IEnumerator Fade(float startAlpha, float endAlpha, Action onFadeComplete = null)
    {
        if (isFading || fadeImage == null) yield break;
        isFading = true;

        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;

        if (endAlpha == 0f)
            fadeImage.gameObject.SetActive(false);

        isFading = false;

        onFadeComplete?.Invoke();
    }
}
