using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAndWind : MonoBehaviour
{
    private Vector2[,,] NewWind;

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
        NewWind = new Vector2[World.Instance.WorldSize.x, World.Instance.WorldSize.y, 2];
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
                NewWind[x, y, 1] = CalculateWind(x, y) * 0.1f;
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
                NewWind[x, y, 0] = World.Instance.Points[x, y].Wind[0];
                NewWind[x, y, 1] = World.Instance.Points[x, y].Wind[1];
            }
        }
    }

    public void SetDataArray()
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                World.Instance.Points[x, y].Wind[0] = NewWind[x, y, 0];
                World.Instance.Points[x, y].Wind[1] = NewWind[x, y, 1];
            }
        }
    }

    private Vector2 CalculateWind(int x, int y)
    {
        float dx = 0;
        float dy = 0;


        //TODO: NOG FIXEN

        // Calculate x
        if (World.Instance.InBounds(x + 1, y))
        {
            dx = (World.Instance.Points[x, y].Temperature[1] - World.Instance.Points[x + 1, y].Temperature[1]);
        }
        if (World.Instance.InBounds(x - 1, y))
        {
            dx -= (World.Instance.Points[x, y].Temperature[1] - World.Instance.Points[x - 1, y].Temperature[1]);
        }


        if (World.Instance.InBounds(x, y + 1))
        {
            dy = (World.Instance.Points[x, y].Temperature[1] - World.Instance.Points[x, y + 1].Temperature[1]);
        }
        if (World.Instance.InBounds(x, y - 1))
        {
            dy -= (World.Instance.Points[x, y].Temperature[1] - World.Instance.Points[x, y - 1].Temperature[1]);
        }


        return new Vector2(dx, dy);
    }
}
