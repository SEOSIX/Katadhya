using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [Header("References")]
    public GlobalPlayerData playerData;
    public GameObject slotPrefab;
    public Transform gridParent;

    [Header("Assets")]
    public Consumable[] allConsumables;

    private int[,] oldGrid;
    private GameObject[,] slotObjects;

    void Start()
    {
        InitGrids();
        GenerateInventoryUI();
    }

    void Update()
    {
        CheckGridChanges();
    }

    void InitGrids()
    {
        int width = playerData.width;
        int height = playerData.height;

        oldGrid = new int[width, height];
        slotObjects = new GameObject[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                oldGrid[x, y] = playerData.grid[x, y];
    }

    void GenerateInventoryUI()
    {
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        for (int y = 0; y < playerData.height; y++)
        {
            for (int x = 0; x < playerData.width; x++)
            {
                GameObject slot = Instantiate(slotPrefab, gridParent);
                slotObjects[x, y] = slot;
                RefreshSlot(x, y);
            }
        }
    }

    void CheckGridChanges()
    {
        for (int x = 0; x < playerData.width; x++)
        {
            for (int y = 0; y < playerData.height; y++)
            {
                if (playerData.grid[x, y] != oldGrid[x, y])
                {
                    RefreshSlot(x, y);
                    oldGrid[x, y] = playerData.grid[x, y];
                }
            }
        }
    }

    public void RefreshSlot(int x, int y)
    {
        if (x < 0 || x >= playerData.width || y < 0 || y >= playerData.height) return;

        GameObject slot = slotObjects[x, y];
        foreach (Transform child in slot.transform)
            Destroy(child.gameObject);

        int index = playerData.grid[x, y];
        Consumable c = GetConsumableByIndex(index);

        if (c != null && c.spriteRender != null)
        {
            GameObject iconGO = new GameObject("Icon");
            iconGO.transform.SetParent(slot.transform, false);

            Image iconImage = iconGO.AddComponent<Image>();
            iconImage.sprite = c.spriteRender;
            iconImage.preserveAspect = true;

            RectTransform rt = iconGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        TMP_Text text = slot.GetComponentInChildren<TMP_Text>();
        if (text != null)
            text.text = index.ToString();
    }

    Consumable GetConsumableByIndex(int index)
    {
        foreach (var c in allConsumables)
        {
            if (c != null && c.IndexRef == index)
                return c;
        }
        return null;
    }
}
