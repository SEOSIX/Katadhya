using UnityEngine;

public class QTE_Controller : MonoBehaviour
{
    [SerializeField] private QTE_System qteSystem;
    private bool isQTEActive = false;


    public int AttackCount;
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
        bool isCorrectKey = (keyPressed == qteSystem.sequence[qteSystem.currentSequenceIndex]);
    
        qteSystem.CorrectKey = isCorrectKey ? 1 : 2;
        StartCoroutine(qteSystem.KeyPressing());
    
        isQTEActive = false;
    }
    
    public void ReactivateQTEDetection()
    {
        isQTEActive = true;
    }
    
    void OnEnable()
    {
        qteSystem.OnQTEEnded += HandleQTEResult;
    }

    void OnDisable()
    {
        qteSystem.OnQTEEnded -= HandleQTEResult;
    }

    private void HandleQTEResult(bool isSuccess)
    {
        if(isSuccess)
        {
            Debug.Log("Le joueur a réussi le QTE complet !");
            qteSystem.EndQTE();;
            CombatManager.SINGLETON.AttackSelectedEnemy(AttackCount);
            CombatManager.SINGLETON.EndUnitTurn();
        }
        else
        {
            Debug.Log("Le joueur a échoué le QTE");
        }
    }
    
    public void attackModifyer(bool attackValue)
    {
        
    }
}