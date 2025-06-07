using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StartClicker : MonoBehaviour
{
    public Button myButton;

    void Start()
    {
        myButton.onClick.Invoke();
    }
}
