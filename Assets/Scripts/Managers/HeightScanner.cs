using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightScanner : MonoBehaviour
{
    [SerializeField]
    private bool UseKinect;

    [SerializeField]
    [ConditionalHide("UseKinect", false, true)]
    private float Tseed;

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
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                World.Instance.Tiles[x, y].LandHeight = newHeight[x,y];
            }
        }
    }

    private int[,] PerlinNoiseHeight(float seed)
    {
        int[,] newHeight = new int[World.Instance.WorldSize.x,World.Instance.WorldSize.y];

        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                newHeight[x, y] = Mathf.RoundToInt((float)World.Instance.WorldSize.z * Mathf.PerlinNoise(0.05f * x + seed, 0.05f * y + seed));
            }
        }

        return newHeight;
    } 


}
