using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAndSpace : MonoBehaviour
{
    private float[,,] NewTemp;

    [SerializeField]
    private float FlowSpeed = 1f;
    private int OPS;
    private float RunTime;

    private bool doRadiateToSky;

    [SerializeField]
    private float radiationAmount = 100f;

    public void UpdateSettings()
    {
        doRadiateToSky = Settings.Instance.doRadiateToSky;
        OPS = Settings.Instance.OPS;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Settings stuf
        UpdateSettings();
        Settings.Instance.SettingsChanged.AddListener(UpdateSettings);

        // Populate array
        NewTemp = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y,2];
    }

    // Update is called once per frame
    void Update()
    {
        // Check if water should move
        if (!doRadiateToSky)
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
                NewTemp[x, y, 0] -= FlowSpeed * CalculateDeltaTemp(World.Instance.Points[x, y])[0] * 0.005f;
                NewTemp[x, y, 1] += FlowSpeed * CalculateDeltaTemp(World.Instance.Points[x, y])[0] * 0.005f;
                NewTemp[x, y, 1] -= FlowSpeed * CalculateDeltaTemp(World.Instance.Points[x, y])[1] * 0.005f;
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
                NewTemp[x, y, 0] = World.Instance.Points[x, y].Temperature[0];
                NewTemp[x, y, 1] = World.Instance.Points[x, y].Temperature[1];
            }
        }
    }

    public void SetDataArray()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                World.Instance.Points[x, y].Temperature[0] = NewTemp[x, y, 0];
                World.Instance.Points[x, y].Temperature[1] = NewTemp[x, y, 1];
            }
        }
    }

    private float[] CalculateDeltaTemp(Point point)
    {
        float[] delta = new float[2];
        delta[0] = point.Temperature[0];
        delta[0] = delta[0] - (delta[0] * point.AirHumidity[1] * 0.5f);
        delta[1] = point.Temperature[1];
        return delta;
    }
}
