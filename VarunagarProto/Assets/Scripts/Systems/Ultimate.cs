using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Ultimate : MonoBehaviour
{
    public Slider sliderUltimate;
    public Button UltButton;
    public Image fill;

    private DataEntity previousEntity;
    
    public Animator qteAnimator;
    public GameObject qteUI;

    private DataEntity CurrentEntity => CombatManager.SINGLETON?.currentTurnOrder.Count > 0 ?
        CombatManager.SINGLETON.currentTurnOrder[0] : null;
    
    
    public Transform pointer; 
    public Transform center;

    public float blueZoneStart = 330f;
    public float blueZoneEnd = 30f;

    private void Start()
    {
        UltButton.interactable = false;
        StartCoroutine(CheckEntityChange());
        StartCoroutine(SyncSliderWithEntity());
        StartCoroutine(DrainUltimateOverTime());
    }

    private IEnumerator CheckEntityChange()
    {
        while (true)
        {
            if (CurrentEntity != previousEntity)
            {
                previousEntity = CurrentEntity;
                UpdateSliderFromEntity();
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void UpdateSliderFromEntity()
    {
        if (CurrentEntity == null) return;

        sliderUltimate.maxValue = 100;
        sliderUltimate.minValue = 0;
        sliderUltimate.value = CurrentEntity.UltimateSlider;

        fill.sprite = CurrentEntity.UltimateEmpty;
    }
    
    private IEnumerator SyncSliderWithEntity()
    {
        while (true)
        {
            if (CurrentEntity != null)
            {
                if ((int)sliderUltimate.value != CurrentEntity.UltimateSlider)
                {
                    sliderUltimate.value = CurrentEntity.UltimateSlider;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator DrainUltimateOverTime()
    {
        while (true)
        {
            if (CurrentEntity != null && CurrentEntity.UltimateSlider > 0)
            {
                CurrentEntity.UltimateSlider -= 1;
            }

            yield return new WaitForSeconds(1f);
            SliderManager();
        }
    }
    public void SliderManager()
    {
        if (CurrentEntity == null) return;
        CurrentEntity.UltimateSlider = (int)sliderUltimate.value;
        bool isReady = (sliderUltimate.value == sliderUltimate.minValue);
        UltButton.interactable = isReady;
        CurrentEntity.UltIsReady = isReady;
    }
    public void QTE_Start()
    {
        if (qteAnimator == null || qteUI == null)
        {
            Debug.LogWarning("QTE Animator ou UI non assigné.");
            return;
        }

        qteUI.SetActive(true);
        StartCoroutine(CheckQTEInput());
    }
    
    IEnumerator CheckQTEInput()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                qteAnimator.speed = 0f;
                Debug.Log("QTE: Animation mise en pause par l'utilisateur.");
                CheckPointerInZone();
            }

            yield return null;
        }
    }

    private void CheckPointerInZone()
    {
        if (pointer == null || center == null) return;

        float angle = pointer.eulerAngles.z;
        angle = (angle + 360f) % 360f;

        bool isInZone = (angle >= blueZoneStart || angle <= blueZoneEnd);

        Debug.DrawLine(center.position, center.position + Quaternion.Euler(0, 0, angle) * Vector3.up * 2f, isInZone ? Color.cyan : Color.yellow);

        if (isInZone)
        {
            Debug.Log("🟡 Pointeur DANS la zone bleue !");
        }
        else
        {
            Debug.Log("n'est pas dedans");
        }
    }
}
