using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;

public class AutelQTEUpgrade : MonoBehaviour
{
    public static AutelQTEUpgrade Instance;
    public GlobalPlayerData playerData;
    public Image imageDisplay;

    private QTEUpgrade currentQTE;
    private DataEntity currentEntity;
    private Animator currentAnimator; 

    public TextMeshProUGUI[] affinityTexts = new TextMeshProUGUI[4];

    public bool isSelectedMonk;
    public bool isSelectedPriso;

    void Awake()
    {
        Instance = this;
        imageDisplay.enabled = false;
    }

    public void SelectPlayer(DataEntity entity, QTEUpgrade qte, Animator animator)
    {
        if (currentEntity == entity)
        {
            animator.SetTrigger("Normal");
            imageDisplay.enabled = false;
            currentEntity = null;
            currentQTE = null;
            currentAnimator = null;
            isSelectedMonk = false;
            isSelectedPriso = false;
            return;
        }
        if (currentAnimator != null)
        {
            currentAnimator.SetTrigger("Normal");
        }
        currentEntity = entity;
        currentQTE = qte;
        currentAnimator = animator;

        animator.SetTrigger("Selected");

        if (entity.index == 0)
        {
            isSelectedPriso = false;
            isSelectedMonk = true;
        }
        else if (entity.index == 1)
        {
            isSelectedPriso = true;
            isSelectedMonk = false;
        }
        else
        {
            isSelectedPriso = false;
            isSelectedMonk = false;
        }

        imageDisplay.sprite = qte.imageToDisplay;
        imageDisplay.enabled = true;

        UpdateAffinityTexts(entity);
    }

    public void UpgradeAffinity(int affinityIndex)
    {
        if (currentEntity == null || currentQTE == null)
            return;

        int cost = 20;
        if (!playerData.SpendCauris(cost))
            return;

        switch (affinityIndex)
        {
            case 0:
                if (currentEntity.UltLvl_1 < 9) currentEntity.UltLvl_1++;
                break;
            case 1:
                if (currentEntity.UltLvl_2 < 9) currentEntity.UltLvl_2++;
                break;
            case 2:
                if (currentEntity.UltLvl_3 < 9) currentEntity.UltLvl_3++;
                break;
            case 3:
                if (currentEntity.UltLvl_4 < 9) currentEntity.UltLvl_4++;
                break;
        }

        UpdateAffinityTexts(currentEntity);
    }



    private void UpdateAffinityTexts(DataEntity entity)
    {
        affinityTexts[0].text = entity.UltLvl_1.ToString();
        affinityTexts[0].color = entity.UltLvl_1 >= 9 ? Color.yellow : Color.white;

        affinityTexts[1].text = entity.UltLvl_2.ToString();
        affinityTexts[1].color = entity.UltLvl_2 >= 9 ? Color.yellow : Color.white;

        affinityTexts[2].text = entity.UltLvl_3.ToString();
        affinityTexts[2].color = entity.UltLvl_3 >= 9 ? Color.yellow : Color.white;

        affinityTexts[3].text = entity.UltLvl_4.ToString();
        affinityTexts[3].color = entity.UltLvl_4 >= 9 ? Color.yellow : Color.white;

    }
}
