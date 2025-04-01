using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EntiityManager : MonoBehaviour
{
    public int playerIndex;
    [Header("entityHandler")]
    public EntityHandler entityHandler;
    private int currentLifeValue;
    
    
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
                GameObject enemySliderGO = LifeEntity.SINGLETON.enemySliders[i].gameObject;
                enemySliderGO.SetActive(false);
                CombatManager.SINGLETON.circles[i].SetActive(false);
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
        LifeEntity.SINGLETON.LifeManage();
        RestoreEnemiesLife();
    }

    void OnMouseDown()
    {
        CombatManager.SINGLETON.SelectEnemy(playerIndex);
    }
}