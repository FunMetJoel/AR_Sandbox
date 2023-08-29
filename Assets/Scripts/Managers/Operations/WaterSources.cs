using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSources : MonoBehaviour
{

    [SerializeField]
    private float FlowSpeed = 1f;
    private int OPS;
    private float RunTime;

    private bool doWaterSources;

    public void UpdateSettings()
    {
        doWaterSources = Settings.Instance.doWaterSources;
        OPS = Settings.Instance.OPS;
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
        if (!doWaterSources)
            return;

        // Moves water each 1sec / OperationsPerSeconds
        RunTime += Time.deltaTime;
        if (RunTime <= 1f / OPS)
            return;

        RunTime = 0;

        newFrame();
    }

    void OnDrawGizmos()
    {
        if (!doWaterSources)
            return;

        foreach (Vector3Int Source in World.Instance.WaterSources)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(new Vector3(((-Source.x*1f) / World.Instance.WorldSize.x) * 40 + 20, 0, ((-Source.y * 1f) / World.Instance.WorldSize.y) * 30 + 15), 0.1f);
        }
    }

    void newFrame()
    {
        foreach (Vector3Int Source in World.Instance.WaterSources)
        {
            World.Instance.Points[Source.x, Source.y].WaterHeight += (Source.z * FlowSpeed) / 10000f;
        }
    }
}
