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

    [Header("Noise")]
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
                World.Instance.Points[x, y].LandHeight = newHeight[x,y];
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

        return newHeight;
    }

    private int[,] PerlinNoiseHeight(float seed)
    {
        int[,] newHeight = new int[World.Instance.WorldSize.x,World.Instance.WorldSize.y];

        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                int nHeight = Mathf.RoundToInt((float)World.Instance.WorldSize.z * Mathf.PerlinNoise(0.03f * x + seed, 0.03f * y + seed));
                nHeight += Mathf.RoundToInt((float)World.Instance.WorldSize.z * 0.1f * Mathf.PerlinNoise(0.1f * x + seed, 0.2f * y + seed));
                nHeight = Mathf.RoundToInt((float)nHeight * ((Mathf.PerlinNoise(0.01f * x + seed, 0.01f * y + seed)) + 0.5f));
                newHeight[x, y] = nHeight;

            }
        }

        return newHeight;
    } 


}
