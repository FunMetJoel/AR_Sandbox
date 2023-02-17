using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidManager : MonoBehaviour
{
    private bool doWaterMovement;
    private bool doWaterSources;

    private int[,] NewWaterHeight;

    public float FlowSpeed = 10f;
    private float RunTime;

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
        UpdateSettings();
        Settings.Instance.SettingsChanged.AddListener(UpdateSettings);
        NewWaterHeight = new int[World.Instance.WorldSize.x, World.Instance.WorldSize.y];
    }
    // Update is called once per frame
    void Update()
    {
        if (doWaterMovement)
        {
            RunTime += Time.deltaTime * FlowSpeed;

            if (RunTime >= 10)
            {
                RunTime = 0;

                UpdateWater();
            }
        }

        if (doWaterSources)
        {
            foreach (Vector3Int Source in World.Instance.WaterSources)
            {
                World.Instance.Tiles[Source.x, Source.y].WaterHeight += Source.z;
            }
        }
    }

    [InspectorButton("DrainAllWater")]
    public bool DrainWater;
    public void DrainAllWater()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                World.Instance.Tiles[x, y].WaterHeight = 0;
            }
        }
    }

    public void UpdateWater()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                NewWaterHeight[x, y] = World.Instance.Tiles[x, y].WaterHeight;
            }
        }

        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                if (World.Instance.Tiles[x, y].WaterHeight >= 8)
                {

                    if (x - 1 > 0 && World.Instance.Tiles[x-1, y].TotalHeight() <= World.Instance.Tiles[x, y].TotalHeight())
                    {
                        NewWaterHeight[x - 1, y] += Mathf.RoundToInt((World.Instance.Tiles[x, y].TotalHeight() - World.Instance.Tiles[x - 1, y].TotalHeight())/5);
                        NewWaterHeight[x, y] -= Mathf.RoundToInt((World.Instance.Tiles[x, y].TotalHeight() - World.Instance.Tiles[x - 1, y].TotalHeight()) / 5);
                    }
                    if (x + 1 < World.Instance.WorldSize.x && World.Instance.Tiles[x + 1, y].TotalHeight() <= World.Instance.Tiles[x, y].TotalHeight())
                    {
                        NewWaterHeight[x + 1, y] += Mathf.RoundToInt((World.Instance.Tiles[x, y].TotalHeight() - World.Instance.Tiles[x + 1, y].TotalHeight()) / 5);
                        NewWaterHeight[x, y] -= Mathf.RoundToInt((World.Instance.Tiles[x, y].TotalHeight() - World.Instance.Tiles[x + 1, y].TotalHeight()) / 5);
                    }
                    if (y - 1 > 0 && World.Instance.Tiles[x, y - 1].TotalHeight() <= World.Instance.Tiles[x, y].TotalHeight())
                    {
                        NewWaterHeight[x, y - 1] += Mathf.RoundToInt((World.Instance.Tiles[x, y].TotalHeight() - World.Instance.Tiles[x, y - 1].TotalHeight()) / 5);
                        NewWaterHeight[x, y] -= Mathf.RoundToInt((World.Instance.Tiles[x, y].TotalHeight() - World.Instance.Tiles[x, y - 1].TotalHeight()) / 5);
                    }
                    if (y + 1 < World.Instance.WorldSize.y && World.Instance.Tiles[x, y + 1].TotalHeight() <= World.Instance.Tiles[x, y].TotalHeight())
                    {
                        NewWaterHeight[x, y + 1] += Mathf.RoundToInt((World.Instance.Tiles[x, y].TotalHeight() - World.Instance.Tiles[x, y + 1].TotalHeight()) / 5);
                        NewWaterHeight[x, y] -= Mathf.RoundToInt((World.Instance.Tiles[x, y].TotalHeight() - World.Instance.Tiles[x, y + 1].TotalHeight()) / 5);
                    }
                }
                else if (World.Instance.Tiles[x, y].WaterHeight > 0)
                {
                    Vector2Int LowestTile = new Vector2Int(x, y);
                    if (x - 1 > 0 && World.Instance.Tiles[x - 1, y].LandHeight + World.Instance.Tiles[x - 1, y].WaterHeight < World.Instance.Tiles[LowestTile.x, LowestTile.y].LandHeight + World.Instance.Tiles[LowestTile.x, LowestTile.y].WaterHeight)
                    {
                        LowestTile = new Vector2Int(x - 1, y);
                    }
                    if (x + 1 < World.Instance.WorldSize.x && World.Instance.Tiles[x + 1, y].LandHeight + World.Instance.Tiles[x + 1, y].WaterHeight < World.Instance.Tiles[LowestTile.x, LowestTile.y].LandHeight + World.Instance.Tiles[LowestTile.x, LowestTile.y].WaterHeight)
                    {
                        LowestTile = new Vector2Int(x + 1, y);
                    }
                    if (y - 1 > 0 && World.Instance.Tiles[x, y - 1].LandHeight + World.Instance.Tiles[x, y - 1].WaterHeight < World.Instance.Tiles[LowestTile.x, LowestTile.y].LandHeight + World.Instance.Tiles[LowestTile.x, LowestTile.y].WaterHeight)
                    {
                        LowestTile = new Vector2Int(x, y - 1);
                    }
                    if (y + 1 < World.Instance.WorldSize.y && World.Instance.Tiles[x, y + 1].LandHeight + World.Instance.Tiles[x, y + 1].WaterHeight < World.Instance.Tiles[LowestTile.x, LowestTile.y].LandHeight + World.Instance.Tiles[LowestTile.x, LowestTile.y].WaterHeight)
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
                World.Instance.Tiles[x, y].WaterHeight = NewWaterHeight[x, y];
            }
        }
    }
}
