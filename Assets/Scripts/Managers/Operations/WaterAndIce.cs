using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAndIce : MonoBehaviour
{
    private float[,] NewIceHeight;
    private float[,] NewWaterHeight;

    [SerializeField]
    private float FlowSpeed = 1f;
    private int OPS;
    private float RunTime;

    private bool doIceForming;

    public void UpdateSettings()
    {
        doIceForming = Settings.Instance.doIceForming;
        OPS = Settings.Instance.OPS;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Settings stuf
        UpdateSettings();
        Settings.Instance.SettingsChanged.AddListener(UpdateSettings);

        // Populate array
        NewWaterHeight = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y];
        NewIceHeight = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y];
    }

    // Update is called once per frame
    void Update()
    {
        // Check if water should move
        if (!doIceForming)
            return;

        // Moves water each 1sec / OperationsPerSeconds
        RunTime += Time.deltaTime;
        if (RunTime <= 1f / OPS)
            return;

        RunTime = 0;

        LoadDataArray();

        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                NewWaterHeight[x, y] -= FlowSpeed * CalculateDeltaIceHeight(World.Instance.Points[x, y]);
                NewIceHeight[x, y] += FlowSpeed * CalculateDeltaIceHeight(World.Instance.Points[x, y]);
            }
        }

        SetDataArray();
    }

    public void LoadDataArray()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                NewWaterHeight[x, y] = World.Instance.Points[x, y].WaterHeight;
                NewIceHeight[x, y] = World.Instance.Points[x, y].IceHeight;
            }
        }
    }

    public void SetDataArray()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                World.Instance.Points[x, y].WaterHeight = NewWaterHeight[x, y];
                World.Instance.Points[x, y].IceHeight = NewIceHeight[x, y];
            }
        }
    }

    private float CalculateDeltaIceHeight(Point point)
    {
        if (point.Temperature[0] > 0f)
            return 0;

        return point.WaterHeight * (-point.Temperature[0] / 10000000f);
    }
}

