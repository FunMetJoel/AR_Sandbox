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

    private float[,] NewTemp;

    public float DiffusionAmount;

    // Start is called before the first frame update
    void Start()
    {
        NewTemp = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y];
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
                        NewTemp[x, y] = World.Instance.Tiles[x, y].Temperature;
                    }
                }

                UpdateTemp(Diffuse(NewTemp));
            }
        }
    }

    private float[,] Diffuse(float[,] temp)
    {
        float[,] newTempArr = temp;

        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                float newTemp = temp[x, y];
                if(World.Instance.InBounds(x-1,y)) newTemp += DiffusionAmount * temp[x-1, y];
                if(World.Instance.InBounds(x+1,y)) newTemp += DiffusionAmount * temp[x+1, y];
                if (World.Instance.InBounds(x, y-1)) newTemp += DiffusionAmount * temp[x, y-1];
                if (World.Instance.InBounds(x, y+1)) newTemp += DiffusionAmount * temp[x, y+1];

                newTempArr[x, y] = newTemp / (1 + (4 * DiffusionAmount));
            }
        }

        return newTempArr;
    }

    private void UpdateTemp(float[,] newTemp)
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                World.Instance.Tiles[x, y].Temperature = newTemp[x, y];
            }
        }
    }
}
