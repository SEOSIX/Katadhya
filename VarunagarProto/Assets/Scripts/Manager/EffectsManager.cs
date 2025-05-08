using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;

public class EffectsManager : MonoBehaviour
{
    public GameObject effetFoudre;
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

    public void AfficherAttaqueFoudre(int index)
    {
        if (effetFoudre != null && IsValid(index))
        {
            Instantiate(effetFoudre, Effects1Position[index].position, Quaternion.identity);
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
            Vector3 worldPos = DamagePosition[index].position + new Vector3(0, 0f, 0);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            
            GameObject textInstance = Instantiate(damageTextPrefab, canvas.transform);
            
            RectTransform rect = textInstance.GetComponent<RectTransform>();
            rect.position = screenPos;
            TMP_Text textMesh = textInstance.GetComponentInChildren<TMP_Text>();
            if (textMesh != null)
            {
                textMesh.text = degats.ToString();
                textMesh.color = couleur;
            }

            Destroy(textInstance, effetDuration);
        }
    }


    private bool IsValid(int index)
    {
        return DamagePosition != null && index >= 0 && index < DamagePosition.Length && DamagePosition[index] != null;
    }
}
