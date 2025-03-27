using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class QTE_System : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI DisplayBox;
    [SerializeField]private TextMeshProUGUI PassBox;
    public int QTEGen;
    public int WaitingForKey;
    public int CorrectKey;
    public int CountingDown;


    private void Update()
    {
        QTE();
    }

    public void QTE()
    {
        if (WaitingForKey == 0)
        {
            StartCoroutine(CountDown());
            QTEGen = Random.Range(1, 4);
            CountingDown = 1;

            if (QTEGen == 1)
            {
                WaitingForKey = 1;
                DisplayBox.text = "E";
            }
            if (QTEGen == 2)
            {
                WaitingForKey = 1;
                DisplayBox.text = "R";
            }
            if (QTEGen == 3)
            {
                WaitingForKey = 1;
                DisplayBox.text = "T";
            }
        }

        if (QTEGen == 1)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    CorrectKey = 1;
                    StartCoroutine(KeyPressing());
                }
                else
                {
                    CorrectKey = 2;
                    StartCoroutine(KeyPressing());
                }
            }
        }
        if (QTEGen == 2)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    CorrectKey = 1;
                    StartCoroutine(KeyPressing());
                }
                else
                {
                    CorrectKey = 2;
                    StartCoroutine(KeyPressing());
                }
            }
        }
        if (QTEGen == 3)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    CorrectKey = 1;
                    StartCoroutine(KeyPressing());
                }
                else
                {
                    CorrectKey = 2;
                    StartCoroutine(KeyPressing());
                }
            }
        }
    }

    IEnumerator KeyPressing()
    {
        QTEGen = 4;
        if (CorrectKey == 1)
        {
            CountingDown = 2;
            PassBox.text = "good one";
            yield return new WaitForSeconds(1f);
            CorrectKey = 0;
            PassBox.text = "";
            DisplayBox.text = "";
            yield return new WaitForSeconds(1f);
            WaitingForKey = 0;
            CountingDown = 1;
        }
        
        if (CorrectKey == 2)
        {
            CountingDown = 2;
            PassBox.text = "noob";
            yield return new WaitForSeconds(1f);
            CorrectKey = 0;
            PassBox.text = "";
            DisplayBox.text = "";
            yield return new WaitForSeconds(1f);
            WaitingForKey = 0;
            CountingDown = 1;
        }
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(2f);
        if (CountingDown == 1)
        {
            QTEGen = 4;
            CountingDown = 2;
            PassBox.text = "noob";
            yield return new WaitForSeconds(1f);
            CorrectKey = 0;
            PassBox.text = "";
            DisplayBox.text = "";
            yield return new WaitForSeconds(1f);
            WaitingForKey = 0;
            CountingDown = 1;
        }
    }
}
