using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealingZone : MonoBehaviour
{
    public GameObject[] BackgroundPrefabs;
    public TextMeshProUGUI[] PriceTexts = new TextMeshProUGUI[4];
    public int PriceIndex;
    public int[] Prices = new int[3];
    public string SelectedItem;
    
    public void LoadRandomBackground()
    {

    }
    public void UpdatePrices()
    {
        foreach (TextMeshProUGUI G in PriceTexts)
        {
            G.text = $"{Prices[PriceIndex]}";
        }
        PriceIndex += 1;
    }
    public void ResetAll()
    {

    }

    public void SelectItem(string ItemType)
    {
        SelectedItem = ItemType;
        switch (ItemType)
        {
            case "Orange": LifeEntity.SINGLETON.HealAllPlayer();
        }
    }

}
