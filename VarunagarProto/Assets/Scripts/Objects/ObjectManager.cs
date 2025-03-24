using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectManager : MonoBehaviour
{
    public GameObject[] inventorySlots;
    
    public Button[] buttons;
    
    public void OnButtonClicked(Button clickedButton)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].transform.childCount == 0)
            {
                Image buttonImage = clickedButton.image;
                GameObject newItem = new GameObject("InventoryItem");
                Image itemImage = newItem.AddComponent<Image>();
                itemImage.sprite = buttonImage.sprite;
                
                
                RectTransform rectTransform = newItem.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(100, 100); 
                rectTransform.SetParent(inventorySlots[i].transform, false);
                
                GrabdableItem grabdableItem = newItem.AddComponent<GrabdableItem>();
                grabdableItem.image = itemImage;
                
                clickedButton.gameObject.SetActive(false);
                break;
            }
        }
    }
}
