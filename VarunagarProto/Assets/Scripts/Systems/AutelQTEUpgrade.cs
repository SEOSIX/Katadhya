using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutelQTEUpgrade : MonoBehaviour
{
    public static AutelQTEUpgrade Instance;

    public Image imageDisplay;

    private QTEUpgrade currentQTE;
    private DataEntity currentEntity;
    
    public TextMeshProUGUI[] affinityTexts = new TextMeshProUGUI[4];

    void Awake()
    {
        Instance = this;
        imageDisplay.enabled = false;
    }
    public void SelectPlayer(DataEntity entity, QTEUpgrade qte)
    {
        currentEntity = entity;
        currentQTE = qte;

        imageDisplay.sprite = qte.imageToDisplay;
        imageDisplay.enabled = true;
        
        UpdateAffinityTexts(entity);
    }
    public void UpgradeAffinity(int affinityIndex)
    {
        if (currentEntity == null || currentQTE == null)
            return;

        if (currentEntity == null || currentQTE == null) return;

        switch (affinityIndex)
        {
            case 0: currentEntity.UltLvl_1++; break;
            case 1: currentEntity.UltLvl_2++; break;
            case 2: currentEntity.UltLvl_3++; break;
            case 3: currentEntity.UltLvl_4++; break;
        }
        
        UpdateAffinityTexts(currentEntity);
    }
    
    private void UpdateAffinityTexts(DataEntity entity)
    {
        affinityTexts[0].text = entity.UltLvl_1.ToString();
        affinityTexts[1].text = entity.UltLvl_2.ToString();
        affinityTexts[2].text = entity.UltLvl_3.ToString();
        affinityTexts[3].text = entity.UltLvl_4.ToString();
    }

}