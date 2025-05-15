using UnityEngine;
using UnityEngine.UI;

public class MarchandManager : MonoBehaviour
{
    [Header("Références")]
    public ConsumableHandler handler;
    public InventoryManager inventory;

    [Header("Boutons UI")]
    public Button[] buttons = new Button[3];

    void Start()
    {
        UpdateBoutonsMarchand();
    }

    void UpdateBoutonsMarchand()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;

            if (handler.objects.Length <= index || handler.objects[index] == null)
                continue;

            Consumable c = handler.objects[index];
            Image image = buttons[index].GetComponent<Image>();
            if (image != null && c.spriteRender != null)
            {
                image.sprite = c.spriteRender;
                image.preserveAspect = true;
            }
            buttons[index].onClick.RemoveAllListeners();
            buttons[index].onClick.AddListener(() => AddInInventory(c));
        }
    }

    void AddInInventory(Consumable c)
    {
        for (int y = 0; y < inventory.playerData.height; y++)
        {
            for (int x = 0; x < inventory.playerData.width; x++)
            {
                if (inventory.playerData.grid[x, y] == c.IndexRef &&
                    inventory.playerData.quantityGrid[x, y] < 3)
                {
                    inventory.playerData.quantityGrid[x, y]++;
                    inventory.RefreshSlot(x, y);
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
                    return;
                }
            }
        }

        Debug.Log("Inventaire plein !");
    }

}