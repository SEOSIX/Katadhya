using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealingZone : MonoBehaviour
{
    public EntityHandler entityHandler;
    public GameObject[] BackgroundPrefabs;
    public TextMeshProUGUI[] PriceTexts = new TextMeshProUGUI[4];
    public int PriceIndex;
    public int[] Prices = new int[3];
    public string SelectedItem;
    public GameObject SelectedObj;
    public GameObject ConfirmationPanel;
    public GlobalPlayerData BigData;
    public TextMeshProUGUI ObjectDescription;
    public TextMeshProUGUI ConfirmationPrice;
    public int StatueIndex;
    public GameObject[] Statues;
    public GameObject[] StatuesImages;
    public GameObject OrangeImage;
    public GameObject PlayerLife;

    public void Start()
    {
        for(int i = 0; i < entityHandler.players.Count; i++)
        {
            if (entityHandler.players[i].UnitLife <= 0)
            {
                Statues[i].SetActive(true);
            }
            else
            {
                Statues[i].SetActive(false);
            }
        }
    }

    public void LoadRandomBackground()
    {

    }
    public void UpdatePrices()
    {
        PriceIndex += 1;
        foreach (TextMeshProUGUI G in PriceTexts)
        {
            G.text = $"{Prices[PriceIndex]}";
        }
    }

    public void SelectItem(string ItemType)
    {
        SelectedItem = ItemType;
        if (BigData.caurisCount < Prices[PriceIndex]) return;
        else ConfirmationPanel.SetActive(true);
        OrangeImage.SetActive(false);
        PlayerLife.SetActive(false);
        foreach (GameObject G in StatuesImages) G.SetActive(false);
        ConfirmationPrice.text = $"{Prices[PriceIndex]}";
        switch (SelectedItem)
        {
            case "Orange":
                {
                    ObjectDescription.text = "Soigne 25% des PV de votre équipe. \n Valider ? ";
                    PlayerLife.SetActive(true);
                    OrangeImage.SetActive(true);
                }
                break;
            case "StatueGarde":
                {
                    ObjectDescription.text = "Ressuscite le Garde avec 50% de ses PV. \n Valider ? ";
                    StatueIndex = 2;
                    StatuesImages[2].SetActive(true);
                }
                break;
            case "StatueMoine":
                {
                    ObjectDescription.text = "Ressuscite la Moine avec 50% de ses PV. \n Valider ? ";
                    StatueIndex = 0;
                    StatuesImages[0].SetActive(true);
                }
                break;
            case "StatuePriso":
                {
                    ObjectDescription.text = "Ressuscite la Prisonnière avec 50% de ses PV. \n Valider ? ";
                    StatueIndex = 1;
                    StatuesImages[1].SetActive(true);
                }
                break;
        }

    }
    public void UseItem()
    {
        BigData.caurisCount -= Prices[PriceIndex]; 
        switch (SelectedItem)
        {
            case "Orange": LifeEntity.SINGLETON.HealAllPlayer(); break;
            default: LifeEntity.SINGLETON.HealSpecificPlayer(StatueIndex, 0.5f); break;
        }
        SelectedObj.SetActive(false);
        UpdatePrices();


    }
    public void GetSelectedObj(GameObject obj) { SelectedObj = obj; }

}
