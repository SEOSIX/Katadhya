using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusIconsUpdater : MonoBehaviour
{
    public static StatusIconsUpdater SINGLETON { get; private set; }

    public GameObject[] TurnIcons;

    private void SetTurnIcon(int index)
    {
        TurnIcons[index].SetActive(true);
    }
}
