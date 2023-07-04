using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{

    [SerializeField]
    private float FrameFrequency = 10f;
    private float RunTime;

    public void UpdateSettings()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        // Settings stuf
        UpdateSettings();
        Settings.Instance.SettingsChanged.AddListener(UpdateSettings);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if water should move
        if (!true)
            return;

        // Moves water each sec/flowspeed
        RunTime += Time.deltaTime * FrameFrequency;
        if (RunTime >= 1)
        {
            RunTime = 0;

            NewFrame();
        }
    }

    public void NewFrame()
    {
        // Gets water start values

        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                World.Instance.Points[x, y].GroundHumidity -= CalculatePlantGroth(World.Instance.Points[x, y]);
                World.Instance.Points[x, y].PlantDensity += CalculatePlantGroth(World.Instance.Points[x, y]);
            }
        }
    }

    private float CalculatePlantGroth(Point point)
    {
        if (point.GroundHumidity == 0)
            return 0;

        if (point.Temperature <= 0)
            return 0;

        return point.GroundHumidity / 10f;
    }
}
