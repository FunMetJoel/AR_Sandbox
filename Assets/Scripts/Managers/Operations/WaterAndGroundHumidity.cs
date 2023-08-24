using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAndGroundHumidity : MonoBehaviour
{
    private float[,] NewGroundHumidity;
    private float[,] NewWaterHeight;

    [SerializeField]
    private float FlowSpeed = 1f;

    [SerializeField]
    private int OPS;
    private float RunTime;

    private bool doGroundHumidity;

    public void UpdateSettings()
    {
        doGroundHumidity = Settings.Instance.doGroundHumidity;
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
        NewGroundHumidity = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y];
    }

    // Update is called once per frame
    void Update()
    {
        // Check if water should move
        if (!doGroundHumidity)
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
                NewWaterHeight[x, y] -= FlowSpeed*CalculateDeltaGndHumidity(World.Instance.Points[x, y]);
                NewGroundHumidity[x, y] += FlowSpeed*CalculateDeltaGndHumidity(World.Instance.Points[x, y]);
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
                NewGroundHumidity[x, y] = World.Instance.Points[x, y].GroundHumidity;
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
                World.Instance.Points[x, y].GroundHumidity = NewGroundHumidity[x, y];
            }
        }
    }

    private float CalculateDeltaGndHumidity(Point point)
    {
        if (point.GroundHumidity > 0.03f)
            return 0;

        if (point.GroundHumidity > 0)
            return point.WaterHeight * (1f / (point.GroundHumidity + 0.1f)) / 1000f - 0.000001f;

        return point.WaterHeight * (1f / (point.GroundHumidity + 0.1f)) / 1000f;
    }
}
