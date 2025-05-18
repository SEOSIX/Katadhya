using UnityEngine;

public class AffinityButton : MonoBehaviour
{
    public DataEntity entity;
    public QTEUpgrade qte;

    public void OnQTEButtonClicked()
    {
        AutelQTEUpgrade.Instance.SelectPlayer(entity, qte);
    }
}