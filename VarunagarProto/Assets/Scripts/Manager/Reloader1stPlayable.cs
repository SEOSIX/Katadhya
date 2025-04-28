using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reloader1stPlayable : MonoBehaviour
{/*
    void SpawnNewEnemies()
    {
        GameObject enemyPrefab = Resources.Load<GameObject>("EnemyPrefab"); // Example path

        for (int i = 0; i < 2; i++) 
        {
            // Instantiate new enemy GameObject
            GameObject newEnemyGO = Instantiate(enemyPrefab, GetEnemySpawnPosition(i), Quaternion.identity);

            // Create a new DataEntity
            DataEntity newEnemy = new DataEntity();
            newEnemy.namE = $"Enemy {i + 1}";
            newEnemy.BaseLife = 100;       // You can randomize or use templates
            newEnemy.UnitLife = 100;
            newEnemy.UnitShield = 0;
            newEnemy.instance = newEnemyGO;

            // Attach EntiityManager for click/select logic
            EntiityManager manager = newEnemyGO.GetComponent<EntiityManager>();
            if (manager != null)
            {
                manager.entityHandler = entityHandler; // Link to the current entity handler
                manager.playerIndex = i;
            }

            // Add to the list
            entityHandler.ennemies.Add(newEnemy);
        }

        // Reassign everything clean
        AssignPlayerIndices();
        LifeEntity.SINGLETON.LifeManage();
    }*/
}
