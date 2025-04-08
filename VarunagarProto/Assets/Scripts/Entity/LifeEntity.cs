using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeEntity : MonoBehaviour
{
    [Header("Enemy Health Bars")] [Tooltip("Tableau de sliders (entre 1 et 4 éléments requis)")]
    public Slider[] enemySliders;
        public Slider[] PlayerSliders;
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
            for (int i = 0; i < entityHandler.players.Length; i++)
            {
                if (entityHandler.players[i] != null && PlayerSliders[i] != null)
                {
                    entityHandler.players[i].UnitLife = entityHandler.players[i].BaseLife;
                }
            }
        }

        public void LifeManage()
        {
            for (int i = 0; i < entityHandler.ennemies.Length && i < enemySliders.Length; i++)
            {
                if (entityHandler.ennemies[i] != null && enemySliders[i] != null)
                {
                    enemySliders[i].maxValue = entityHandler.ennemies[i].BaseLife;
                    enemySliders[i].value = entityHandler.ennemies[i].UnitLife;
                }
            }
            for (int i = 0; i < entityHandler.players.Length && i < PlayerSliders.Length; i++)
            {
                if (entityHandler.players[i] != null && PlayerSliders[i] != null)
                {
                    PlayerSliders[i].maxValue = entityHandler.players[i].BaseLife;
                    PlayerSliders[i].value = entityHandler.players[i].UnitLife;
                }
            }
        }
}
