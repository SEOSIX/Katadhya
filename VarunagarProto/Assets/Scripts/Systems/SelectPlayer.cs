using UnityEngine;

public class SelectPlayer : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.SINGLETON.entityHandler == null)
        {
            Debug.LogWarning("EntiityManager non assigné !");
            return;
        }

        var players = GameManager.SINGLETON.entityHandler.players;

        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            if (player != null && player.instance == this.gameObject)
            {
                Debug.Log($"[CLICK] Joueur sélectionné : {player.namE}");

                if (LifeEntity.SINGLETON != null)
                {
                    LifeEntity.SINGLETON.HealSpecificPlayer(i);
                    LifeEntity.SINGLETON.LifeManage();   
                }
                else
                {
                    return;
                }

                break;
            }
        }
    }
}