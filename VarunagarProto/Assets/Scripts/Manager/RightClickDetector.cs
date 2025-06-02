using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClickDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public int CptIndex;
    public bool MouseIsOver;
    public GameObject Button = null;

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseIsOver = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        CombatManager.SINGLETON.ResetListener(CptIndex);
        MouseIsOver = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Button = eventData.pointerEnter;
            CombatManager CM = CombatManager.SINGLETON;
            CM.UpgradeCpt(CptIndex, Button);
        }
    }

}
