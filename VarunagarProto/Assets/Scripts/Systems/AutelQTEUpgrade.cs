using UnityEngine;
using UnityEngine.UI;

public class AutelQTEUpgrade : MonoBehaviour
{
    public RectTransform targetImage; 
    public Vector3 enlargedScale = new Vector3(1.2f, 1.2f, 1f);
    public float lerpSpeed = 5f;

    private Vector3 originalScale;
    private bool isEnlarging = false;
    private bool isShrinking = false;

    void Start()
    {
        if (targetImage == null)
            targetImage = GetComponent<RectTransform>();

        originalScale = targetImage.localScale;
    }

    void Update()
    {
        if (isEnlarging)
        {
            targetImage.localScale = Vector3.Lerp(targetImage.localScale, enlargedScale, Time.deltaTime * lerpSpeed);

            if (Vector3.Distance(targetImage.localScale, enlargedScale) < 0.01f)
            {
                targetImage.localScale = enlargedScale;
                isEnlarging = false;
            }
        }
        else if (isShrinking)
        {
            targetImage.localScale = Vector3.Lerp(targetImage.localScale, originalScale, Time.deltaTime * lerpSpeed);

            if (Vector3.Distance(targetImage.localScale, originalScale) < 0.01f)
            {
                targetImage.localScale = originalScale;
                isShrinking = false;
            }
        }
    }

    // Appelée via un EventTrigger OnPointerEnter par exemple
    public void EnlargeImage()
    {
        isEnlarging = true;
        isShrinking = false;
    }

    // Appelée via un EventTrigger OnPointerExit par exemple
    public void ResetImageSize()
    {
        isShrinking = true;
        isEnlarging = false;
    }
}