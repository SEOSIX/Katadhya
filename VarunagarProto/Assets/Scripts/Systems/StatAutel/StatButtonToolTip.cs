using UnityEngine;
using UnityEngine.EventSystems;

public class StatButtonTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int statIndex; 
    public AutelStats autelStats;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 buttonPos = transform.position;
        autelStats.ShowTooltip(statIndex, buttonPos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        autelStats.HideTooltip();
    }
}