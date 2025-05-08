using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class DammagesInstances : MonoBehaviour
{
    public static DammagesInstances singleton { get; private set; }
    public Transform Player1Damages;
    public Transform Player2Damages;
    public GameObject damageTextPrefab;
    
    private void Awake()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            singleton = this;
        }
    }
    
    public void InstanciateDammagesPlayer1(int damageValue)
    {
        GameObject dmgText = Instantiate(damageTextPrefab, Player1Damages);
        dmgText.GetComponent<TextMeshProUGUI>().text = "-" + damageValue;
        Destroy(dmgText, 1.5f);
    }

    private void Update()
    {
        int randomIndex = Random.Range(0, 300);
        if (Input.GetKeyDown((KeyCode.Space)))
        {
            InstanciateDammagesPlayer1(randomIndex);
        }
    }
}