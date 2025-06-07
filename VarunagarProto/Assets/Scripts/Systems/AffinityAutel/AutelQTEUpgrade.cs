using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutelQTEUpgrade : MonoBehaviour
{
    public static AutelQTEUpgrade Instance;

    [Header("Data")]
    public GlobalPlayerData playerData;
    public EntityHandler entityHandler;

    [Header("UI Elements")]
    public Image imageDisplay;
    public TextMeshProUGUI[] affinityTexts = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] caurisTextsPerAffinity = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] caurisCostTextsPerAffinity = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] LevelBefore= new TextMeshProUGUI[4];
    public TextMeshProUGUI[] LevelAfter = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] DescriptionBefore = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] DescriptionAfter = new TextMeshProUGUI[4];
    public List<ListOfDescriptions> DescriptionsBase = new List<ListOfDescriptions>(4);
    public List<ListOfDescriptions> DescriptionsUpgrade = new List<ListOfDescriptions>(4);
    public TextMeshProUGUI bio;
    public TextMeshProUGUI UltLvl;



    public GameObject[] ZonesAff1 =new GameObject[3];
    public GameObject[] ZonesAff2 = new GameObject[3];
    public GameObject[] ZonesAff3 = new GameObject[3];
    public GameObject[] ZonesAff4 = new GameObject[3];


    [Header("Prix")]
    public int cout1 = 20;
    public int cout2 = 40;
    public int cout3 = 60;

    [Header("Who is Selected")]
    public bool isSelectedMonk;
    public bool isSelectedPriso;
    public bool isSelectedGard;

    private QTEUpgrade currentQTE;
    [HideInInspector]public DataEntity currentEntity;
    private Animator currentAnimator;

    private Color caurisDefaultColor = Color.black;

    void Awake()
    {
        Instance = this;
        imageDisplay.enabled = false;

    }

    void Update()
    {
        UpdateCaurisDisplay();
        UpdateZones();
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
        isSelectedGard = (entity.index == 2);


        imageDisplay.sprite = qte.imageToDisplay;
        imageDisplay.enabled = true;

        for (int i = 0; i < 4; i++)
        {
            UpdateAffText(i);
        }
        UpdateAffinityTexts(entity);
        UpdateBottom();
    }

    private void ResetSelection()
    {
        imageDisplay.enabled = false;
        currentEntity = null;
        currentQTE = null;
        currentAnimator = null;
        isSelectedMonk = false;
        isSelectedPriso = false;
        isSelectedGard = false;

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
        UpdateAffText(affinityIndex);
        UpdateBottom();
    }
    public void UpdateAffText(int i)
    {
        int value = GetAffinityLevel(i);
        LevelBefore[i].text = $"{value}";
        string descriptionBefore = "";
        descriptionBefore = value switch
        {
            0 => DescriptionsBase[i].Level1,
            1 => DescriptionsBase[i].Level2,
            2 => DescriptionsBase[i].Level3,
            3 => DescriptionsBase[i].Level4,

            _ => ""
        };
        DescriptionBefore[i].text = descriptionBefore;

        if (value + 1 < 4)
        {
            LevelAfter[i].text = $"{value+1}";
            string descriptionAfter = "";
            descriptionAfter = value switch
            {
                0 => DescriptionsUpgrade[i].Level1,
                1 => DescriptionsUpgrade[i].Level2,
                2 => DescriptionsUpgrade[i].Level3,
                3 => DescriptionsUpgrade[i].Level4,
                _ => ""
            };
            DescriptionAfter[i].text = descriptionAfter;
        }
        else
        {
            LevelAfter[i].text = $"{value}";
            DescriptionAfter[i].text = "";
        }
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

    public int GetUpgradeCost(int level)
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
        Color targetColor = reachedMaxTotal ? new Color(0.3f,0.3f,0.3f) : Color.black;

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

    public void UpdateBottom()
    {
        currentEntity.CptUltlvl = currentEntity.UltLvl_1 + currentEntity.UltLvl_2 + currentEntity.UltLvl_3 + currentEntity.UltLvl_4;
        UltLvl.text = $"Niv {currentEntity.CptUltlvl}";
        if(isSelectedGard) bio.text = $"Buff de défense augmenté de {currentEntity.CptUltlvl *5}%";
        if (isSelectedMonk) bio.text = $"Soin augmenté de {currentEntity.CptUltlvl * 2}";
        if (isSelectedPriso) bio.text = $"Attaque augmentée de {currentEntity.CptUltlvl * 2}";



    }

    private void UpdateZones()
    {
        if (currentEntity == null) return;
        List<GameObject[]> AllZones = new List<GameObject[]> { ZonesAff1, ZonesAff2, ZonesAff3, ZonesAff4 };
        List<int> Affinities = new List<int> { currentEntity.UltLvl_1, currentEntity.UltLvl_2, currentEntity.UltLvl_3, currentEntity.UltLvl_4 };
        for (int i = 0; i < 4; i++)
        {
            for (int j=0; j<3; j++)
            {
                if (j >= Affinities[i]) AllZones[i][j].SetActive(false);
                else AllZones[i][j].SetActive(true);
            } 
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

[System.Serializable]
public class ListOfDescriptions
{
    public string Level1;
    public string Level2;
    public string Level3;
    public string Level4;
}
