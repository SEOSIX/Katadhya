using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager SINGLETON;

    public GameObject[] effetsFoudre = new GameObject[4];
    private Dictionary<int, GameObject> lastFoudreEffects = new Dictionary<int, GameObject>();
    public GameObject effetBouclier;
    public GameObject pictoBuff;

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

    public int RegisterEnemy(Transform damagePos, Transform effect1Pos, Transform effect2Pos)
    {
        DamagePosition.Add(damagePos);
        Effects1Position.Add(effect1Pos);
        Effects2Position.Add(effect2Pos);
        return DamagePosition.Count - 1;
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

    public void AfficherAttaqueBouclier(int index, int degats)
    {
        if (!IsValid(index)) return;

        if (effetBouclier != null)
        {
            Instantiate(effetBouclier, Effects2Position[index].position, Quaternion.identity);
            AfficherTexteDegats(index, degats, Color.cyan);
        }
    }

    public void AfficherAttaqueSimple(int index, int degats)
    {
        if (IsValid(index))
        {
            AfficherTexteDegats(index, degats, Color.red);
        }
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
        return index >= 0 &&
            index < DamagePosition.Count && DamagePosition[index] != null &&
            index < Effects1Position.Count && Effects1Position[index] != null &&
            index < Effects2Position.Count && Effects2Position[index] != null;
    }

    public void ResetAll()
    {
        DamagePosition.Clear();
        Effects1Position.Clear();
        Effects2Position.Clear();
        lastFoudreEffects.Clear();
    }
}
