using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    public GameObject creditsPanel;
    public GameObject menuPanel;


    public void OuvrirCredits()
    {
        creditsPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void FermerCredits()
    {
        creditsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
