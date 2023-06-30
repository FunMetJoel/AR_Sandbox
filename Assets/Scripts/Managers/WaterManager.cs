using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    private bool doWaterMovement;
    private bool doWaterSources;
    private bool doIceForming;
    private bool doGroundHumidity;

    private float[,] NewWaterHeight;

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
        }
        else
        {
            doWaterSources = false;
            doIceForming = false;
            doGroundHumidity = false;
        }
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
        NewWaterHeight = GetWaterHeightArray();

        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                if (World.Instance.InBounds(x - 1, y) && World.Instance.Points[x - 1, y].AbsoluteWaterHeight() < World.Instance.Points[x, y].AbsoluteWaterHeight())
                {
                    NewWaterHeight[x - 1, y] += World.Instance.Points[x, y].WaterHeight / 8f;
                    NewWaterHeight[x, y] -= World.Instance.Points[x, y].WaterHeight / 8f;
                }
                if (World.Instance.InBounds(x + 1, y) && World.Instance.Points[x + 1, y].AbsoluteWaterHeight() < World.Instance.Points[x, y].AbsoluteWaterHeight())
                {
                    NewWaterHeight[x + 1, y] += World.Instance.Points[x, y].WaterHeight / 8f;
                    NewWaterHeight[x, y] -= World.Instance.Points[x, y].WaterHeight / 8f;
                }
                if (World.Instance.InBounds(x, y - 1) && World.Instance.Points[x, y - 1].AbsoluteWaterHeight() < World.Instance.Points[x, y].AbsoluteWaterHeight())
                {
                    NewWaterHeight[x, y - 1] += World.Instance.Points[x, y].WaterHeight / 8f;
                    NewWaterHeight[x, y] -= World.Instance.Points[x, y].WaterHeight / 8f;
                }
                if (World.Instance.InBounds(x, y + 1) && World.Instance.Points[x, y + 1].AbsoluteWaterHeight() < World.Instance.Points[x, y].AbsoluteWaterHeight())
                {
                    NewWaterHeight[x, y + 1] += World.Instance.Points[x, y].WaterHeight / 8f;
                    NewWaterHeight[x, y] -= World.Instance.Points[x, y].WaterHeight / 8f;
                }

                if (doIceForming)
                {
                    NewWaterHeight[x, y] -= CalculateGndHumidity(World.Instance.Points[x, y]);
                    World.Instance.Points[x, y].GroundHumidity += CalculateGndHumidity(World.Instance.Points[x, y]);
                }

                if (doGroundHumidity)
                {
                    NewWaterHeight[x, y] -= CalculateIce(World.Instance.Points[x, y]);
                    World.Instance.Points[x, y].IceHeight += CalculateIce(World.Instance.Points[x, y]);
                }
            }
        }

        // Sets water height values
        SetWaterHeightArray(NewWaterHeight);
    }

    private float CalculateIce(Point point)
    {
        if(point.Temperature < 0)
        {
            return point.WaterHeight * (-point.Temperature / 10000000f) ;
        }
        return 0;
    }

    private float CalculateGndHumidity(Point point)
    {
        if (point.GroundHumidity < 0.03f)
        {
            return point.WaterHeight * (1f / (point.GroundHumidity + 0.1f)) / 10000f;
        }
        return 0;
    }

    public float[,] GetWaterHeightArray()
    {
        float[,] Waterheight = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y];
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                Waterheight[x, y] = World.Instance.Points[x, y].WaterHeight;
            }
        }

        return Waterheight;
    }

    public void SetWaterHeightArray(float[,] Waterheight)
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                World.Instance.Points[x, y].WaterHeight = Waterheight[x, y];
            }
        }
    }

}
