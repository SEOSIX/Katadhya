using UnityEngine;
using TMPro;

public class AffinityPriceDisplay : MonoBehaviour
{
    public TextMeshProUGUI[] priceTexts = new TextMeshProUGUI[4];

    private void Update()
    {
        if (AutelQTEUpgrade.Instance.currentEntity == null || AutelQTEUpgrade.Instance == null) return;

        for (int i = 0; i < 4; i++)
        {
            UpdatePriceDisplay(i);
        }
    }

    private void UpdatePriceDisplay(int index)
    {
        if (priceTexts[index] == null)
            return;

        int level = GetAffinityLevel(index);
        int cost = GetCostFromLevel(level);
        var cauris = AutelQTEUpgrade.Instance.playerData.caurisPerAffinity[index];

        if (level >= 9)
        {
            priceTexts[index].text = "";
            priceTexts[index].color = Color.grey;
        }
        else
        {
            priceTexts[index].text = cost.ToString();
            priceTexts[index].color = (cauris >= cost) ? Color.black : Color.red;
        }
    }

    private int GetAffinityLevel(int index)
    {
        return index switch
        {
            0 => AutelQTEUpgrade.Instance.currentEntity.UltLvl_1,
            1 =>  AutelQTEUpgrade.Instance.currentEntity.UltLvl_2,
            2 =>  AutelQTEUpgrade.Instance.currentEntity.UltLvl_3,
            3 =>  AutelQTEUpgrade.Instance.currentEntity.UltLvl_4,
            _ => 0
        };
    }

    private int GetCostFromLevel(int level)
    {
        var upgradeSystem = AutelQTEUpgrade.Instance;

        return upgradeSystem.GetUpgradeCost(level);
    }
}