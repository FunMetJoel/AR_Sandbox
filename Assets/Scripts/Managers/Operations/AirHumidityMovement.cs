using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirHumidityMovement : MonoBehaviour
{
    private float[,,] NewAirHumidity;

    [SerializeField]
    private float FlowSpeed = 1f;
    private int OPS;
    private float RunTime;

    private bool doWindMoveAir;

    public void UpdateSettings()
    {
        doWindMoveAir = Settings.Instance.doWindMoveAir;
        OPS = Settings.Instance.OPS;
    }

    void Start()
    {
        // Settings stuf
        UpdateSettings();
        Settings.Instance.SettingsChanged.AddListener(UpdateSettings);

        // Populate array
        NewAirHumidity = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y, 2];
    }

    void Update()
    {
        // Check if water should move
        if (!doWindMoveAir)
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
                CalculateAirHumidity(x, y);
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
                NewAirHumidity[x, y, 0] = World.Instance.Points[x, y].AirHumidity[0];
                NewAirHumidity[x, y, 1] = World.Instance.Points[x, y].AirHumidity[1];
            }
        }
    }

    public void SetDataArray()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                World.Instance.Points[x, y].AirHumidity[0] = NewAirHumidity[x, y, 0];
                World.Instance.Points[x, y].AirHumidity[1] = NewAirHumidity[x, y, 1];
            }
        }
    }

    private void CalculateAirHumidity(int x, int y)
    {
        Point point = World.Instance.Points[x, y];
        for (int z = 0; z < 2; z++)
        {
            if (World.Instance.InBounds(x - 1, y) && point.Wind[z].x < 0)
            {
                NewAirHumidity[x - 1, y, z] += FlowSpeed * point.AirHumidity[z] / 8f;
                NewAirHumidity[x, y, z] -= FlowSpeed * point.AirHumidity[z] / 8f;
            }
            if (World.Instance.InBounds(x + 1, y) && point.Wind[z].x > 0)
            {
                NewAirHumidity[x + 1, y, z] += FlowSpeed * point.AirHumidity[z] / 8f;
                NewAirHumidity[x, y, z] -= FlowSpeed * point.AirHumidity[z] / 8f;
            }
            if (World.Instance.InBounds(x, y - 1) && point.Wind[z].y < 0)
            {
                NewAirHumidity[x, y - 1, z] += FlowSpeed * point.AirHumidity[z] / 8f;
                NewAirHumidity[x, y, z] -= FlowSpeed * point.AirHumidity[z] / 8f;
            }
            if (World.Instance.InBounds(x, y + 1) && point.Wind[z].y > 0)
            {
                NewAirHumidity[x, y + 1, z] += FlowSpeed * point.AirHumidity[z] / 8f;
                NewAirHumidity[x, y, z] -= FlowSpeed * point.AirHumidity[z] / 8f;
            }
        }
    }

}
