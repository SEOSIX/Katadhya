using TMPro;
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
            var enemy = entityHandler.ennemies[i];
            if (enemy == null || enemy.UnitLife > 0)
                continue;

            Debug.Log($"L'ennemi {enemy.namE} est mort et va être détruit.");
            GameObject enemyInstance = enemy.instance;
            CombatManager.SINGLETON.RemoveUnitFromList(enemy);
            Destroy(enemyInstance);

            if (i < LifeEntity.SINGLETON.enemySliders.Length)
            {
                GameObject enemySliderGO = LifeEntity.SINGLETON.enemySliders[i].gameObject;
                enemySliderGO.SetActive(false);
                CombatManager.SINGLETON.circlesEnnemy[i].SetActive(false);
            }
            entityHandler.ennemies[i] = null;
        }
    }
    
    public void DestroyDeadPlayers()
    {
        for (int i = 0; i < entityHandler.players.Length; i++)
        {
            Debug.Log($"Le jouer {entityHandler.players[i].namE} est mort et va être détruit.");
            GameObject PlayerInstance = entityHandler.players[i].instance;
            CombatManager.SINGLETON.RemoveUnitFromList(entityHandler.players[i]);
            Destroy(PlayerInstance);
            if (i >= LifeEntity.SINGLETON.PlayerSliders.Length )
            {
                continue;
            }
            GameObject PlayerSliderGO = LifeEntity.SINGLETON.PlayerSliders[i].gameObject;
            PlayerSliderGO.SetActive(false);
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

    private void Update()
    {
        LifeEntity.SINGLETON.LifeManage();
        for (int i = 0; i < entityHandler.ennemies.Length; i++)
        {
            if (entityHandler.ennemies[i] != null && entityHandler.ennemies[i].UnitLife <= 0)
            {
                DestroyDeadEnemies();
            }
        }
        for (int i = 0; i < entityHandler.players.Length; i++)
        {
            if (entityHandler.players[i] != null && entityHandler.players[i].UnitLife <= 0)
            {
                DestroyDeadPlayers();
            }
        }
        if (entityHandler.players.Length == 0)
        {
            Debug.Log("enemis ont gagnés");
        }
        else if (entityHandler.ennemies.Length == 0)
        {
            Debug.Log("players ont gagnés");
        }
    }

    void OnMouseDown()
    {
        CombatManager.SINGLETON.SelectEnemy(playerIndex);
        CombatManager.SINGLETON.SelectAlly(playerIndex);
    }
}