using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class LifeEntity : MonoBehaviour
{
    [Header("Combat Settings")]
    public bool isCombatEnabled = true;
    
    
    [Header("Enemy Health Bars")]
    [Tooltip("Tableau de sliders (entre 1 et 4 éléments requis)")]
    public Slider[] enemySliders;
    public Slider[] enemyShieldSliders;
    public TextMeshProUGUI[] enemyPVTexts;
    public Slider[] PlayerSliders;
    public Slider[] PlayerShieldSliders;
    public TextMeshProUGUI[] PlayerPVTexts;

    public float[] healingPlayers;
    public float GlobalHealingAmount;

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
    private void Update()
    {
        
        for (int i = 0; i < entityHandler.players.Count && i < PlayerSliders.Length && i < PlayerPVTexts.Length; i++)
        {
            var currentEntity = entityHandler.players[i];
            if (currentEntity != null)
            {
                PlayerPVTexts[i].text = currentEntity.UnitLife + " / " + currentEntity.BaseLife;
                PlayerSliders[i].maxValue = currentEntity.BaseLife;
                PlayerSliders[i].value = currentEntity.UnitLife;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            for(int i =0; i<entityHandler.ennemies.Count; i++)
            {
                Debug.Log($"{entityHandler.ennemies[i].UnitLife}");
            }
        }
    }

    public void LifeManage()
    {
        int sliderIndex = 0;

        for (int i = 0; i < entityHandler.ennemies.Count && sliderIndex < enemySliders.Length; i++)
        {
            var enemy = entityHandler.ennemies[i];
            if (enemy != null && enemy.instance != null)
            {
                enemySliders[sliderIndex].gameObject.SetActive(true);
                enemyShieldSliders[sliderIndex].gameObject.SetActive(true);
                enemyPVTexts[sliderIndex].gameObject.SetActive(true);

                enemySliders[sliderIndex].maxValue = enemy.BaseLife;
                enemySliders[sliderIndex].value = Mathf.Max(0, enemy.UnitLife);

                enemyShieldSliders[sliderIndex].maxValue = enemy.BaseLife / 2;
                enemyShieldSliders[sliderIndex].value = Mathf.Max(0, enemy.UnitShield);

                enemyPVTexts[sliderIndex].text = $"{Mathf.Max(0, enemy.UnitLife)} / {enemy.BaseLife}";

                sliderIndex++;
            }
        }

        for (int i = sliderIndex; i < enemySliders.Length; i++)
        {
            enemySliders[i].gameObject.SetActive(false);
            enemyShieldSliders[i].gameObject.SetActive(false);
            enemyPVTexts[i].gameObject.SetActive(false);
        }

        int playerSliderIndex = 0;

        for (int i = 0; i < entityHandler.players.Count && playerSliderIndex < PlayerSliders.Length; i++)
        {
            var player = entityHandler.players[i];
            if (player != null && player.instance != null)
            {
                PlayerSliders[playerSliderIndex].gameObject.SetActive(true);
                PlayerShieldSliders[playerSliderIndex].gameObject.SetActive(true);
                PlayerPVTexts[playerSliderIndex].gameObject.SetActive(true);

                PlayerSliders[playerSliderIndex].maxValue = player.BaseLife;
                PlayerSliders[playerSliderIndex].value = Mathf.Max(0, player.UnitLife);

                PlayerShieldSliders[playerSliderIndex].maxValue = player.BaseLife / 2;
                PlayerShieldSliders[playerSliderIndex].value = Mathf.Max(0, player.UnitShield);

                PlayerPVTexts[playerSliderIndex].text = $"{Mathf.Max(0, player.UnitLife)} / {player.BaseLife}";

                playerSliderIndex++;
            }
        }

        for (int i = playerSliderIndex; i < PlayerSliders.Length; i++)
        {
            PlayerSliders[i].gameObject.SetActive(false);
            PlayerShieldSliders[i].gameObject.SetActive(false);
            PlayerPVTexts[i].gameObject.SetActive(false);
        }
    }
    public void SetAllPlayersToOnePercentLife()
    {
        for (int i = 0; i < entityHandler.players.Count; i++)
        {
            var player = entityHandler.players[i];
            if (player != null)
            {
                player.UnitLife = Mathf.Max(1, (int)(player.BaseLife * 0.01f));
            }
        }
    }

    public void HealSpecificPlayer(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= entityHandler.players.Count) return;

        var player = entityHandler.players[playerIndex];
        if (player != null)
        {
            float healAmount = healingPlayers.Length > playerIndex ? healingPlayers[playerIndex] : 0f;

            player.UnitLife = Mathf.Min(player.BaseLife, player.UnitLife + (int)healAmount);
        }
    }

    public void HealAllPlayer()
    {
        HealingPlayer.HealAllPlayersByPercent(entityHandler, GlobalHealingAmount);
        LifeManage();
    }
}
