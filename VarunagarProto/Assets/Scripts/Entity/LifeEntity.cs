using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeEntity : MonoBehaviour
{
      [Header("Enemy Health Bars")]
        [Tooltip("Tableau de sliders (entre 1 et 4 éléments requis)")]
        public Slider[] enemySliders = new Slider[4];
        public Slider[] PlayerSliders = new Slider[4];
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
        
        public void LifeManage()
        {
            DataEntity currentEntity = CombatManager.SINGLETON.currentTurnOrder[0];
            currentLifeValue = currentEntity.UnitLife;

            for (int i = 0; i < entityHandler.ennemies.Length && i < enemySliders.Length; i++)
            {
                enemySliders[i].maxValue = entityHandler.ennemies[i].UnitLife;
                enemySliders[i].value = enemySliders[i].maxValue;
            }
            for (int i = 0; i < entityHandler.players.Length && i < PlayerSliders.Length; i++)
            {
                PlayerSliders[i].maxValue = entityHandler.players[i].UnitLife;
                PlayerSliders[i].value = PlayerSliders[i].maxValue;
            }
        }
}
