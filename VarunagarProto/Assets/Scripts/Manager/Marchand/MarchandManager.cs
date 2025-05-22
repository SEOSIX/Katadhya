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
        UpdateBoutonsMarchand();
        UpdateCaurisText();
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
    caurisManager.ResetCaurisToBase();

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
            foreach (Transform child in button.transform)
            {
                if (child.name == "PriceText" || child.name == "QtyText")
                    Destroy(child.gameObject);
            }

            GameObject priceTextGO = new GameObject("PriceText");
            priceTextGO.name = "PriceText";
            priceTextGO.transform.SetParent(button.transform, false);

            TextMeshProUGUI priceText = priceTextGO.AddComponent<TextMeshProUGUI>();
            priceText.text = c.prix.ToString();
            priceText.fontSize = 20;
            priceText.alignment = TextAlignmentOptions.BottomRight;
            priceText.color = caurisManager.CanAfford(c.prix) ? Color.white : Color.red;

            RectTransform rtPrice = priceText.GetComponent<RectTransform>();
            rtPrice.anchorMin = Vector2.zero;
            rtPrice.anchorMax = Vector2.one;
            rtPrice.offsetMin = new Vector2(10, 10);
            rtPrice.offsetMax = new Vector2(-10, -10);
            GameObject qtyTextGO = new GameObject("QtyText");
            qtyTextGO.name = "QtyText";
            qtyTextGO.transform.SetParent(button.transform, false);

            TextMeshProUGUI qtyText = qtyTextGO.AddComponent<TextMeshProUGUI>();
            qtyText.text = $"x{c.quantiteDisponible}";
            qtyText.fontSize = 30;
            qtyText.alignment = TextAlignmentOptions.TopRight;
            qtyText.color = (c.quantiteDisponible <= 0) ? Color.red : Color.yellow;

            RectTransform rtQty = qtyText.GetComponent<RectTransform>();
            rtQty.anchorMin = Vector2.zero;
            rtQty.anchorMax = Vector2.one;
            rtQty.offsetMin = new Vector2(10, 10);
            rtQty.offsetMax = new Vector2(-10, -10);

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

        if (!caurisManager.CanAfford(c.prix))
        {
            Debug.Log("Pas assez de cauris !");
            return;
        }

        caurisManager.SpendCauris(c.prix);
        c.quantiteDisponible--;
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
