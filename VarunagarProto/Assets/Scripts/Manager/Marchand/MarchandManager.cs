using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarchandManager : MonoBehaviour
{
    [Header("Références")]
    public ConsumableHandler handler;
    public InventoryManager inventory;
    public GlobalPlayerData caurisManager;

    public Material GreyScale;

    [Header("Boutons UI")]
    public Button[] buttons = new Button[3];

    [Header("UI Monnaie")]
    public TMP_Text caurisText;

    void Start()
    {
        ResetQuantites();
        UpdateCaurisText();
        UpdateBoutonsMarchand();
    }

    void Update()
    {
        UpdateCaurisText();
        UpdateBoutonsMarchand();
    }

    void UpdateCaurisText()
    {
        if (caurisText != null)
        {
            caurisText.text = caurisManager.caurisCount.ToString();
        }
    }

    void ResetQuantites()
    {
        foreach (var consumable in handler.objects)
        {
            if (consumable != null)
            {
                consumable.quantiteDisponible = consumable.baseQuantity;
            }
        }

        caurisManager.ResetAllCaurisToBase();
    }

    void UpdateBoutonsMarchand()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;

            if (handler.objects.Length <= index || handler.objects[index] == null)
                continue;

            Consumable c = handler.objects[index];
            Button button = buttons[index];
            Image image = button.GetComponent<Image>();

            if (image != null && c.spriteRender != null)
            {
                image.sprite = c.spriteRender;
                image.preserveAspect = true;
                image.material = (c.quantiteDisponible <= 0) ? GreyScale : null;
            }

            // Trouve ou crée PriceText
            TextMeshProUGUI priceText = button.transform.Find("PriceText")?.GetComponent<TextMeshProUGUI>();
            if (priceText == null)
            {
                GameObject priceTextGO = new GameObject("PriceText", typeof(RectTransform));
                priceTextGO.transform.SetParent(button.transform, false);
                priceText = priceTextGO.AddComponent<TextMeshProUGUI>();
                priceText.fontSize = 20;
                priceText.alignment = TextAlignmentOptions.BottomRight;
                RectTransform rt = priceText.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = new Vector2(10, 10);
                rt.offsetMax = new Vector2(-10, -10);
            }

            priceText.text = c.prix.ToString();
            priceText.color = (caurisManager.caurisCount >= c.prix) ? Color.white : Color.red;

            // Trouve ou crée QtyText
            TextMeshProUGUI qtyText = button.transform.Find("QtyText")?.GetComponent<TextMeshProUGUI>();
            if (qtyText == null)
            {
                GameObject qtyTextGO = new GameObject("QtyText", typeof(RectTransform));
                qtyTextGO.transform.SetParent(button.transform, false);
                qtyText = qtyTextGO.AddComponent<TextMeshProUGUI>();
                qtyText.fontSize = 30;
                qtyText.alignment = TextAlignmentOptions.TopRight;
                RectTransform rt = qtyText.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = new Vector2(10, 10);
                rt.offsetMax = new Vector2(-10, -10);
            }

            qtyText.text = $"x{c.quantiteDisponible}";
            qtyText.color = (c.quantiteDisponible <= 0) ? Color.red : Color.yellow;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => AcheterObjet(c));
        }
    }

    void AcheterObjet(Consumable c)
    {
        if (c.quantiteDisponible <= 0)
        {
            Debug.Log("Objet épuisé !");
            return;
        }

        if (caurisManager.caurisCount < c.prix)
        {
            Debug.Log("Pas assez de cauris !");
            return;
        }

        caurisManager.caurisCount -= c.prix;
        c.quantiteDisponible--;

        // Placement dans l'inventaire
        for (int y = 0; y < inventory.playerData.height; y++)
        {
            for (int x = 0; x < inventory.playerData.width; x++)
            {
                if (inventory.playerData.grid[x, y] == c.IndexRef &&
                    inventory.playerData.quantityGrid[x, y] < 3)
                {
                    inventory.playerData.quantityGrid[x, y]++;
                    inventory.RefreshSlot(x, y);
                    UpdateBoutonsMarchand();
                    return;
                }
            }
        }

        for (int y = 0; y < inventory.playerData.height; y++)
        {
            for (int x = 0; x < inventory.playerData.width; x++)
            {
                if (inventory.playerData.grid[x, y] == 0)
                {
                    inventory.playerData.grid[x, y] = c.IndexRef;
                    inventory.playerData.quantityGrid[x, y] = 1;
                    inventory.RefreshSlot(x, y);
                    UpdateBoutonsMarchand();
                    return;
                }
            }
        }

        Debug.Log("Inventaire plein !");
    }
}
