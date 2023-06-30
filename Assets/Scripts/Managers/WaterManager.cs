using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    private bool doWaterMovement;
    private bool doWaterSources;

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
        }
        else
        {
            doWaterSources = false;
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

            UpdateWater3();
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
    public void UpdateWater()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                NewWaterHeight[x, y] = World.Instance.Points[x, y].WaterHeight;
            }
        }

        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                if (World.Instance.Points[x, y].WaterHeight >= 8)
                {

                    if (x - 1 > 0 && World.Instance.Points[x-1, y].TotalHeight() <= World.Instance.Points[x, y].TotalHeight())
                    {
                        NewWaterHeight[x - 1, y] += Mathf.RoundToInt((World.Instance.Points[x, y].TotalHeight() - World.Instance.Points[x - 1, y].TotalHeight())/5);
                        NewWaterHeight[x, y] -= Mathf.RoundToInt((World.Instance.Points[x, y].TotalHeight() - World.Instance.Points[x - 1, y].TotalHeight()) / 5);
                    }
                    if (x + 1 < World.Instance.WorldSize.x && World.Instance.Points[x + 1, y].TotalHeight() <= World.Instance.Points[x, y].TotalHeight())
                    {
                        NewWaterHeight[x + 1, y] += Mathf.RoundToInt((World.Instance.Points[x, y].TotalHeight() - World.Instance.Points[x + 1, y].TotalHeight()) / 5);
                        NewWaterHeight[x, y] -= Mathf.RoundToInt((World.Instance.Points[x, y].TotalHeight() - World.Instance.Points[x + 1, y].TotalHeight()) / 5);
                    }
                    if (y - 1 > 0 && World.Instance.Points[x, y - 1].TotalHeight() <= World.Instance.Points[x, y].TotalHeight())
                    {
                        NewWaterHeight[x, y - 1] += Mathf.RoundToInt((World.Instance.Points[x, y].TotalHeight() - World.Instance.Points[x, y - 1].TotalHeight()) / 5);
                        NewWaterHeight[x, y] -= Mathf.RoundToInt((World.Instance.Points[x, y].TotalHeight() - World.Instance.Points[x, y - 1].TotalHeight()) / 5);
                    }
                    if (y + 1 < World.Instance.WorldSize.y && World.Instance.Points[x, y + 1].TotalHeight() <= World.Instance.Points[x, y].TotalHeight())
                    {
                        NewWaterHeight[x, y + 1] += Mathf.RoundToInt((World.Instance.Points[x, y].TotalHeight() - World.Instance.Points[x, y + 1].TotalHeight()) / 5);
                        NewWaterHeight[x, y] -= Mathf.RoundToInt((World.Instance.Points[x, y].TotalHeight() - World.Instance.Points[x, y + 1].TotalHeight()) / 5);
                    }
                }
                else if (World.Instance.Points[x, y].WaterHeight > 0)
                {
                    Vector2Int LowestTile = new Vector2Int(x, y);
                    if (x - 1 > 0 && World.Instance.Points[x - 1, y].LandHeight + World.Instance.Points[x - 1, y].WaterHeight < World.Instance.Points[LowestTile.x, LowestTile.y].LandHeight + World.Instance.Points[LowestTile.x, LowestTile.y].WaterHeight)
                    {
                        LowestTile = new Vector2Int(x - 1, y);
                    }
                    if (x + 1 < World.Instance.WorldSize.x && World.Instance.Points[x + 1, y].LandHeight + World.Instance.Points[x + 1, y].WaterHeight < World.Instance.Points[LowestTile.x, LowestTile.y].LandHeight + World.Instance.Points[LowestTile.x, LowestTile.y].WaterHeight)
                    {
                        LowestTile = new Vector2Int(x + 1, y);
                    }
                    if (y - 1 > 0 && World.Instance.Points[x, y - 1].LandHeight + World.Instance.Points[x, y - 1].WaterHeight < World.Instance.Points[LowestTile.x, LowestTile.y].LandHeight + World.Instance.Points[LowestTile.x, LowestTile.y].WaterHeight)
                    {
                        LowestTile = new Vector2Int(x, y - 1);
                    }
                    if (y + 1 < World.Instance.WorldSize.y && World.Instance.Points[x, y + 1].LandHeight + World.Instance.Points[x, y + 1].WaterHeight < World.Instance.Points[LowestTile.x, LowestTile.y].LandHeight + World.Instance.Points[LowestTile.x, LowestTile.y].WaterHeight)
                    {
                        LowestTile = new Vector2Int(x, y + 1);
                    }

                    NewWaterHeight[x, y] -= 1;
                    NewWaterHeight[LowestTile.x, LowestTile.y] += 1;
                }
            }
        }

        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                World.Instance.Points[x, y].WaterHeight = NewWaterHeight[x, y];
            }
        }
    }

    public void UpdateWater2()
    {
        // Gets water start values
        NewWaterHeight = GetWaterHeightArray();

        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                int possibleFlowSides = 0;
                if (World.Instance.InBounds(x - 1, y) && World.Instance.Points[x - 1, y].AbsoluteWaterHeight() < World.Instance.Points[x, y].AbsoluteWaterHeight())
                    possibleFlowSides++;
                if (World.Instance.InBounds(x + 1, y) && World.Instance.Points[x + 1, y].AbsoluteWaterHeight() < World.Instance.Points[x, y].AbsoluteWaterHeight())
                    possibleFlowSides++;
                if (World.Instance.InBounds(x, y - 1) && World.Instance.Points[x, y - 1].AbsoluteWaterHeight() < World.Instance.Points[x, y].AbsoluteWaterHeight())
                    possibleFlowSides++;
                if (World.Instance.InBounds(x, y + 1) && World.Instance.Points[x, y + 1].AbsoluteWaterHeight() < World.Instance.Points[x, y].AbsoluteWaterHeight())
                    possibleFlowSides++;

                if(x == 20 && y == 23)
                {
                    Debug.Log(World.Instance.InBounds(20,23));
                    Debug.Log(World.Instance.Points[x - 1, y - 1].AbsoluteWaterHeight() < World.Instance.Points[x, y].AbsoluteWaterHeight());
                }

                if (possibleFlowSides > World.Instance.Points[x, y].WaterHeight || possibleFlowSides==0)
                    continue;
                
                if (World.Instance.InBounds(x - 1, y) && World.Instance.Points[x - 1, y].AbsoluteWaterHeight() < World.Instance.Points[x, y].AbsoluteWaterHeight())
                    NewWaterHeight[x - 1, y] ++;
                if (World.Instance.InBounds(x + 1, y) && World.Instance.Points[x + 1, y].AbsoluteWaterHeight() < World.Instance.Points[x, y].AbsoluteWaterHeight())
                    NewWaterHeight[x + 1, y]++;
                if (World.Instance.InBounds(x, y - 1) && World.Instance.Points[x, y - 1].AbsoluteWaterHeight() < World.Instance.Points[x, y].AbsoluteWaterHeight())
                    NewWaterHeight[x, y - 1]++;
                if (World.Instance.InBounds(x, y + 1) && World.Instance.Points[x, y + 1].AbsoluteWaterHeight() < World.Instance.Points[x, y].AbsoluteWaterHeight())
                    NewWaterHeight[x, y + 1]++;

                NewWaterHeight[x, y] -= possibleFlowSides;
            }
        }

        // Sets water height values
        SetWaterHeightArray(NewWaterHeight);
    }

    public void UpdateWater3()
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
            }
        }

        // Sets water height values
        SetWaterHeightArray(NewWaterHeight);
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
