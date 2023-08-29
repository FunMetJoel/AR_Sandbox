using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAndSpace : MonoBehaviour
{
    private float[,] NewTemp;

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
        NewTemp = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y];
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
                NewTemp[x, y] -= FlowSpeed * CalculateDeltaTemp(World.Instance.Points[x, y]) * 0.005f;
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
        float delta = radiationAmount;
        delta = delta - (radiationAmount * point.AirHumidity * 0.5f);
        return delta;
    }
}
