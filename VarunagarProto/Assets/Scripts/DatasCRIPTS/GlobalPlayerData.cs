using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/GlobalPlayer", order = 4)]
public class GlobalPlayerData : ScriptableObject
{
    [Header("Cauris global")]
    public int caurisCount;

    [Header("Cauris par affinité (0 = Force, etc.)")]
    public int[] caurisPerAffinity = new int[4];

    [Header("Cauris de base")]
    public int[] baseCaurisPerAffinity = new int[4];
    public int baseCaurisCount;

    [Header("Inventory Dimensions")]
    public int width = 5;
    public int height = 5;

    [System.NonSerialized] public int[,] grid;
    [System.NonSerialized] public int[,] quantityGrid;

    [SerializeField] private int[] flatGrid;
    [SerializeField] private int[] flatQuantities;

    private void OnEnable()
    {
        LoadGrid();
        ResetAllCaurisToBase();
    }

    public void ResetAllCaurisToBase()
    {
        for (int i = 0; i < 4; i++)
            caurisPerAffinity[i] = baseCaurisPerAffinity[i];
        caurisCount = baseCaurisCount;
    }

    public bool CanAfford(int amount, int affinityIndex)
    {
        return caurisPerAffinity[affinityIndex] >= amount;
    }

    public bool SpendCauris(int amount, int affinityIndex)
    {
        if (CanAfford(amount, affinityIndex))
        {
            caurisPerAffinity[affinityIndex] -= amount;
            return true;
        }
        return false;
    }

    public void AddCauris(int amount, int affinityIndex)
    {
        caurisPerAffinity[affinityIndex] += amount;
    }

    public int GetCauris(int affinityIndex)
    {
        return caurisPerAffinity[affinityIndex];
    }

    public void AddGlobalCauris(int amount)
    {
        caurisCount += amount;
    }

    public bool SpendGlobalCauris(int amount)
    {
        if (caurisCount >= amount)
        {
            caurisCount -= amount;
            return true;
        }
        return false;
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
}
