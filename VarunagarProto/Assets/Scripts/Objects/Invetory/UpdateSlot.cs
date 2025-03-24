using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpdateSlot : MonoBehaviour
{
    public Button targetButton; 
    public Transform requiredParent; 
    public Sprite defaultSprite; 

    private void Update()
    {
        if (requiredParent.childCount > 0) 
        {
            Image childImage = requiredParent.GetChild(0).GetComponent<Image>(); 
            if (childImage != null)
            {
                targetButton.image.sprite = childImage.sprite;
                return;
            }
        }
        
        targetButton.image.sprite = defaultSprite;
    }
}
