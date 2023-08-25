using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHumiditySpread : MonoBehaviour
{
    private float[,] NewGroundHumidity;

    [SerializeField]
    private float FlowSpeed = 1f;
    private int OPS;
    private float RunTime;

    private bool doGroundHumidityMovement;

    public void UpdateSettings()
    {
        doGroundHumidityMovement = Settings.Instance.doGroundHumidityMovement;
        OPS = Settings.Instance.OPS;
    }

    void Start()
    {
        // Settings stuf
        UpdateSettings();
        Settings.Instance.SettingsChanged.AddListener(UpdateSettings);

        // Populate array
        NewGroundHumidity = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y];
    }

    void Update()
    {
        // Check if water should move
        if (!doGroundHumidityMovement)
            return;

        // Moves water each 1sec / OperationsPerSeconds
        RunTime += Time.deltaTime;
        if (RunTime <= 1f / OPS)
            return;

        RunTime = 0;



        LoadGroundHumidityArray();

        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                CalculateGroundFluidPoint(x, y);
            }
        }

        SetGroundHumidityArray();
    }



    public void LoadGroundHumidityArray()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                NewGroundHumidity[x, y] = World.Instance.Points[x, y].GroundHumidity;
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

    private void CalculateGroundFluidPoint(int x, int y)
    {
        Point point = World.Instance.Points[x, y];
        if (World.Instance.InBounds(x - 1, y) && World.Instance.Points[x - 1, y].GroundHumidity < point.GroundHumidity)
        {
            NewGroundHumidity[x - 1, y] += FlowSpeed * point.GroundHumidity / (8f);
            NewGroundHumidity[x, y] -= FlowSpeed * point.GroundHumidity / (8f);
        }
        if (World.Instance.InBounds(x + 1, y) && World.Instance.Points[x + 1, y].GroundHumidity < point.GroundHumidity)
        {
            NewGroundHumidity[x + 1, y] += FlowSpeed * point.GroundHumidity / (8f);
            NewGroundHumidity[x, y] -= FlowSpeed * point.GroundHumidity / (8f);
        }
        if (World.Instance.InBounds(x, y - 1) && World.Instance.Points[x, y - 1].GroundHumidity < point.GroundHumidity)
        {
            NewGroundHumidity[x, y - 1] += FlowSpeed * point.GroundHumidity / (8f);
            NewGroundHumidity[x, y] -= FlowSpeed * point.GroundHumidity / (8f);
        }
        if (World.Instance.InBounds(x, y + 1) && World.Instance.Points[x, y + 1].GroundHumidity < point.GroundHumidity)
        {
            NewGroundHumidity[x, y + 1] += FlowSpeed * point.GroundHumidity / (8f);
            NewGroundHumidity[x, y] -= FlowSpeed * point.GroundHumidity / (8f);
        }
    }

}

