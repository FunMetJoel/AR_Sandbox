using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterWaterSpread : MonoBehaviour
{
    private float[,] NewWaterHeight;

    public float diffusionRate = 0.1f;
    public float advectionRate = 0.1f;

    [SerializeField]
    private float FlowSpeed = 1f;
    private int OPS;
    private float RunTime;

    public void UpdateSettings()
    {
        OPS = Settings.Instance.OPS;
    }

    void Start()
    {
        // Settings stuf
        UpdateSettings();
        Settings.Instance.SettingsChanged.AddListener(UpdateSettings);

        // Populate array
        NewWaterHeight = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y];
    }

    void Update()
    {
        // Moves water each 1sec / OperationsPerSeconds
        RunTime += Time.deltaTime;
        if (RunTime <= 1f / OPS)
            return;

        RunTime = 0;

        Diffuse();
        Advect();
    }

    public void Diffuse()
    {
        for (int x = 0; x < World.Instance.WorldSize.x; x++)
        {
            for (int y = 0; y < World.Instance.WorldSize.y; y++)
            {
                float landDiffusionFactor = World.Instance.Points[x, y].LandHeight;
                NewWaterHeight[x, y] = World.Instance.Points[x, y].WaterHeight
                    + diffusionRate * (GetNeighborWaterHeight(x, y) - 4 * World.Instance.Points[x, y].WaterHeight) * landDiffusionFactor;
            }
        }

        SwapWaterHeightArrays();
    }

    private float GetNeighborWaterHeight(int x, int y)
    {
        float sum = 0;
        int count = 0;

        if (x > 0)
        {
            sum += World.Instance.Points[x - 1, y].WaterHeight;
            count++;
        }
        if (x < World.Instance.WorldSize.x - 1)
        {
            sum += World.Instance.Points[x + 1, y].WaterHeight;
            count++;
        }
        if (y > 0)
        {
            sum += World.Instance.Points[x, y - 1].WaterHeight;
            count++;
        }
        if (y < World.Instance.WorldSize.y - 1)
        {
            sum += World.Instance.Points[x, y + 1].WaterHeight;
            count++;
        }

        return sum / count;
    }

    private void SwapWaterHeightArrays()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                float temp = World.Instance.Points[x, y].WaterHeight;
                World.Instance.Points[x, y].WaterHeight = NewWaterHeight[x, y];
                NewWaterHeight[x, y] = temp;
            }
        }
        
    }

    public void Advect()
    {
        for (int x = 0; x < World.Instance.WorldSize.x; x++)
        {
            for (int y = 0; y < World.Instance.WorldSize.y; y++)
            {
                int prevX = Mathf.Clamp(x - (int)(advectionRate * World.Instance.Points[x, y].WaterHeight), 0, World.Instance.WorldSize.x - 1);
                int prevY = Mathf.Clamp(y - (int)(advectionRate * World.Instance.Points[x, y].WaterHeight), 0, World.Instance.WorldSize.y - 1);

                NewWaterHeight[x, y] = World.Instance.Points[prevX, prevY].WaterHeight;
            }
        }

        SwapWaterHeightArrays();
    }
}

class FluidSimulation
{
    private int gridSizeX;
    private int gridSizeY;
    private float[,] waterHeight;
    private float[,] tempWaterHeight;
    private float[,] landHeight;

    private float diffusionRate = 0.1f;
    private float advectionRate = 0.1f;

    public FluidSimulation(int sizeX, int sizeY)
    {
        gridSizeX = sizeX;
        gridSizeY = sizeY;

        waterHeight = new float[sizeX, sizeY];
        tempWaterHeight = new float[sizeX, sizeY];
        landHeight = new float[sizeX, sizeY];
    }

    public void InitializeWaterHeight(float initialValue)
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                waterHeight[x, y] = initialValue;
            }
        }
    }

    public void SetLandHeight(int x, int y, float height)
    {
        if (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY)
        {
            landHeight[x, y] = height;
        }
    }

    public void Diffuse()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                float landDiffusionFactor = 1.0f - landHeight[x, y];
                tempWaterHeight[x, y] = waterHeight[x, y]
                    + diffusionRate * (GetNeighborWaterHeight(x, y) - 4 * waterHeight[x, y]) * landDiffusionFactor;
            }
        }

        SwapWaterHeightArrays();
    }

    public void Advect()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                int prevX = Mathf.Clamp(x - (int)(advectionRate * waterHeight[x, y]), 0, gridSizeX - 1);
                int prevY = Mathf.Clamp(y - (int)(advectionRate * waterHeight[x, y]), 0, gridSizeY - 1);

                tempWaterHeight[x, y] = waterHeight[prevX, prevY];
            }
        }

        SwapWaterHeightArrays();
    }

    public void SimulateStep()
    {
        Diffuse();
        Advect();
    }

    private float GetNeighborWaterHeight(int x, int y)
    {
        float sum = 0;
        int count = 0;

        if (x > 0)
        {
            sum += waterHeight[x - 1, y];
            count++;
        }
        if (x < gridSizeX - 1)
        {
            sum += waterHeight[x + 1, y];
            count++;
        }
        if (y > 0)
        {
            sum += waterHeight[x, y - 1];
            count++;
        }
        if (y < gridSizeY - 1)
        {
            sum += waterHeight[x, y + 1];
            count++;
        }

        return sum / count;
    }

    private void SwapWaterHeightArrays()
    {
        float[,] temp = waterHeight;
        waterHeight = tempWaterHeight;
        tempWaterHeight = temp;
    }
}
