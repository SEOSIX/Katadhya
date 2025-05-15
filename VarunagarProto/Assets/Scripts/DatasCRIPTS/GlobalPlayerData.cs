using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/GlobalPlayer", order = 4)]
public class GlobalPlayerData : ScriptableObject
{
    [Header("Cauris")]
    public int caurisCount;
    
    [Header("InventoryLenght")]
    public int width = 5;
    public int height = 5;

    [System.NonSerialized]
    public int[,] grid;

    [SerializeField]
    private int[] flatGrid;

    private void OnEnable()
    {
        LoadGrid();
    }

    public void LoadGrid()
    {
        if (width <= 0 || height <= 0) return;

        grid = new int[width, height];
        if (flatGrid == null || flatGrid.Length != width * height)
        {
            flatGrid = new int[width * height];
        }
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                grid[x, y] = flatGrid[index];
            }
        }
    }
    public void SaveGrid()
    {
        flatGrid = new int[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                flatGrid[index] = grid[x, y];
            }
        }
    }
}