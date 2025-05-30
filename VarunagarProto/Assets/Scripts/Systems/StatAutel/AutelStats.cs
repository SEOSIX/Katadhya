using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AutelStats : MonoBehaviour
{
    public static AutelStats Instance { get; private set; }

    [Header("Référence aux Données Globales")]
    public GlobalPlayerData playerData;

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
    
    [Header("Prix par Stat")]
    public int atkPrice = 5;
    public int defPrice = 10;
    public int speedPrice = 7;
    public int lifePrice = 15;
    
    [Header("Incréments de Prix")]
    public int atkPriceIncrement = 2;
    public int defPriceIncrement = 3;
    public int speedPriceIncrement = 2;
    public int lifePriceIncrement = 5;

    private int currentAtkPrice;
    private int currentDefPrice;
    private int currentSpeedPrice;
    private int currentLifePrice;

    [Header("Valeurs d'Amélioration")]
    public int atkUpgradeValue = 1;
    public int defUpgradeValue = 2;
    public int speedUpgradeValue = 1;
    public int lifeUpgradeValue = 5;

    private DataEntity currentEntity;
    private Animator currentAnimator;
    private Coroutine blinkCoroutine;

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

        currentAtkPrice = atkPrice;
        currentDefPrice = defPrice;
        currentSpeedPrice = speedPrice;
        currentLifePrice = lifePrice;
        
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
        int[] prices = new int[] { currentAtkPrice, currentDefPrice, currentSpeedPrice, currentLifePrice };

        for (int i = 0; i < 4; i++)
        {
            statTexts[i].text = prices[i].ToString();
            statTexts[i].color = (cauris >= prices[i]) ? Color.black : Color.red;
        }
    }

    private void OnStatButtonClicked(int index)
    {
        if (currentEntity == null) return;

        int cost = 0;
        switch (index)
        {
            case 0: cost = atkPrice; break;
            case 1: cost = defPrice; break;
            case 2: cost = speedPrice; break;
            case 3: cost = lifePrice; break;
        }

        if (!playerData.SpendGlobalCauris(cost))
        {
            Debug.Log("Pas assez de cauris globaux !");
            return;
        }
        switch (index)
        {
            case 0:
                currentEntity.BaseAtk += atkUpgradeValue;
                currentEntity.AtkLevel++;
                currentAtkPrice += atkPriceIncrement;
                break;
            case 1:
                currentEntity.BaseDef += defUpgradeValue;
                currentEntity.DefLevel++;
                currentDefPrice += defPriceIncrement;
                break;
            case 2:
                currentEntity.BaseSpeed += speedUpgradeValue;
                currentEntity.SpeedLevel++;
                currentSpeedPrice += speedPriceIncrement;
                break;
            case 3:
                currentEntity.BaseLife += lifeUpgradeValue;
                currentEntity.LifeLevel++;
                currentLifePrice += lifePriceIncrement;
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

        int currentLevel = GetCurrentLevel(index);
        int nextLevel = currentLevel + 1;

        int currentStat = GetCurrentStat(index);
        int nextStat = currentStat + GetUpgradeValue(index);

        string statName = GetStatName(index);
        
        levelCurrentText.text = currentLevel.ToString();
        levelNextText.text = nextLevel.ToString();
        statCurrentText.text = $"{statName} {currentStat}";
        statNextText.text = $"{statName} {nextStat}";

        tooltipPanel.SetActive(true);
        tooltipPanel.transform.position = buttonPosition + new Vector3(-4, 0, 0);
        
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

    private int GetCurrentPrice(int index)
    {
        switch (index)
        {
            case 0: return currentAtkPrice;
            case 1: return currentDefPrice;
            case 2: return currentSpeedPrice;
            case 3: return currentLifePrice;
            default: return 0;
        }
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
