using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeEntity : MonoBehaviour
{
    [Header("Enemy Health Bars")]
    [Tooltip("Tableau de sliders (entre 1 et 4 éléments requis)")]
    public Slider[] enemySliders;
    public Slider[] enemyShieldSliders;
    public Slider[] PlayerSliders;
    public Slider[] PlayerShieldSliders;

    private int currentLifeValue;
    public EntityHandler entityHandler;

    public static LifeEntity SINGLETON { get; private set; }


    void Awake()
    {
        if (SINGLETON != null)
        {
            Debug.LogError("Trying to instantiate another CombatManager SINGLETON");
            Destroy(gameObject);
            return;
        }
        SINGLETON = this;
        DontDestroyOnLoad(gameObject);
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
            if (entityHandler.ennemies[i] != null && enemySliders[i] != null && enemyShieldSliders != null)
            {
                int old_i = i;
                if (entityHandler.ennemies.Count==1)
                {
                    i = 1;
                }
                enemySliders[i].maxValue = entityHandler.ennemies[old_i].BaseLife;
                enemySliders[i].value = entityHandler.ennemies[old_i].UnitLife;
                enemyShieldSliders[i].maxValue = entityHandler.ennemies[old_i].BaseLife;
                enemyShieldSliders[i].value = entityHandler.ennemies[old_i].UnitShield;
            }
        }
        for (int i = 0; i < entityHandler.players.Count && i < PlayerSliders.Length && i < PlayerShieldSliders.Length; i++)
        {
            if (entityHandler.players[i] != null && PlayerSliders[i] != null && PlayerShieldSliders != null)
            {
                PlayerSliders[i].maxValue = entityHandler.players[i].BaseLife;
                PlayerSliders[i].value = entityHandler.players[i].UnitLife;
                PlayerShieldSliders[i].maxValue = entityHandler.players[i].BaseLife;
                PlayerShieldSliders[i].value = entityHandler.players[i].UnitShield;
            }
        }
    }
}
