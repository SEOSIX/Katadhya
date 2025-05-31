using UnityEngine;
using TMPro;

public class CaurisManage : MonoBehaviour
{
    public static CaurisManage Instance { get; private set; }

    [Header("Références")]
    public GlobalPlayerData playerData;
    public TextMeshProUGUI caurisText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        UpdateCaurisDisplay();
    }

    public bool SpendCauris(int amount)
    {
        if (playerData.SpendGlobalCauris(amount))
        {
            UpdateCaurisDisplay();
            return true;
        }
        return false;
    }

    public void AddCauris(int amount)
    {
        playerData.AddGlobalCauris(amount);
        UpdateCaurisDisplay();
    }

    public void UpdateCaurisDisplay()
    {
        if (caurisText != null)
            caurisText.text = playerData.caurisCount.ToString();
    }
}