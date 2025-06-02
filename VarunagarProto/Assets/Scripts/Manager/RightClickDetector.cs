using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClickDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int CptIndex;
    public bool MouseIsOver;

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseIsOver = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        MouseIsOver = false;
    }
    public void Update()
    {
        if (MouseIsOver)
        {
            if (Input.GetMouseButtonDown(1))
            {
                CombatManager CM = CombatManager.SINGLETON;
                CM.UpgradeCpt(CptIndex);
            }
        }
    }
}
