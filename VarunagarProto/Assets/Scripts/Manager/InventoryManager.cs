using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [Header("References")]
    public GlobalPlayerData playerData;
    public GameObject slotPrefab;
    public Transform gridParent;

    [Header("Debug")]
    private GameObject[,] slotObjects;

    void Start()
    {
        GenerateInventoryUI();
    }
    void GenerateInventoryUI()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
        int width = playerData.width;
        int height = playerData.height;
        slotObjects = new GameObject[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject slot = Instantiate(slotPrefab, gridParent);
                slotObjects[x, y] = slot;
                int value = playerData.grid[x, y];
                TMP_Text text = slot.GetComponentInChildren<TMP_Text>();
                if (text != null)
                {
                    text.text = value.ToString();
                }
                Image img = slot.GetComponent<Image>();
                if (img != null)
                {
                    img.color = value > 0 ? Color.yellow : Color.white;
                }
            }
        }
    }
}