using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAndSun : MonoBehaviour
{
    private float[,] NewTemp;

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
        NewTemp = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y];
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
                NewTemp[x, y] += FlowSpeed * CalculateDeltaTemp(World.Instance.Points[x, y]) * 0.0001f;
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
                NewTemp[x, y] = World.Instance.Points[x, y].Temperature;
            }
        }
    }

    public void SetDataArray()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                World.Instance.Points[x, y].Temperature = NewTemp[x, y];
            }
        }
    }

    private float CalculateDeltaTemp(Point point)
    {
        float delta = SunTemp;
        delta = delta - (SunTemp * point.AirHumidity * 0.9f);
        delta = delta - (SunTemp * point.IceHeight * 0.9f);

        return delta;
    }
}