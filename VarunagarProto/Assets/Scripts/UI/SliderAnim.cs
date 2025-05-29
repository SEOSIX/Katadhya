using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SliderAnim : MonoBehaviour
{
    [SerializeField] float time = 1;
    [SerializeField] float strengh = 1;
    [SerializeField] bool fadeOut = true;

    [ContextMenu("TesterLeShake")]
    public void TakeDamageSliderAnim()
    {
        transform.DOShakePosition(time, strengh);
    }
}
