using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    [SerializeField] private float size;
    private Vector3 scale;

    private void Start()
    {
        scale = transform.localScale;
    }

    public void GoToSize(float duration) 
    {
        transform.DOScale(scale * size,  duration);
    }
    
    public void GoBackSize(float timeGoBack)
    {
        transform.DOScale(scale,  timeGoBack);
    }
}
