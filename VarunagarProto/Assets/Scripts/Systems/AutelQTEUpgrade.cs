using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutelQTEUpgrade : MonoBehaviour
{
    public static AutelQTEUpgrade Instance;

    [Header("Data")]
    public GlobalPlayerData playerData;

    [Header("UI Elements")]
    public Image imageDisplay;
    public TextMeshProUGUI[] affinityTexts = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] caurisTextsPerAffinity = new TextMeshProUGUI[4];

    [Header("Prix")]
    public int cout1 = 20;
    public int cout2 = 40;
    public int cout3 = 60;

    [Header("Who is Selected")]
    public bool isSelectedMonk;
    public bool isSelectedPriso;

    private QTEUpgrade currentQTE;
    [HideInInspector]public DataEntity currentEntity;
    private Animator currentAnimator;

    private Color caurisDefaultColor = Color.white;

    void Awake()
    {
        Instance = this;
        imageDisplay.enabled = false;
    }

    void Update()
    {
        UpdateCaurisDisplay();
    }

    public void SelectPlayer(DataEntity entity, QTEUpgrade qte, Animator animator)
    {
        if (currentEntity == entity)
        {
            animator.SetTrigger("Normal");
            ResetSelection();
            return;
        }

        if (currentAnimator != null)
            currentAnimator.SetTrigger("Normal");

        currentEntity = entity;
        currentQTE = qte;
        currentAnimator = animator;

        animator.SetTrigger("Selected");

        isSelectedMonk = (entity.index == 0);
        isSelectedPriso = (entity.index == 1);

        imageDisplay.sprite = qte.imageToDisplay;
        imageDisplay.enabled = true;

        UpdateAffinityTexts(entity);
    }

    private void ResetSelection()
    {
        imageDisplay.enabled = false;
        currentEntity = null;
        currentQTE = null;
        currentAnimator = null;
        isSelectedMonk = false;
        isSelectedPriso = false;
    }

    public void UpgradeAffinity(int affinityIndex)
    {
        if (currentEntity == null || currentQTE == null)
            return;

        int currentLevel = GetAffinityLevel(affinityIndex);
        if (currentLevel >= 3)
            return;
        currentEntity.CptUltlvl = currentEntity.UltLvl_1 + currentEntity.UltLvl_2 + currentEntity.UltLvl_3 + currentEntity.UltLvl_4;
        if (currentEntity.CptUltlvl >= 10)
            return;

        int cost = GetUpgradeCost(currentLevel);
        if (!playerData.SpendCauris(cost, affinityIndex))
        {
            NotEnoughCaurisFeedback(affinityIndex);
            return;
        }

        IncrementAffinity(affinityIndex);
        UpdateAffinityTexts(currentEntity);
    }

    private int GetAffinityLevel(int index)
    {
        return index switch
        {
            0 => currentEntity.UltLvl_1,
            1 => currentEntity.UltLvl_2,
            2 => currentEntity.UltLvl_3,
            3 => currentEntity.UltLvl_4,
            _ => 0
        };
    }

    private int GetUpgradeCost(int level)
    {
        if (level >=2) return cout3;
        if (level >=1) return cout2;
        return cout1;
    }

    private void IncrementAffinity(int index)
    {
        switch (index)
        {
            case 0: currentEntity.UltLvl_1++; break;
            case 1: currentEntity.UltLvl_2++; break;
            case 2: currentEntity.UltLvl_3++; break;
            case 3: currentEntity.UltLvl_4++; break;
        }
    }

    private void UpdateAffinityTexts(DataEntity entity)
    {
        int[] levels = {
            entity.UltLvl_1,
            entity.UltLvl_2,
            entity.UltLvl_3,
            entity.UltLvl_4
        };

        int total = 0;
        for (int i = 0; i < levels.Length; i++)
            total += levels[i];

        bool reachedMaxTotal = total >= 10;
        Color targetColor = reachedMaxTotal ? new Color(1f, 0.84f, 0f) : Color.white;

        for (int i = 0; i < affinityTexts.Length; i++)
        {
            affinityTexts[i].text = levels[i].ToString();
            affinityTexts[i].color = targetColor;
        }
    }


    private void UpdateCaurisDisplay()
    {
        for (int i = 0; i < caurisTextsPerAffinity.Length; i++)
        {
            if (caurisTextsPerAffinity[i] != null)
                caurisTextsPerAffinity[i].text = playerData.caurisPerAffinity[i].ToString();
        }
    }

    public void NotEnoughCaurisFeedback(int affinityIndex)
    {
        if (caurisTextsPerAffinity.Length > affinityIndex && caurisTextsPerAffinity[affinityIndex] != null)
            StartCoroutine(FlashCaurisRedSmooth(caurisTextsPerAffinity[affinityIndex]));
    }

    private IEnumerator FlashCaurisRedSmooth(TextMeshProUGUI text)
    {
        float duration = 0.4f;
        float half = duration / 2f;

        Color startColor = caurisDefaultColor;
        Color alertColor = Color.red;

        float t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float lerp = t / half;
            text.color = Color.Lerp(startColor, alertColor, lerp);
            yield return null;
        }

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float lerp = t / half;
            text.color = Color.Lerp(alertColor, startColor, lerp);
            yield return null;
        }

        text.color = startColor;
    }
}
