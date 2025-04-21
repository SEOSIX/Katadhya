using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using JetBrains.Annotations;
using System.Linq;

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
            if (i >= LifeEntity.SINGLETON.PlayerSliders.Length)
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
    private void RestoreShield()
    {
        for (int i = 0; i < entityHandler.ennemies.Length; i++)
        {
            if (entityHandler.ennemies[i] != null)
            {
                entityHandler.ennemies[i].UnitShield = 0;
            }
        }
        for (int i = 0; i < entityHandler.players.Length; i++)
        {
            if (entityHandler.players[i] != null)
            {
                entityHandler.players[i].UnitShield = 0;
            }
        }
    }
    void Start()
    {
        LifeEntity.SINGLETON.LifeManage();
        RestoreEnemiesLife();
        RestoreShield();
        AssignPlayerIndices();
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

    public void UpdateSpellData(DataEntity player)
    {
        CapacityData[] allData = Resources.LoadAll<CapacityData>("Data");
        Debug.Log($"{allData.Length}");
        Debug.Log($"{allData[20]}");
        Debug.Log($"{player.index} skibidi {player.Affinity}");
        player._CapacityData1 = allData.FirstOrDefault(d => d.name == $"Cpt{player.index}a{player.Affinity}");
        player._CapacityData2 = allData.FirstOrDefault(d => d.name == $"Cpt{player.index}b{player.Affinity}");
        player._CapacityData3 = allData.FirstOrDefault(d => d.name == $"Cpt{player.index}c{player.Affinity}");
    }
    void LateUpdate()
    {
        if(Input.GetKey("k"))
        {
            UpdateSpellData(entityHandler.players[1]);
            UpdateSpellData(entityHandler.players[0]);
        }
    }
    
    private void AssignPlayerIndices()
    {
        for (int i = 0; i < entityHandler.players.Length; i++)
        {
            if (entityHandler.players[i] != null)
            {
                EntiityManager manager = entityHandler.players[i].instance.GetComponent<EntiityManager>();
                if (manager != null)
                {
                    manager.playerIndex = i;
                    Debug.Log($"Index assigné à {entityHandler.players[i].namE} : {i}");
                }
                else
                {
                    Debug.LogWarning($"Aucun EntiityManager trouvé sur {entityHandler.players[i].namE}");
                }
            }
        }

        for (int i = 0; i < entityHandler.ennemies.Length; i++)
        {
            if (entityHandler.ennemies[i] != null)
            {
                EntiityManager manager = entityHandler.ennemies[i].instance.GetComponent<EntiityManager>();
                if (manager != null)
                {
                    manager.playerIndex = i;
                    Debug.Log($"Index assigné à ennemi {entityHandler.ennemies[i].namE} : {i}");
                }
                else
                {
                    Debug.LogWarning($"Aucun EntiityManager trouvé sur {entityHandler.ennemies[i].namE}");
                }
            }
        }
    }
}