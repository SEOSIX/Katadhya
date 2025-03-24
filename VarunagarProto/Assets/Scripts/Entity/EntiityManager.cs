using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EntiityManager : MonoBehaviour
{
    public int playerIndex;
    [SerializeField] private EntityHandler entityHandler;
    public Slider[] enemySliders = new Slider[4];

    private int currentLifeValue;

    public void LifeManage()
    {
        DataEntity currentEntity = CombatManager.SINGLETON.currentTurnOrder[0];
        currentLifeValue = currentEntity.UnitLife;

        for (int i = 0; i < entityHandler.ennemies.Length && i < enemySliders.Length; i++)
        {
            enemySliders[i].maxValue = entityHandler.ennemies[i].UnitLife;
            enemySliders[i].value = enemySliders[i].maxValue;
        }
    }
    
    public void DestroyDeadEnemies()
    {
        for (int i = 0; i < entityHandler.ennemies.Length; i++)
        {
            if (entityHandler.ennemies[i] != null && entityHandler.ennemies[i].UnitLife <= 0)
            {
                Debug.Log($"L'ennemi {entityHandler.ennemies[i].namE} est mort et va être détruit.");
                GameObject enemyInstance = entityHandler.ennemies[i].instance;
                CombatManager.SINGLETON.RemoveUnitFromList(entityHandler.ennemies[i]);
                Destroy(enemyInstance);
            }
        }
    }
    
    private void RestoreEnemiesLife()
    {
        for (int i = 0; i < entityHandler.ennemies.Length; i++)
        {
            if (entityHandler.ennemies[i] != null)
            {
                entityHandler.ennemies[i].UnitLife = entityHandler.ennemies[i].BaseLife;
            }
        }
    }

    void Start()
    {
        LifeManage();
        RestoreEnemiesLife();
    }

    void OnMouseDown()
    {
        CombatManager.SINGLETON.SelectEnemy(playerIndex);
    }
}