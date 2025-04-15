using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Ultimate : MonoBehaviour
{
    public Slider sliderUltimate;
    public Button UltButton;
    public Image fill;
    
    private DataEntity CurrentEntity => CombatManager.SINGLETON?.currentTurnOrder.Count > 0 ? 
        CombatManager.SINGLETON.currentTurnOrder[0] : null;
    
    private void Start()
    {
        sliderUltimate.value = sliderUltimate.maxValue;
        UltButton.interactable = false;
        sliderUltimate.onValueChanged.AddListener(delegate { SliderManager(); });
        SliderManager();
        StartCoroutine(SliderUpdate());
    }

    public void SliderManager()
    {
        if (CurrentEntity == null) return;
        
        CurrentEntity.UltimateSlider = (int)sliderUltimate.value;
        fill.sprite = CurrentEntity.UltimateEmpty;
        
        UltButton.interactable = (sliderUltimate.value == sliderUltimate.minValue);
        CurrentEntity.UltIsReady = UltButton.interactable;
    }

    IEnumerator SliderUpdate()
    {
        while (sliderUltimate.value != sliderUltimate.minValue)
        {
            sliderUltimate.value -= 1;
            yield return new WaitForSeconds(1f);
        }
    }
}