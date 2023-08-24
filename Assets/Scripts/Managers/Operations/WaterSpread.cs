using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpread : MonoBehaviour
{
    private float[,] NewWaterHeight;

    [SerializeField]
    private float FlowSpeed = 1f;

    [SerializeField]
    private int OPS;
    private float RunTime;

    private bool doWaterMovement;

    public void UpdateSettings()
    {
        doWaterMovement = Settings.Instance.doWaterMovement;
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
        // Check if water should move
        if (!doWaterMovement)
            return;

        // Moves water each 1sec / OperationsPerSeconds
        RunTime += Time.deltaTime;
        if (RunTime <= 1f / OPS)
            return;

        RunTime = 0;



        LoadWaterHeightArray();

        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                CalculateWaterFluidPoint(x, y);
            }
        }

        SetWaterHeightArray();
    }



    public void LoadWaterHeightArray()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                NewWaterHeight[x, y] = World.Instance.Points[x, y].WaterHeight;
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

    private void CalculateWaterFluidPoint(int x, int y)
    {
        Point point = World.Instance.Points[x, y];
        if (World.Instance.InBounds(x - 1, y) && World.Instance.Points[x - 1, y].AbsoluteWaterHeight() < point.AbsoluteWaterHeight())
        {
            NewWaterHeight[x - 1, y] += FlowSpeed * point.WaterHeight / (8f);
            NewWaterHeight[x, y] -= FlowSpeed * point.WaterHeight / (8f);
        }
        if (World.Instance.InBounds(x + 1, y) && World.Instance.Points[x + 1, y].AbsoluteWaterHeight() < point.AbsoluteWaterHeight())
        {
            NewWaterHeight[x + 1, y] += FlowSpeed * point.WaterHeight / (8f);
            NewWaterHeight[x, y] -= FlowSpeed * point.WaterHeight / (8f);
        }
        if (World.Instance.InBounds(x, y - 1) && World.Instance.Points[x, y - 1].AbsoluteWaterHeight() < point.AbsoluteWaterHeight())
        {
            NewWaterHeight[x, y - 1] += FlowSpeed * point.WaterHeight / (8f);
            NewWaterHeight[x, y] -= FlowSpeed * point.WaterHeight / (8f);
        }
        if (World.Instance.InBounds(x, y + 1) && World.Instance.Points[x, y + 1].AbsoluteWaterHeight() < point.AbsoluteWaterHeight())
        {
            NewWaterHeight[x, y + 1] += FlowSpeed * point.WaterHeight / (8f);
            NewWaterHeight[x, y] -= FlowSpeed * point.WaterHeight / (8f);
        }
    }

}
