using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAndSun : MonoBehaviour
{
    private float[,,] NewTemp;

    [SerializeField]
    private float FlowSpeed = 1f;
    private int OPS;
    private float RunTime;

    public float SunTemp = 100f;

    private bool doSunTemp;

    public void UpdateSettings()
    {
        doSunTemp = Settings.Instance.doSunTemp;
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
        if (!doSunTemp)
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
                NewTemp[x, y, 0] += FlowSpeed * CalculateDeltaTemp(World.Instance.Points[x, y])[0] * 0.005f;
                NewTemp[x, y, 1] += FlowSpeed * CalculateDeltaTemp(World.Instance.Points[x, y])[1] * 0.005f;
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
        float opneem = 0.5f;
        float T = SunTemp * (Mathf.Max(0, World.Instance.SunLine.z * 255f + (1f - World.Instance.SunLine.z) * (255f - 1f * World.Instance.distanceToSunLine(new Vector2(point.x, point.y)))) / 255f);
        delta[1] = T * opneem;
        T = T - Mathf.Min(T * point.AirHumidity[1] * 300f, T);
        T = T - Mathf.Min(T * point.WaterHeight * 4f, T);
        delta[0] = T * opneem;
        return delta;
    }
}