using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager SINGLETON;

    public GameObject[] effetsFoudre = new GameObject[4];
    private Dictionary<int, GameObject> lastFoudreEffects = new Dictionary<int, GameObject>();
    public GameObject effetBouclier;
    public GameObject pictoBuff;
    
    [Header("Sliders de Rage par entité")]
    public List<UnityEngine.UI.Slider> rageSliders = new List<UnityEngine.UI.Slider>();

    [Header("Prefab de texte pour afficher les dégâts")]
    public GameObject damageTextPrefab;

    [Header("Positions d'effet par entité")]
    public List<Transform> DamagePosition = new List<Transform>();
    public List<Transform> Effects1Position = new List<Transform>();
    public List<Transform> Effects2Position = new List<Transform>();

    public Canvas canvas;
    public float effetDuration = 2f;

    void Awake()
    {
        if (SINGLETON != null)
        {
            Debug.LogError("Multiple instances of EffectsManager detected!");
            Destroy(gameObject);
            return;
        }
        SINGLETON = this;
    }
    void Start()
    {
        foreach (var slider in rageSliders)
        {
            if (slider != null)
                slider.gameObject.SetActive(false);
        }
    }

    public void AfficherAttaqueFoudre(int typeFoudre, int index)
    {
        if (typeFoudre < 1 || typeFoudre > 4 || !IsValid(index)) return;

        if (lastFoudreEffects.TryGetValue(index, out var lastEffect))
        {
            Destroy(lastEffect);
            lastFoudreEffects.Remove(index);
        }

        GameObject effet = effetsFoudre[typeFoudre - 1];
        if (effet != null)
        {
            GameObject nouvelEffet = Instantiate(effet, Effects1Position[index].position, Quaternion.identity, Effects1Position[index]);
            lastFoudreEffects.Add(index, nouvelEffet);
        }
    }
    
    public void AfficherRageSlider(int rageValue, int index)
    {
        Debug.Log($"Mise à jour Rage: valeur={rageValue}, index={index}");
        if (!IsValid(index) || rageValue < 0 || rageSliders == null || rageSliders.Count <= index)
            return;

        Slider slider = rageSliders[index];
        if (slider == null) return;

        slider.maxValue = 12;
        slider.value = rageValue;
        slider.gameObject.SetActive(rageValue > 0);
    }

    public void AfficherAttaqueBouclier(int index, int degats)
    {
        if (!IsValid(index)) return;

        if (effetBouclier != null)
        {
            Instantiate(effetBouclier, Effects2Position[index].position, Quaternion.identity);
            AfficherTexteDegats(index, degats, Color.cyan);
        }
    }

    public void AfficherAttaqueSimple(DataEntity entity, int degats)
    {
        int index = entity.index;
        if (index == -1) return;
        Debug.Log($"AfficherAttaqueSimple: Entity={entity.namE}, index={entity.index}");
        AfficherTexteDegats(index, degats, Color.red);
    }
    public void AfficherHeal(DataEntity entity, int healAmmount)
    {
        int index = entity.index;
        if (index == -1) return;
        Color darkGreen;
        ColorUtility.TryParseHtmlString("#0b4d0b", out darkGreen);
        
        AfficherTexteHeal(index, healAmmount, darkGreen);
    }

    public void AfficherPictoBuff(int index)
    {
        if (!IsValid(index)) return;

        if (pictoBuff != null)
        {
            GameObject instance = Instantiate(pictoBuff, DamagePosition[index].position, Quaternion.identity);
            Destroy(instance, effetDuration);
        }
    }

    private void AfficherTexteDegats(int index, int degats, Color couleur)
    {
        if (!IsValid(index) || damageTextPrefab == null || canvas == null) return;

        GameObject dmgText = Instantiate(damageTextPrefab, DamagePosition[index].position, Quaternion.identity, canvas.transform);
        var tmp = dmgText.GetComponent<TextMeshProUGUI>();
        tmp.text = "-" + degats;
        tmp.color = couleur;
        Destroy(dmgText, effetDuration);
    }
    private void AfficherTexteHeal(int index, int healAmmount, Color couleur)
    {
        if (!IsValid(index) || damageTextPrefab == null || canvas == null) return;

        GameObject dmgText = Instantiate(damageTextPrefab, DamagePosition[index].position, Quaternion.identity, canvas.transform);
        var tmp = dmgText.GetComponent<TextMeshProUGUI>();
        tmp.text = "+" + healAmmount;
        tmp.color = couleur;
        Destroy(dmgText, effetDuration);
    }

    public void ClearEffectsForEntity(int index)
    {
        if (!IsValid(index)) return;

        ClearInstantiatedChildren(Effects1Position[index]);
        ClearInstantiatedChildren(Effects2Position[index]);
        ClearInstantiatedChildren(DamagePosition[index]);
    }

    private void ClearInstantiatedChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains("(Clone)"))
                Destroy(child.gameObject);
        }
    }

    private bool IsValid(int index)
    {
        bool isValid = index >= 0 
                       && index < DamagePosition.Count && DamagePosition[index] != null
                       && index < Effects1Position.Count && Effects1Position[index] != null
                       && index < Effects2Position.Count && Effects2Position[index] != null;

        if (!isValid)
        {
            Debug.LogError($"Validation failed for index {index}. Counts: "
                           + $"Damage={DamagePosition.Count}, "
                           + $"Eff1={Effects1Position.Count}, "
                           + $"Eff2={Effects2Position.Count}");
        }

        return isValid;
    }

    public void ResetAll()
    {
        DamagePosition.Clear();
        Effects1Position.Clear();
        Effects2Position.Clear();
        lastFoudreEffects.Clear();
    }
}
