using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;
using JetBrains.Annotations;

[System.Serializable]
public class SoundPackage
{
    public AudioClip Cpt1;
    public AudioClip Cpt2;
    public AudioClip Cpt3;
    public AudioClip Cpt4;
}

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager SINGLETON { get; private set; }

    public GameObject[] effetsFoudre = new GameObject[4];
    private Dictionary<int, GameObject> lastFoudreEffects = new Dictionary<int, GameObject>();
    public GameObject effetBouclier;
    public GameObject pictoBuff;
    
    [Header("Sliders de Rage par entité")]
    public List<UnityEngine.UI.Slider> rageSliders = new List<UnityEngine.UI.Slider>();

    [Header("Prefab de texte pour afficher les dégâts")]
    public GameObject damageTextPrefab;

    [Header("VFX")]
    //0 : Atk,  1 : Heal, 2 : Crit, 3 : Débuff, 4 : Buff
    public GameObject[] ParticlePrefabs;
    public Transform ParticleParent;

    [Header("Sound Effects")]
    public List<SoundPackage> PlayerCptSounds;

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

    public void AfficherAttaqueFoudre(int typeFoudre, int index, DataEntity entity)
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
        Renderer[] renderers = entity.instance.GetComponentsInChildren<Renderer>();
        Bounds AllRenderers = renderers[0].bounds;
        foreach (Renderer r in renderers) AllRenderers.Encapsulate(r.bounds);
        GameObject dmgVFX = Instantiate(ParticlePrefabs[4], new Vector3(AllRenderers.center.x, AllRenderers.min.y, AllRenderers.center.z), Quaternion.identity, ParticleParent);

    }
    
    public void AfficherRageSlider(int rageValue, int index)
    {
        if (!IsValid(index) || rageValue < 0 || rageSliders == null || rageSliders.Count <= index)
            return;

        Slider slider = rageSliders[index];
        if (slider == null) return;

        slider.maxValue = 12;
        slider.value = rageValue;
        slider.gameObject.SetActive(rageValue > 0);
    }

    public void AfficherAttaqueBouclier(int index, int degats, DataEntity player)
    {
        if (!IsValid(index)) return;

        if (effetBouclier != null)
        {
            Instantiate(effetBouclier, Effects2Position[index].position, Quaternion.identity);
            AfficherTexteDegats(index, degats, Color.cyan, player);
        }
    }

    public void AfficherAttaqueSimple(DataEntity entity, int degats, float modifier)
    {
        int index = entity.index;
        if (index == -1) return;
        AfficherTexteDegats(index, degats, Color.red, entity, modifier);
    }
    public void AfficherHeal(DataEntity entity, int healAmmount)
    {
        int index = entity.index;
        if (index == -1) return;
        Color darkGreen;
        ColorUtility.TryParseHtmlString("#0b4d0b", out darkGreen);
        
        AfficherTexteHeal(index, healAmmount, darkGreen, entity);
    }

    public void AfficherPictoBuff(int index, CapacityData CData, DataEntity target)
    {
        if (!IsValid(index)) return;
        StartCoroutine(BuffUI(index,CData,target));
        
    }

    private void AfficherTexteDegats(int index, int degats, Color couleur, DataEntity entity, float modifier = 1)
    {
        if (!IsValid(index) || damageTextPrefab == null || canvas == null) return;

        GameObject dmgText = Instantiate(damageTextPrefab, DamagePosition[index].position, Quaternion.identity, canvas.transform);
        if(modifier != 1)
        {
            dmgText.GetComponent<Animator>().SetBool("Crit", true);
        }
        var tmp = dmgText.GetComponent<TextMeshProUGUI>();
        tmp.text = "-" + degats;
        tmp.color = couleur;
        Renderer[] renderers = entity.instance.GetComponentsInChildren<Renderer>();
        Bounds AllRenderers = renderers[0].bounds;
        foreach (Renderer r in renderers) AllRenderers.Encapsulate(r.bounds);
        int particleIndex = 0;
        if (modifier != 1) particleIndex = 2;
        GameObject dmgVFX = Instantiate(ParticlePrefabs[particleIndex],new Vector3(AllRenderers.center.x,AllRenderers.min.y, AllRenderers.center.z), Quaternion.identity, ParticleParent);
        Destroy(dmgText, effetDuration);
    }
    private void AfficherTexteHeal(int index, int healAmmount, Color couleur, DataEntity entity)
    {
        if (!IsValid(index) || damageTextPrefab == null || canvas == null) return;

        GameObject dmgText = Instantiate(damageTextPrefab, DamagePosition[index].position, Quaternion.identity, canvas.transform);
        var tmp = dmgText.GetComponent<TextMeshProUGUI>();
        tmp.text = "+" + healAmmount;
        tmp.color = couleur;
        Renderer[] renderers = entity.instance.GetComponentsInChildren<Renderer>();
        Bounds AllRenderers = renderers[0].bounds;
        foreach (Renderer r in renderers) AllRenderers.Encapsulate(r.bounds);
        GameObject dmgVFX = Instantiate(ParticlePrefabs[1], new Vector3(AllRenderers.center.x, AllRenderers.min.y, AllRenderers.center.z), Quaternion.identity, ParticleParent);
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

    IEnumerator BuffUI(int index, CapacityData CData, DataEntity entity)
    {
        yield return new WaitForSeconds(0.5f);
        GameObject instance = Instantiate(pictoBuff, DamagePosition[index].position+new Vector3(0.25f,0f,0f), Quaternion.identity, canvas.transform);
        Renderer[] renderers = entity.instance.GetComponentsInChildren<Renderer>();
        Bounds AllRenderers = renderers[0].bounds;
        foreach (Renderer r in renderers) AllRenderers.Encapsulate(r.bounds);
        int particleIndex = 3;
        Vector3 particlePosition = new Vector3(AllRenderers.center.x, AllRenderers.min.y, AllRenderers.center.z);
        if (instance != null)
        {
            string arrow;
            if (CData.buffValue > 1)
            {
                StartCoroutine(AudioManager.SINGLETON.PlayCombatClip(10));
                particleIndex = 4;
                instance.GetComponent<TextMeshProUGUI>().color = new Color32(0, 39, 11, 255);
                arrow = "▲";
            }
            else
            {
                StartCoroutine(AudioManager.SINGLETON.PlayCombatClip(11));
                particleIndex = 3;
                particlePosition = new Vector3(AllRenderers.center.x, AllRenderers.max.y, AllRenderers.center.z);
                instance.GetComponent<TextMeshProUGUI>().color = new Color32(155, 0, 0, 255);
                arrow = "▼";
            }

            switch (CData.buffType)
            {
                case 1:
                    instance.GetComponent<TextMeshProUGUI>().SetText($"ATQ{arrow}");
                    break;
                case 2:
                    instance.GetComponent<TextMeshProUGUI>().SetText($"DEF{arrow}");
                    break;
                case 3:
                    instance.GetComponent<TextMeshProUGUI>().SetText($"VIT{arrow}");
                    break;
                case 4:
                    instance.GetComponent<TextMeshProUGUI>().SetText($"PRE{arrow}");
                    break;
            }
        }
        GameObject dmgVFX = Instantiate(ParticlePrefabs[particleIndex], particlePosition, Quaternion.identity, ParticleParent);
        yield return new WaitForSeconds(1f);
        Destroy(instance, effetDuration);
        
    }


}
