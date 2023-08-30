using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering;

public class TemperatureManager : MonoBehaviour
{
    public bool doTempMovement;

    public float FlowSpeed = 10f;
    private float RunTime;

    private float[,,] NewTemp;

    public float DiffusionAmount;

    // Start is called before the first frame update
    void Start()
    {
        NewTemp = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y, 2];
    }

    // Update is called once per frame
    void Update()
    {
        if (doTempMovement)
        {
            RunTime += Time.deltaTime * FlowSpeed;

            if (RunTime >= 10)
            {
                RunTime = 0;

                for (int y = 0; y < World.Instance.WorldSize.y; y++)
                {
                    for (int x = 0; x < World.Instance.WorldSize.x; x++)
                    {
                        NewTemp[x, y, 0] = World.Instance.Points[x, y].Temperature[0];
                        NewTemp[x, y, 1] = World.Instance.Points[x, y].Temperature[1];
                    }
                }

                UpdateTemp(Diffuse(NewTemp));
            }
        }
    }

    private float[,,] Diffuse(float[,,] temp)
    {
        float[,,] newTempArr = temp;

        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                for(int i = 0; i < 2; i++)
                {
                    float newTemp = temp[x, y, i];
                    if (World.Instance.InBounds(x - 1, y)) newTemp += DiffusionAmount * temp[x - 1, y, i];
                    if (World.Instance.InBounds(x + 1, y)) newTemp += DiffusionAmount * temp[x + 1, y, i];
                    if (World.Instance.InBounds(x, y - 1)) newTemp += DiffusionAmount * temp[x, y - 1, i];
                    if (World.Instance.InBounds(x, y + 1)) newTemp += DiffusionAmount * temp[x, y + 1, i];

                    newTempArr[x, y, i] = newTemp / (1 + (4 * DiffusionAmount));
                }
            }
        }

        return newTempArr;
    }

    private void UpdateTemp(float[,,] newTemp)
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                World.Instance.Points[x, y].Temperature[0] = newTemp[x, y, 0];
            }
        }
    }
}
