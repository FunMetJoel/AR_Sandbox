using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    private bool doWaterMovement;
    private bool doWaterSources;
    private bool doIceForming;
    private bool doGroundHumidity;
    private bool doGroundHumidityMovement;

    private float[,] NewWaterHeight;
    public float[,] NewGroundHumidity;

    [SerializeField]
    private float FlowSpeed = 10f;
    private float RunTime; 
    private float SourceTime;

    public void UpdateSettings()
    {
        doWaterMovement = Settings.Instance.doWaterMovement;
        if (doWaterMovement)
        {
            doWaterSources = Settings.Instance.doWaterSources;
            doIceForming = Settings.Instance.doIceForming;
            doGroundHumidity = Settings.Instance.doGroundHumidity;
            doGroundHumidityMovement = Settings.Instance.doGroundHumidityMovement;
        }
        else
        {
            doWaterSources = false;
            doIceForming = false;
            doGroundHumidity = false;
            doGroundHumidityMovement = false;
        }
    }
    void Start()
    {
        // Settings stuf
        UpdateSettings();
        Settings.Instance.SettingsChanged.AddListener(UpdateSettings);

        // Populate array
        NewWaterHeight = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y];
        NewGroundHumidity = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y];
    }

    void Update()
    {
        // Check if water should move
        if (!doWaterMovement)
            return;

        // Moves water each sec/flowspeed
        RunTime += Time.deltaTime * FlowSpeed;
        if (RunTime >= 1)
        {
            RunTime = 0;

            NewFrame();
        }

        
        


        SourceTime += Time.deltaTime;
        if (doWaterSources && SourceTime >= 1f/100f)
        {
            SourceTime = 0;
            foreach (Vector3Int Source in World.Instance.WaterSources)
            {
                World.Instance.Points[Source.x, Source.y].WaterHeight += Source.z/60000f;
            }
        }

    }

    // Drains all water
    [InspectorButton("DrainAllWater")]
    public bool DrainWater;
    public void DrainAllWater()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                World.Instance.Points[x, y].WaterHeight = 0;
            }
        }
    }

    // Generates new water frame
    public void NewFrame()
    {
        // Gets water start values
        GetWaterHeightArray();
        GetGroundHumidityArray();

        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {

                CalculateWaterFluidPoint(x, y);

                if (doGroundHumidityMovement)
                    CalculateGroundFluidPoint(x, y);

                if (doIceForming)
                {
                    NewWaterHeight[x, y] -= CalculateGndHumidity(World.Instance.Points[x, y]);
                    NewGroundHumidity[x, y] += CalculateGndHumidity(World.Instance.Points[x, y]);
                }

                if (doGroundHumidity)
                {
                    NewWaterHeight[x, y] -= CalculateIce(World.Instance.Points[x, y]);
                    World.Instance.Points[x, y].IceHeight += CalculateIce(World.Instance.Points[x, y]);
                }
            }
        }

        // Sets water height values
        SetWaterHeightArray();
        SetGroundHumidityArray();
    }

    private float CalculateIce(Point point)
    {
        if (point.Temperature > 0f)
            return 0;

        return point.WaterHeight * (-point.Temperature / 10000000f) ;
    }

    private float CalculateGndHumidity(Point point)
    {
        if (point.GroundHumidity > 0.03f)
            return 0;
        
        return point.WaterHeight * (1f / (point.GroundHumidity + 0.1f)) / 1000f;
    }

    private void CalculateWaterFluidPoint(int x, int y)
    {
        Point point = World.Instance.Points[x, y];
        if (World.Instance.InBounds(x - 1, y) && World.Instance.Points[x - 1, y].AbsoluteWaterHeight() < point.AbsoluteWaterHeight())
        {
            NewWaterHeight[x - 1, y] += point.WaterHeight / (8f);
            NewWaterHeight[x, y] -= point.WaterHeight / (8f);
        }
        if (World.Instance.InBounds(x + 1, y) && World.Instance.Points[x + 1, y].AbsoluteWaterHeight() < point.AbsoluteWaterHeight())
        {
            NewWaterHeight[x + 1, y] += point.WaterHeight / (8f);
            NewWaterHeight[x, y] -= point.WaterHeight / (8f);
        }
        if (World.Instance.InBounds(x, y - 1) && World.Instance.Points[x, y - 1].AbsoluteWaterHeight() < point.AbsoluteWaterHeight())
        {
            NewWaterHeight[x, y - 1] += point.WaterHeight / (8f);
            NewWaterHeight[x, y] -= point.WaterHeight / (8f);
        }
        if (World.Instance.InBounds(x, y + 1) && World.Instance.Points[x, y + 1].AbsoluteWaterHeight() < point.AbsoluteWaterHeight())
        {
            NewWaterHeight[x, y + 1] += point.WaterHeight / (8f);
            NewWaterHeight[x, y] -= point.WaterHeight / (8f);
        }
    }

    private void CalculateGroundFluidPoint(int x, int y)
    {
        Point point = World.Instance.Points[x, y];
        if (World.Instance.InBounds(x - 1, y) && World.Instance.Points[x - 1, y].GroundHumidity < point.GroundHumidity)
        {
            NewGroundHumidity[x - 1, y] += point.GroundHumidity / (80000f);
            NewGroundHumidity[x, y] -= point.GroundHumidity / (80000f);
        }
        if (World.Instance.InBounds(x + 1, y) && World.Instance.Points[x + 1, y].GroundHumidity < point.GroundHumidity)
        {
            NewGroundHumidity[x + 1, y] += point.GroundHumidity / (80000f);
            NewGroundHumidity[x, y] -= point.GroundHumidity / (80000f);
        }
        if (World.Instance.InBounds(x, y - 1) && World.Instance.Points[x, y - 1].GroundHumidity < point.GroundHumidity)
        {   
            NewGroundHumidity[x, y - 1] += point.GroundHumidity / (80000f);
            NewGroundHumidity[x, y] -= point.GroundHumidity / (80000f);
        }
        if (World.Instance.InBounds(x, y + 1) && World.Instance.Points[x, y + 1].GroundHumidity < point.GroundHumidity)
        {
            NewGroundHumidity[x, y + 1] += point.GroundHumidity / (80000f);
            NewGroundHumidity[x, y] -= point.GroundHumidity / (80000f);
        }
    }

    public void GetWaterHeightArray()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                NewWaterHeight[x, y] = World.Instance.Points[x, y].WaterHeight;
            }
        }
    }

    public void GetGroundHumidityArray()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                NewGroundHumidity[x, y] = World.Instance.Points[x, y].GroundHumidity;
            }
        }
    }

    public void SetWaterHeightArray()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                World.Instance.Points[x, y].WaterHeight = NewWaterHeight[x, y];
            }
        }
    }

    public void SetGroundHumidityArray()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                World.Instance.Points[x, y].GroundHumidity = NewGroundHumidity[x, y];
            }
        }
    }

}
