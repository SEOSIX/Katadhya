using System.Collections;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class QTE_System : MonoBehaviour
{
    public TextMeshProUGUI E_DisplayBox;
    public TextMeshProUGUI R_DisplayBox;
    public TextMeshProUGUI T_DisplayBox;
    [SerializeField] private GameObject qteTest;
    [SerializeField] private QTE_Controller qteController;
    
    public int QTEGen;
    public int WaitingForKey;
    public int CorrectKey;
    public int CountingDown;
    [SerializeField] private int _qteCount = 0;
    private const int MaxQTECount = 3;
    private Coroutine countdownCoroutine;
    
    public delegate void QTEEventHandler(bool isSuccess);
    public event QTEEventHandler OnQTEEnded;
    
    public int currentSequenceIndex = 0;
    public readonly KeyCode[] sequence = { KeyCode.E, KeyCode.R, KeyCode.T };

    public void QTE()
    {
        if (WaitingForKey == 0)
        {
            _qteCount = 0;
            currentSequenceIndex = 0;
            StartNewQTEInstance();
        }
    }

    private void StartNewQTEInstance()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }
        
        switch (sequence[currentSequenceIndex])
        {
            case KeyCode.E:
                E_DisplayBox.text = "E";
                QTEGen = 1;
                break;
            case KeyCode.R:
                R_DisplayBox.text = "R";
                QTEGen = 2;
                break;
            case KeyCode.T:
                T_DisplayBox.text = "T";
                QTEGen = 3;
                break;
        }

        WaitingForKey = 1;
        CountingDown = 1;
        countdownCoroutine = StartCoroutine(CountDown());
        
        if (qteController != null)
            qteController.ReactivateQTEDetection();
    }

     public IEnumerator KeyPressing()
    {
        if (CorrectKey == 1)
        {
            switch (sequence[currentSequenceIndex])
            {
                case KeyCode.E:
                    E_DisplayBox.color = Color.green;
                    break;
                case KeyCode.R:
                    R_DisplayBox.color = Color.green;
                    break;
                case KeyCode.T:
                    T_DisplayBox.color = Color.green;
                    break;
            }

            yield return new WaitForSeconds(0.5f);
            
            currentSequenceIndex++;
            E_DisplayBox.color = Color.black;
            R_DisplayBox.color = Color.black;
            T_DisplayBox.color = Color.black;

            if (currentSequenceIndex < sequence.Length)
            {
                StartNewQTEInstance();
            }
            else
            {
                OnQTEEnded?.Invoke(true);
                EndQTE();
            }
        }
        else if (CorrectKey == 2)
        {
            switch (sequence[currentSequenceIndex])
            {
                case KeyCode.E:
                    E_DisplayBox.color = Color.red;
                    break;
                case KeyCode.R:
                    R_DisplayBox.color = Color.red;
                    break;
                case KeyCode.T:
                    T_DisplayBox.color = Color.red;
                    break;
            }

            yield return new WaitForSeconds(0.5f);
            OnQTEEnded?.Invoke(false);
            EndQTE();
        }
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(2f);
        if (CountingDown == 1)
        {
            if (QTEGen == 1) E_DisplayBox.color = Color.red;
            else if (QTEGen == 2) R_DisplayBox.color = Color.red;
            else if (QTEGen == 3) T_DisplayBox.color = Color.red;
        
            yield return new WaitForSeconds(1f);
            E_DisplayBox.text = "";
            R_DisplayBox.text = "";
            T_DisplayBox.text = "";
            E_DisplayBox.color = Color.black;
            R_DisplayBox.color = Color.black;
            T_DisplayBox.color = Color.black;
        
            OnQTEEnded?.Invoke(false);
            EndQTE();
        }
    }

    public void EndQTE()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        QTEGen = 4;
        CountingDown = 2;
        WaitingForKey = 0;
        CountingDown = 1;
        qteTest.SetActive(false);
    }
    public void ResetQTECount()
    {
        _qteCount = 0;
    }
}