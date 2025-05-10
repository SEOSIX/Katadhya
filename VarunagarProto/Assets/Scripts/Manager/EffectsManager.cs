using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;

public class EffectsManager : MonoBehaviour
{
    public GameObject[] effetsFoudre = new GameObject[4];
    private Dictionary<int, GameObject> lastFoudreEffects = new Dictionary<int, GameObject>();
    public GameObject effetBouclier;
    public GameObject pictoBuff;

    [Header("Prefab de texte pour afficher les dégâts")]
    public GameObject damageTextPrefab;

    [FormerlySerializedAs("positionsEffets")] [Header("Positions d'effet par entité")]
    public Transform[] DamagePosition;
    public Transform[] Effects1Position;
    public Transform[] Effects2Position;

    
    public Canvas canvas;
    public static EffectsManager SINGLETON;

    [Tooltip("Durée de vie des effets visuels")]
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

    public void AfficherAttaqueFoudre(int typeFoudre, int index)
    {
        if (typeFoudre < 1 || typeFoudre > 4) return;
        if (lastFoudreEffects.ContainsKey(index))
        {
            Destroy(lastFoudreEffects[index]);
            lastFoudreEffects.Remove(index);
        }

        GameObject effet = effetsFoudre[typeFoudre - 1];

        if (effet != null && IsValid(index))
        {
            GameObject nouvelEffet = Instantiate(
                effet, 
                Effects1Position[index].position, 
                Quaternion.identity, 
                Effects1Position[index]
            );
        
            lastFoudreEffects.Add(index, nouvelEffet);
        }
    }

    public void AfficherAttaqueBouclier(int index, int degats)
    {
        if (effetBouclier != null && IsValid(index))
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
        if (pictoBuff != null && IsValid(index))
        {
            GameObject instance = Instantiate(pictoBuff, DamagePosition[index].position, Quaternion.identity);
            Destroy(instance, effetDuration);
        }
    }

    private void AfficherTexteDegats(int index, int degats, Color couleur)
    {
        if (damageTextPrefab != null && IsValid(index) && canvas != null)
        {
            GameObject dmgText = Instantiate(damageTextPrefab, DamagePosition[index].position, Quaternion.identity,canvas.transform);
            dmgText.GetComponent<TextMeshProUGUI>().text = "-" + degats;
            dmgText.GetComponent<TextMeshProUGUI>().color = couleur;
            Destroy(dmgText, effetDuration);
        }
    }
    
    public void ClearEffectsForEntity(int index)
    {
        if (!IsValid(index)) return;
        if (Effects1Position[index].childCount > 0)
        {
            foreach (Transform child in Effects1Position[index])
            {
                Destroy(child.gameObject);
            }
        }
        if (Effects2Position[index].childCount > 0)
        {
            foreach (Transform child in Effects2Position[index])
            {
                Destroy(child.gameObject);
            }
        }
        if (DamagePosition[index].childCount > 0)
        {
            foreach (Transform child in DamagePosition[index])
            {
                Destroy(child.gameObject);
            }
        }
    }


    private bool IsValid(int index)
    {
        return DamagePosition != null && index >= 0 && index < DamagePosition.Length && DamagePosition[index] != null;
    }
}
