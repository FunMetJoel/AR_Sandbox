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
            byte[] Data = InfraredSourceManager.GetIntencityData2();

            ChangeWorldHeight(KinectHeight(Data));
        }
        else
        {
            ChangeWorldHeight(PerlinNoiseHeight(Tseed));
        }
    }
    
    private void ChangeWorldHeight(float[,] newHeight)
    {
        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                World.Instance.Points[x, y].LandHeight = newHeight[x,y];
            }
        }
    }

    private float[,] KinectHeight(byte[] KinectData)
    {
        float[,] newHeight = new float[World.Instance.WorldSize.x, World.Instance.WorldSize.y];

        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                newHeight[x, y] = Mathf.Clamp( ( (float)KinectData[(x * 2 + y * InfraredSourceManager.Size.x * 2)] - MinAndMaxKinectValues.x) / (MinAndMaxKinectValues.y - MinAndMaxKinectValues.x) , 0f, 1f);
            }
        }
        return newHeight;
    }

    private float[,] PerlinNoiseHeight(float seed)
    {
        float[,] newHeight = new float[World.Instance.WorldSize.x,World.Instance.WorldSize.y];

        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {
                float nHeight = Mathf.PerlinNoise(0.03f * x + seed, 0.03f * y + seed);
                //nHeight *= Mathf.PerlinNoise(0.1f * x + seed, 0.2f * y + seed);
                newHeight[x, y] = nHeight;

            }
        }

        return newHeight;
    } 


}
