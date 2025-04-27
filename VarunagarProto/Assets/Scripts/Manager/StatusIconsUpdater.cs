using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


public class StatusIconsUpdater : MonoBehaviour
{
    public static StatusIconsUpdater SINGLETON { get; private set; }

    public GameObject[] TurnIcons;
    public Image[] MarkIconsPlayers;
    public Image[] MarkIconsEnemies;
    public Sprite[] MarkStates;
    public EntityHandler entiityHandler;

    private void SetTurnIcon(int index)
    {
        TurnIcons[index].SetActive(true);
    }

    private void SetMarkIcon(DataEntity Target, int value)
    {
        if (entiityHandler.ennemies.Contains(Target))
        {
            MarkIconsEnemies[entiityHandler.ennemies.IndexOf(Target)].sprite = MarkStates[value];
        }
        else
        {
            MarkIconsPlayers[entiityHandler.players.IndexOf(Target)].sprite = MarkStates[value];

        }
    }
    private void RemoveMarks(DataEntity Target)
    {
        if (entiityHandler.ennemies.Contains(Target))
        {
            MarkIconsEnemies[entiityHandler.ennemies.IndexOf(Target)].sprite = null;
        }
        else
        {
            MarkIconsPlayers[entiityHandler.players.IndexOf(Target)].sprite = null;

        }
    }
    private void ResetTurnIcons(int index)
    {
        for (int i = 0; i < entiityHandler.players.Count; i++)
        {
            TurnIcons[i].SetActive(false);
        }
    }

}
