using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class LifeEntity : MonoBehaviour
{
    [Header("Enemy Health Bars")]
    [Tooltip("Tableau de sliders (entre 1 et 4 éléments requis)")]
    public Slider[] enemySliders;
    public Slider[] enemyShieldSliders;
    public TextMeshProUGUI[] enemyPVTexts;
    public Slider[] PlayerSliders;
    public Slider[] PlayerShieldSliders;
    public TextMeshProUGUI[] PlayerPVTexts;

    private int currentLifeValue;
    public EntityHandler entityHandler;

    public static LifeEntity SINGLETON { get; private set; }


    void Awake()
    {
        if (SINGLETON != null)
        {
            Destroy(SINGLETON.gameObject);
            return;
        }
        SINGLETON = this;
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < entityHandler.players.Count; i++)
        {
            if (entityHandler.players[i] != null && PlayerSliders[i] != null)
            {
                entityHandler.players[i].UnitLife = entityHandler.players[i].BaseLife;
            }
        }
    }

    public void LifeManage()
    {
        for (int i = 0; i < entityHandler.ennemies.Count && i < enemySliders.Length && i < enemyShieldSliders.Length; i++)
        {
            var enemy = entityHandler.ennemies[i];
            if (enemy != null && enemy.instance != null)
            {
                enemySliders[i].maxValue = enemy.BaseLife;
                enemySliders[i].value = Mathf.Max(0, enemy.UnitLife);
                enemyShieldSliders[i].maxValue = enemy.BaseLife;
                enemyShieldSliders[i].value = Mathf.Max(0, enemy.UnitShield);
                //enemyPVTexts[i].text = $"{Mathf.Max(0, enemy.UnitLife)} / {enemy.BaseLife}";
            }
        }

        for (int i = 0; i < entityHandler.players.Count && i < PlayerSliders.Length && i < PlayerShieldSliders.Length; i++)
        {
            var player = entityHandler.players[i];
            if (player != null && player.instance != null)
            {
                PlayerSliders[i].maxValue = player.BaseLife;
                PlayerSliders[i].value = Mathf.Max(0, player.UnitLife);
                PlayerShieldSliders[i].maxValue = player.BaseLife;
                PlayerShieldSliders[i].value = Mathf.Max(0, player.UnitShield);
                //PlayerPVTexts[i].text = $"{Mathf.Max(0, player.UnitLife)} / {player.BaseLife}";

            }
        }
    }
}
