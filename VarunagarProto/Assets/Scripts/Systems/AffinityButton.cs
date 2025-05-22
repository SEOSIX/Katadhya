using UnityEngine;
using UnityEngine.UI;

public class AffinityButton : MonoBehaviour
{
    public DataEntity entity;
    public QTEUpgrade qte;
    public Animator ButtonAnimator;

    public void OnQTEButtonClicked()
    {
        AutelQTEUpgrade.Instance.SelectPlayer(entity, qte, ButtonAnimator);
    }
}