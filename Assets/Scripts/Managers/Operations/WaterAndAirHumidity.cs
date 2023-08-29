using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAndAirHumidity : MonoBehaviour
{
    private float[,] NewAirHumidity;
    private float[,] NewWaterHeight;

    [SerializeField]
    private float FlowSpeed = 1f;
    private int OPS;
    private float RunTime;

    private bool doAirHumidity;

    public void UpdateSettings()
    {
        doAirHumidity = Settings.Instance.doAirHumidity;
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
        NewAirHumidity = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y];
    }

    // Update is called once per frame
    void Update()
    {
        // Check if water should move
        if (!doAirHumidity)
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
                NewWaterHeight[x, y] -= FlowSpeed * CalculateDeltaAirHumidity(World.Instance.Points[x, y]);
                NewAirHumidity[x, y] += FlowSpeed * CalculateDeltaAirHumidity(World.Instance.Points[x, y]);
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
                World.Instance.Points[x, y].WaterHeight = NewWaterHeight[x, y];
                World.Instance.Points[x, y].AirHumidity = NewAirHumidity[x, y];
            }
        }
    }

    private float CalculateDeltaAirHumidity(Point point)
    {
        return point.WaterHeight * (1f / (point.AirHumidity + 0.1f)) / 1000f;
    }
}
