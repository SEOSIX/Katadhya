using UnityEngine;

public class QTE_Controller : MonoBehaviour
{
    [SerializeField] private QTE_System qteSystem;
    private bool isQTEActive = false;

    private void Update()
    {
        if (!isQTEActive) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleQTEEvent(KeyCode.E);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            HandleQTEEvent(KeyCode.R);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            HandleQTEEvent(KeyCode.T);
        }
    }

    public void StartQTE()
    {
        isQTEActive = true;
        qteSystem.QTE();
    }

    private void HandleQTEEvent(KeyCode keyPressed)
    {
        if (!isQTEActive) return;
        bool isCorrectKey = false;
        
        switch (qteSystem.QTEGen)
        {
            case 1 when keyPressed == KeyCode.E:
                isCorrectKey = true;
                break;
            case 2 when keyPressed == KeyCode.R:
                isCorrectKey = true;
                break;
            case 3 when keyPressed == KeyCode.T:
                isCorrectKey = true;
                break;
        }

        qteSystem.CorrectKey = isCorrectKey ? 1 : 2;
        StartCoroutine(qteSystem.KeyPressing());
        
        isQTEActive = false;
    }
    
    public void ReactivateQTEDetection()
    {
        isQTEActive = true;
    }
}
