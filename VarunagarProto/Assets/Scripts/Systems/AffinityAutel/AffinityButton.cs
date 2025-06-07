using UnityEngine;
using TMPro;

public class AffinityButton : MonoBehaviour
{
    public DataEntity entity;
    public QTEUpgrade qte;
    public Animator ButtonAnimator;

    public int affinityIndex; 
    public TextMeshProUGUI priceText;

    private void Update()
    {
        UpdatePriceDisplay();
    }

    public void OnQTEButtonClicked()
    {
        AutelQTEUpgrade.Instance.SelectPlayer(entity, qte, ButtonAnimator);
    }

    private void UpdatePriceDisplay()
    {
        if (entity == null || priceText == null)
            return;

        int level = GetAffinityLevel();
        int cost = GetCostFromLevel(level);

        if (level >= 9)
        {
            priceText.text = "";
            priceText.color = Color.grey;
        }
        else
        {
            priceText.text = cost.ToString();
            priceText.color = HasEnoughCauris(cost) ? Color.white : Color.red;
        }
    }

    private int GetAffinityLevel()
    {
        switch (affinityIndex)
        {
            case 0: return entity.UltLvl_1;
            case 1: return entity.UltLvl_2;
            case 2: return entity.UltLvl_3;
            case 3: return entity.UltLvl_4;
            default: return 0;
        }
    }

    private int GetCostFromLevel(int level)
    {
        var upgradeSystem = AutelQTEUpgrade.Instance;

        if (level >= 7) return upgradeSystem.cout3;
        if (level >= 5) return upgradeSystem.cout2;
        return upgradeSystem.cout1;
    }

    private bool HasEnoughCauris(int cost)
    {
        return AutelQTEUpgrade.Instance.playerData.caurisPerAffinity[affinityIndex] >= cost;
    }
}