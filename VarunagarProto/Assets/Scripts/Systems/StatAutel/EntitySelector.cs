using UnityEngine;
using UnityEngine.UI;

public class EntitySelector : MonoBehaviour
{
    public DataEntity entity;
    public Animator animator;

    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(Select);
    }

    private void Select()
    {
        AutelStats.Instance.SelectEntity(entity, animator);
    }
}