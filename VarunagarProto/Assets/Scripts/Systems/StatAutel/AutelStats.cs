using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AutelStats : MonoBehaviour
{
    
    [System.Serializable]
    public class EntityUpgradeData
    {
        public int atkPrice;
        public int defPrice;
        public int speedPrice;
        public int lifePrice;

        public EntityUpgradeData(int atk, int def, int speed, int life)
        {
            atkPrice = atk;
            defPrice = def;
            speedPrice = speed;
            lifePrice = life;
        }
    }
    public static AutelStats Instance { get; private set; }

    [Header("Référence aux Données Globales")]
    public GlobalPlayerData playerData;
    
    [Header("Limite de niveau")]
    public int maxLevel = 10;

    [Header("UI - Affichage des Stats")]
    public TextMeshProUGUI[] statTexts; // 0 = ATK, 1 = DEF, 2 = SPEED, 3 = LIFE
    public Button[] statButtons;
    
    [Header("UI Tooltip")]
    public GameObject tooltipPanel;
    
    [Header("Tooltip Texts")]
    public TextMeshProUGUI levelCurrentText;
    public TextMeshProUGUI levelNextText;
    public TextMeshProUGUI statCurrentText;
    public TextMeshProUGUI statNextText;
    
    [Header("UI - Affichage des Niveau")]
    public TextMeshProUGUI[] levelText;
    
    [Header("Incréments de Prix")]
    public int atkPriceIncrement = 2;
    public int defPriceIncrement = 3;
    public int speedPriceIncrement = 2;
    public int lifePriceIncrement = 5;
    

    [Header("Valeurs d'Amélioration")]
    public int atkUpgradeValue = 1;
    public int defUpgradeValue = 2;
    public int speedUpgradeValue = 1;
    public int lifeUpgradeValue = 5;

    private DataEntity currentEntity;
    private Animator currentAnimator;
    private Coroutine blinkCoroutine;
    public GameObject VitessePanel;
    
    private readonly Color goldColor = new Color(1f, 0.84f, 0f);
    private Dictionary<DataEntity, EntityUpgradeData> entityPrices = new Dictionary<DataEntity, EntityUpgradeData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < statButtons.Length; i++)
        {
            int index = i;
            statButtons[i].onClick.AddListener(() => OnStatButtonClicked(index));
        }
        
        UpdateStatDisplay();
        CaurisManage.Instance.UpdateCaurisDisplay();
    }

    private void Update()
    {
        CaurisManage.Instance.UpdateCaurisDisplay();

        if (currentEntity == null)
        {
            foreach (var button in statButtons)
            {
                ColorBlock cb = button.colors;
                cb.normalColor = Color.gray;  
                cb.highlightedColor = Color.gray;
                cb.pressedColor = Color.gray;
                cb.selectedColor = Color.gray; 
                button.colors = cb;
            }
        }
        else
        {
            foreach (var button in statButtons)
            {
                ColorBlock cb = button.colors;
                cb.normalColor = Color.white;  
                cb.highlightedColor = Color.white;
                cb.pressedColor = Color.white;
                cb.selectedColor = Color.white; 
                button.colors = cb;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Clic en dehors d'un bouton/UI !");
                
                if (currentAnimator != null)
                {
                    currentAnimator.SetTrigger("Normal");
                    currentAnimator = null;
                }
                currentEntity = null;
                UpdateStatDisplay();
                
            }
            else
            {
                Debug.Log("Clic sur un bouton/UI");
                return;
            }
        }
    }

    public void SelectEntity(DataEntity entity, Animator animator)
    {
        if (currentEntity == entity)
        {
            animator.SetTrigger("Normal");
            ResetSelection();
            return;
        }
        if (!entityPrices.ContainsKey(entity))
        {
            entityPrices[entity] = new EntityUpgradeData(50, 50, 50, 50);
        }

        if (currentAnimator != null)
            currentAnimator.SetTrigger("Normal");

        currentEntity = entity;
        currentAnimator = animator;
        animator.SetTrigger("Selected");

        UpdateStatDisplay();
    }

    private void ResetSelection()
    {
        currentEntity = null;
        currentAnimator = null;
        UpdateStatDisplay();
    }

    
    private void UpdateStatDisplay()
    {
        if (currentEntity == null || statTexts.Length < 4)
        {
            foreach (var text in statTexts)
            {
                text.text = "-";
                text.color = Color.black;
            }
            return;
        }

        int cauris = CaurisManage.Instance.playerData.caurisCount;
        int[] prices = new int[]
        {
            GetCurrentPrice(0),
            GetCurrentPrice(1),
            GetCurrentPrice(2),
            GetCurrentPrice(3)
        };

        for (int i = 0; i < 4; i++)
        {
            bool isMax = GetCurrentLevel(i) >= maxLevel;
            statTexts[i].text = isMax ? "MAX" : prices[i].ToString();
            statTexts[i].color = isMax ? goldColor : (cauris >= prices[i] ? Color.black : Color.red);

            statButtons[i].interactable = !isMax;
            
            if (levelText != null && levelText.Length > i && currentEntity != null)
            {
                levelText[i].text = "Niveau " + GetCurrentLevel(i);
                levelText[i].color = isMax ? goldColor : Color.black;
            }
        }
    }


    private void OnStatButtonClicked(int index)
    {
        if (currentEntity == null) return;

        int currentLevel = GetCurrentLevel(index);
        if (currentLevel >= maxLevel)
        {
            Debug.Log("Niveau maximum atteint !");
            return;
        }

        int cost = GetCurrentPrice(index);

        if (!playerData.SpendGlobalCauris(cost))
        {
            Debug.Log("Pas assez de cauris globaux !");
            return;
        }

        EntityUpgradeData prices = entityPrices[currentEntity];

        switch (index)
        {
            case 0:
                currentEntity.BaseAtk += atkUpgradeValue;
                currentEntity.AtkLevel++;
                prices.atkPrice += atkPriceIncrement;
                break;
            case 1:
                currentEntity.BaseDef += defUpgradeValue;
                currentEntity.DefLevel++;
                prices.defPrice += defPriceIncrement;
                break;
            case 2:
                currentEntity.BaseSpeed += speedUpgradeValue;
                currentEntity.SpeedLevel++;
                prices.speedPrice += speedPriceIncrement;
                break;
            case 3:
                currentEntity.BaseLife += lifeUpgradeValue;
                currentEntity.LifeLevel++;
                prices.lifePrice += lifePriceIncrement;
                break;
        }

        UpdateStatDisplay();
        CaurisManage.Instance.UpdateCaurisDisplay();

        Vector3 btnPos = statButtons[index].transform.position;
        ShowTooltip(index, btnPos);
    }

    
    
    public void ShowTooltip(int index, Vector3 buttonPosition)
    {
        if (currentEntity == null)
        {
            tooltipPanel.SetActive(false);
            return;
        }

        if(index == 2)
        {
            VitessePanel.SetActive(true);
        }
        else
        {
            VitessePanel.SetActive(false);
        }

        string statName = GetStatName(index);
        int currentLevel = GetCurrentLevel(index);
        int nextLevel = currentLevel + 1;
        bool isMaxLevel = currentLevel >= maxLevel;

        int currentStat = GetCurrentStat(index);
        int nextStat = currentStat + GetUpgradeValue(index);

        levelCurrentText.text = currentLevel.ToString();
        levelNextText.text = isMaxLevel ? "-" : nextLevel.ToString();

        statCurrentText.text = $"{statName} : {currentStat}";
        statNextText.text = isMaxLevel ? $"{statName} : MAX" : $"{statName} : {nextStat}";

        levelCurrentText.color = isMaxLevel ? goldColor : Color.black;
        levelNextText.color = isMaxLevel ? goldColor : Color.black;
        statNextText.color = isMaxLevel ? goldColor : Color.black;

        tooltipPanel.SetActive(true);
        tooltipPanel.transform.position = buttonPosition + new Vector3(-3, 0, 0);

        StopBlinking();
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
    
    private int GetCurrentLevel(int index)
    {
        switch (index)
        {
            case 0: return currentEntity.AtkLevel;
            case 1: return currentEntity.DefLevel;
            case 2: return currentEntity.SpeedLevel;
            case 3: return currentEntity.LifeLevel;
            default: return 0;
        }
    }
    
    private int GetCurrentStat(int index)
    {
        switch (index)
        {
            case 0: return currentEntity.BaseAtk;
            case 1: return currentEntity.BaseDef;
            case 2: return currentEntity.BaseSpeed;
            case 3: return currentEntity.BaseLife;
            default: return 0;
        }
    }

    private int GetUpgradeValue(int index)
    {
        switch (index)
        {
            case 0: return atkUpgradeValue;
            case 1: return defUpgradeValue;
            case 2: return speedUpgradeValue;
            case 3: return lifeUpgradeValue;
            default: return 0;
        }
    }
    private int CalculatePrice(int basePrice, int increment, int level)
    {
        return basePrice + increment * level;
    }
    private int GetCurrentPrice(int index)
    {
        if (currentEntity == null) return 0;

        int level = GetCurrentLevel(index);
        return index switch
        {
            0 => CalculatePrice(50, atkPriceIncrement, level),
            1 => CalculatePrice(50, defPriceIncrement, level),
            2 => CalculatePrice(50, speedPriceIncrement, level),
            3 => CalculatePrice(50, lifePriceIncrement, level),
            _ => 0
        };
    }

    private string GetStatName(int index)
    {
        switch (index)
        {
            case 0: return "ATK";
            case 1: return "DEF";
            case 2: return "VIT";
            case 3: return "VIE";
            default: return "STAT";
        }
    }
    
    private IEnumerator BlinkTextAlpha(TextMeshProUGUI text, float minAlpha = 0.3f, float maxAlpha = 1f, float duration = 1f)
    {
        Color c = text.color;
        while (true)
        {
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float alpha = Mathf.Lerp(maxAlpha, minAlpha, t / duration);
                text.color = new Color(c.r, c.g, c.b, alpha);
                yield return null;
            }
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float alpha = Mathf.Lerp(minAlpha, maxAlpha, t / duration);
                text.color = new Color(c.r, c.g, c.b, alpha);
                yield return null;
            }
        }
    }
    
    private void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
        SetAlpha(levelNextText, 1f);
        SetAlpha(statNextText, 1f);
    }

    private void SetAlpha(TextMeshProUGUI text, float alpha)
    {
        Color c = text.color;
        text.color = new Color(c.r, c.g, c.b, alpha);
    }
}
