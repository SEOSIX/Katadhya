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
            if (entityHandler.ennemies[i] != null && entityHandler.ennemies[i].UnitLife <= 0)
            {
                Debug.Log($"L'ennemi {entityHandler.ennemies[i].namE} est mort et va être détruit.");
                GameObject enemyInstance = entityHandler.ennemies[i].instance;
                CombatManager.SINGLETON.RemoveUnitFromList(entityHandler.ennemies[i]);
                Destroy(enemyInstance);
                if (i >= LifeEntity.SINGLETON.PlayerSliders.Length )
                {
                    continue;
                }
                GameObject enemySliderGO = LifeEntity.SINGLETON.enemySliders[i].gameObject;
                enemySliderGO.SetActive(false);
                CombatManager.SINGLETON.circles[i].SetActive(false);
            }
        }
    }
    
    public void DestroyDeadPlayers()
    {
        for (int i = 0; i < entityHandler.players.Length; i++)
        {
            if (entityHandler.players[i] != null && entityHandler.players[i].UnitLife <= 0)
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
    }

    void OnMouseDown()
    {
        CombatManager.SINGLETON.SelectEnemy(playerIndex);
    }
}