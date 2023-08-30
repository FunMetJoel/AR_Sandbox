using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class WolkMovement : MonoBehaviour
{
    private float[,,] NewAirHumidity;

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
        NewAirHumidity = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y, 2];
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

    private void CalculateMovement(int x, int y)
    {
        Vector2[] wind = World.Instance.Points[x, y].Wind;
    }
}