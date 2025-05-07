using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static CombatManager;
using Random = UnityEngine.Random;

public class QTEZone
{
    public string zoneName;
    public float startAngle;
    public float endAngle;
    public Color debugColor = Color.white;
    public bool successZone = true;
    public int Affinity = 0;
    public int Level;
}

public class OnceCustom
{
    private bool _hasRun = false;

    public void Do(Action action)
    {
        if (_hasRun) return;
        _hasRun = true;
        action?.Invoke();
    }

    public void Reset()
    {
        _hasRun = false;
    }
}

public class Ultimate : MonoBehaviour
{
    public static Ultimate SINGLETON;

    [Header("UI")]
    public Slider sliderUltimate;
    public Button UltButton;
    public Image fill;

    [Header("QTE")]
    public Animator qteAnimator;
    public GameObject qteUI;
    public Transform pointer;
    public Transform center;
    public Transform zoneParent;

    private DataEntity previousEntity;
    private OnceCustom CheckInputOnce = new OnceCustom();
    private List<QTEZone> qteZones = new List<QTEZone>();

    [Header("QTE Zones"), SerializeField]
    public List<GameObject> ZonesLvl1 = new List<GameObject>();
    public List<GameObject> ZonesLvl2 = new List<GameObject>();
    public List<GameObject> ZonesLvl3 = new List<GameObject>();

    [Header("QTE Sprites"), SerializeField]
    public List<GameObject> SpritesLvl1 = new List<GameObject>();
    public List<GameObject> SpritesLvl2 = new List<GameObject>();
    public List<GameObject> SpritesLvl3 = new List<GameObject>();
    private DataEntity CurrentEntity => CombatManager.SINGLETON?.currentTurnOrder.Count > 0
        ? CombatManager.SINGLETON.currentTurnOrder[0]
        : null;

    private void Awake()
    {
        if (SINGLETON == null)
        {
            SINGLETON = this;
        }
        else if (SINGLETON != this)
        {
            Destroy(gameObject);  // Si une autre instance existe déjà, on détruit l'objet.
        }
        LoadZonesFromChildren();
        SliderManager();

    }

    private void Start()
    {
        UltButton.interactable = false;
        StartCoroutine(CheckEntityChange());
        StartCoroutine(SyncSliderWithEntity());
    }

    private void LoadZonesFromChildren()
    {
        qteZones.Clear();

        if (zoneParent == null)
        {
            Debug.LogWarning("Zone parent non assigné !");
            return;
        }

        foreach (Transform child in zoneParent)
        {
            QTEZoneMarker marker = child.GetComponent<QTEZoneMarker>();
            if (marker != null)
            {
                float angle = child.eulerAngles.z;
                float halfSpan = marker.angleSpan / 2f;

                QTEZone zone = new QTEZone
                {
                    zoneName = marker.zoneName,
                    startAngle = (angle - halfSpan + 360f) % 360f,
                    endAngle = (angle + halfSpan) % 360f,
                    debugColor = marker.debugColor,
                    successZone = marker.successZone,
                    Affinity = marker.Affinity
                };
                qteZones.Add(zone);
            }
        }
    }
    public void PickAndAssignZones(DataEntity player)
    {
        List<List<Sprite>> ListAllSpriteLists = new List<List<Sprite>>() {};
        List<List<GameObject>> ListAllZoneLists = new List<List<GameObject>>() {};
        player.CptUltlvl = player.UltLvl_1 + player.UltLvl_2 + player.UltLvl_3 + player.UltLvl_4;
        List<int> UltLvls = new List<int>() { player.UltLvl_1, player.UltLvl_2, player.UltLvl_3, player.UltLvl_4 };
        for (int i = 1; i < 5; i++)
        {
            for (int j = 1; j < 4; j++)
            {
                if (UltLvls[i] >= j)
                {
                    GameObject Zone = ListAllZoneLists[j][Random.Range(0, ListAllZoneLists[j].Count)];
                    Zone.SetActive(true);
                    Zone.GetComponent<QTEZoneMarker>().Affinity = i;
                    Zone.GetComponent<SpriteRenderer>().sprite = ListAllSpriteLists[j][i];
                }
            }
        }
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
            if (CurrentEntity != null && (int)sliderUltimate.value != CurrentEntity.UltimateSlider)
            {
                sliderUltimate.value = CurrentEntity.UltimateSlider;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
    public void GainUltimateCharge(int chargeAmount)
    {
        if (CurrentEntity == null) return;
        CurrentEntity.UltimateSlider -= chargeAmount;
        if (CurrentEntity.UltimateSlider > 100)
        {
            CurrentEntity.UltimateSlider = 100;
        }
        sliderUltimate.value = CurrentEntity.UltimateSlider;
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
        CheckInputOnce.Reset();
        if (qteAnimator == null || qteUI == null)
        {
            Debug.LogWarning("QTE Animator ou UI non assigné.");
            return;
        }

        qteUI.SetActive(true);
        qteAnimator.speed = 1f;
        StartCoroutine(CheckQTEInput());
    }

    IEnumerator CheckQTEInput()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CheckInputOnce.Do(() =>
                {
                    qteAnimator.speed = 0f;
                    CurrentEntity.UltimateSlider = 100;
                    CurrentEntity.UltIsReady = false;
                    Debug.Log("QTE: Animation mise en pause par l'utilisateur.");
                    CheckPointerInZone();
                });
            }
            yield return null;
        }
    }

    private void CheckPointerInZone()
    {
        if (pointer == null || center == null || qteZones == null) return;

        float angle = pointer.eulerAngles.z;
        angle = (angle + 360f) % 360f;

        bool hitSuccess = false;

        foreach (var zone in qteZones)
        {
            bool inZone = IsAngleInRange(angle, zone.startAngle, zone.endAngle);

            Debug.DrawLine(center.position, center.position + Quaternion.Euler(0, 0, zone.startAngle) * Vector3.up * 2f, zone.debugColor, 1f);
            Debug.DrawLine(center.position, center.position + Quaternion.Euler(0, 0, zone.endAngle) * Vector3.up * 2f, zone.debugColor, 1f);

            if (inZone)
            {
                Debug.Log($"🎯 Le pointeur est dans la zone '{zone.zoneName}' ({(zone.successZone ? "réussite" : "échec")})");

                if (zone.successZone)
                    hitSuccess = true;
                CombatManager.SINGLETON.SetupNewAffinity(zone.Affinity);
            }
        }

        if (hitSuccess)
        {
            Debug.Log("QTE réussie !");
            // Logique de réussite

        }
        else
        {
            Debug.Log(" QTE ratée !");
            // Logique d'échec
        }
        CombatManager.SINGLETON.SetUltimate();
        CombatManager.SINGLETON.UseCapacity(GlobalVars.currentSelectedCapacity);
        QTEStop();

    }

    private void QTEStop()
    {
        qteUI.SetActive(false);

    }

    private bool IsAngleInRange(float angle, float start, float end)
    {
        angle = (angle + 360f) % 360f;
        start = (start + 360f) % 360f;
        end = (end + 360f) % 360f;

        if (start < end)
        {
            return angle >= start && angle <= end;
        }
        else
        {
            return angle >= start || angle <= end;
        }
    }
}
