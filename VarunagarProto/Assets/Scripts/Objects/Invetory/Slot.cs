using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            GrabdableItem grabdableItem = dropped.GetComponent<GrabdableItem>();

            grabdableItem.transform.SetParent(transform);
            grabdableItem.transform.localPosition = Vector3.zero;

            grabdableItem.ParentAfterDrag = transform;
        }
    }
}