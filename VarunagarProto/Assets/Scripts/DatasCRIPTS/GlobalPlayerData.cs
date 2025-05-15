using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/GlobalPlayer", order = 4)]
public class GlobalPlayerData : ScriptableObject
{
    [Header("Cauris")]
    public int caurisCount;
    [Header("Cauris de base")]
    public int basCauris;
    
    [Header("InventoryLenght")]
    public int width = 5;
    public int height = 5;

    [System.NonSerialized]
    public int[,] grid;
    [System.NonSerialized]
    public int[,] quantityGrid;

    [SerializeField]
    private int[] flatGrid;
    [SerializeField]
    private int[] flatQuantities;

    private void OnEnable()
    {
        LoadGrid();
    }

    public void LoadGrid()
    {
        grid = new int[width, height];
        quantityGrid = new int[width, height];

        if (flatGrid == null || flatGrid.Length != width * height)
            flatGrid = new int[width * height];

        if (flatQuantities == null || flatQuantities.Length != width * height)
            flatQuantities = new int[width * height];

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            int index = y * width + x;
            grid[x, y] = flatGrid[index];
            quantityGrid[x, y] = flatQuantities[index];
        }
    }
    public void SaveGrid()
    {
        flatGrid = new int[width * height];
        flatQuantities = new int[width * height];

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            int index = y * width + x;
            flatGrid[index] = grid[x, y];
            flatQuantities[index] = quantityGrid[x, y];
        }
    }

    public bool CanAfford(int amount)
    {
        return caurisCount >= amount;
    }

    public bool SpendCauris(int amount)
    {
        if (CanAfford(amount))
        {
            caurisCount -= amount;
            return true;
        }
        return false;
    }

    public void AddCauris(int amount)
    {
        caurisCount += amount;
    }

    public void ResetCaurisToBase()
{
    caurisCount = basCauris;
}
}