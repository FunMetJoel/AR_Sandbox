using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightScanner : MonoBehaviour
{
    [SerializeField]
    private bool UseKinect;

    public float Tseed;

    // Update is called once per frame
    void Update()
    {
        if(UseKinect)
        {

        }else
        {
            ChangeWorldHeight(PerlinNoiseHeight(Tseed));
        }
    }
    
    private void ChangeWorldHeight(int[,] newHeight)
    {
        for (int y = 0; y < World.instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.instance.WorldSize.x; x++)
            {
                World.instance.Tiles[x, y].LandHeight = newHeight[x,y];
            }
        }
    }

    private int[,] PerlinNoiseHeight(float seed)
    {
        int[,] newHeight = new int[World.instance.WorldSize.x,World.instance.WorldSize.y];

        for (int y = 0; y < World.instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.instance.WorldSize.x; x++)
            {
                newHeight[x, y] = Mathf.RoundToInt((float)World.instance.WorldSize.z * Mathf.PerlinNoise(0.01f * x + seed, 0.01f * y + seed));
            }
        }

        return newHeight;
    } 


}
