using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using Windows.Kinect;


public class HeightScanner : MonoBehaviour
{
    [Header("Kinect enz")]
    [SerializeField]
    private bool UseKinect;

    public InfraredSourceManager InfraredSourceManager;
    public Vector2Int MinAndMaxKinectValues = new Vector2Int(0, 255);

    [SerializeField]
    [ConditionalHide("UseKinect", false, true)]
    private float Tseed;

    // Update is called once per frame
    void Update()
    {
        if(UseKinect)
        {
            ushort[] Data = InfraredSourceManager.GetIntencityData();

            ChangeWorldHeight(KinectHeight(Data));
        }
        else
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

    private int[,] KinectHeight(ushort[] KinectData)
    {
        int[,] newHeight = new int[World.Instance.WorldSize.x, World.Instance.WorldSize.y];

        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                newHeight[x, y] = Mathf.Clamp(Mathf.RoundToInt( (((float)KinectData[(x * 4 + y * InfraredSourceManager.Size.x * 4)] - MinAndMaxKinectValues.x) / (MinAndMaxKinectValues.y - MinAndMaxKinectValues.x)) * World.Instance.WorldSize.z), 0, World.Instance.WorldSize.z);
            }
        }

        Debug.Log(newHeight[64,53]);
        return newHeight;
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
