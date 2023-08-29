using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class WolkMovement : MonoBehaviour
{
    private float[,] NewAirHumidity;

    [SerializeField]
    private float FlowSpeed = 1f;
    private int OPS;
    private float RunTime;

    private bool doWind;

    public void UpdateSettings()
    {
        doWind = Settings.Instance.doWind;
        OPS = Settings.Instance.OPS;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Settings stuf
        UpdateSettings();
        Settings.Instance.SettingsChanged.AddListener(UpdateSettings);

        // Populate array
        NewAirHumidity = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y];
    }

    // Update is called once per frame
    void Update()
    {
        // Check if water should move
        if (!doWind)
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
                CalculateMovement(x, y);
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
                NewAirHumidity[x, y] = World.Instance.Points[x, y].AirHumidity;
            }
        }
    }

    public void SetDataArray()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                World.Instance.Points[x, y].AirHumidity = NewAirHumidity[x, y];
            }
        }
    }

    private void CalculateMovement(int x, int y)
    {
        float windX = World.Instance.Points[x, y].Wind.x;
        float windY = World.Instance.Points[x, y].Wind.y;

        if (windX > 0)
        {
            //NewAirHumidity[x + 1, y] += 
        }
    }
}