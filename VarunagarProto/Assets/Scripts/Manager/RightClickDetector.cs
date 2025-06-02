using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightClickDetector : MonoBehaviour
{
    public string function;
    private GameObject button;

    public GameObject Button { get => button; set => button = value; }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CombatManager CM = CombatManager.SINGLETON;
            CM.Invoke(function, 0f);
        }
    }
}
