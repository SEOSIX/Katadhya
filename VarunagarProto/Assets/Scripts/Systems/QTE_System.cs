using System.Collections;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class QTE_System : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI E_DisplayBox;
    [SerializeField] private TextMeshProUGUI R_DisplayBox;
    [SerializeField] private TextMeshProUGUI T_DisplayBox;
    [SerializeField] private TextMeshProUGUI PassBox;
    [SerializeField] private GameObject qteTest;
    [SerializeField] private QTE_Controller qteController;
    
    public int QTEGen;
    public int WaitingForKey;
    public int CorrectKey;
    public int CountingDown;
    private int QTECount = 0;
    private const int MaxQTECount = 3;

    public void QTE()
    {
        if (WaitingForKey == 0)
        {
            StartCoroutine(CountDown());
            QTEGen = Random.Range(1, 4);
            CountingDown = 1;
            E_DisplayBox.text = "";
            R_DisplayBox.text = "";
            T_DisplayBox.text = "";

            if (QTEGen == 1)
            {
                WaitingForKey = 1;
                E_DisplayBox.text = "E";
            }
            if (QTEGen == 2)
            {
                WaitingForKey = 1;
                R_DisplayBox.text = "R";
            }
            if (QTEGen == 3)
            {
                WaitingForKey = 1;
                T_DisplayBox.text = "T";
            }
            if (qteController != null)
                qteController.ReactivateQTEDetection();
        }
    }

    public IEnumerator KeyPressing()
    {
        QTEGen = 4;
        if (CorrectKey == 1)
        {
            CountingDown = 2;
            PassBox.text = "good one";
            yield return new WaitForSeconds(1f);
            CorrectKey = 0;
            PassBox.text = "";
            E_DisplayBox.text = "";
            R_DisplayBox.text = "";
            T_DisplayBox.text = "";
            yield return new WaitForSeconds(1f);
            WaitingForKey = 0;
            CountingDown = 1;
        
            QTECount++;
            if (QTECount < MaxQTECount)
            {
                QTE();
            }
            else
            {
                qteTest.SetActive(false);
            }
        }
    
        if (CorrectKey == 2)
        {
            CountingDown = 2;
            PassBox.text = "noob";
            yield return new WaitForSeconds(1f);
            CorrectKey = 0;
            PassBox.text = "";
            E_DisplayBox.text = "";
            R_DisplayBox.text = "";
            T_DisplayBox.text = "";
            yield return new WaitForSeconds(1f);
            WaitingForKey = 0;
            CountingDown = 1;
            QTE();
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
            E_DisplayBox.text = "";
            R_DisplayBox.text = "";
            T_DisplayBox.text = "";
            yield return new WaitForSeconds(1f);
            WaitingForKey = 0;
            CountingDown = 1;
            QTE();
        }
    }
}