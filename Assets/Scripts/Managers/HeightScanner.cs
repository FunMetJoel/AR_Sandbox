using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Windows.Kinect;


public class HeightScanner : MonoBehaviour
{
    [Header("Kinect enz")]
    [SerializeField]
    private bool UseKinect;

    public InfraredSourceManager InfraredSourceManager;
    public DepthSourceManager mDepthSourceManager;
    public Vector2Int MinAndMaxKinectValues = new Vector2Int(0, 255);
    public Vector2 KinectZoom = new Vector2(1,1);
    public Vector2Int KinectPan = new Vector2Int(0, 0);
    public Vector2 SchaleAndPan;
    public float correction = 1f; 

    public Texture2D levelMap;

    public bool AntiFlikker;

    public bool SaveAsFile;


    [Header("Noise")]
    [SerializeField]
    [ConditionalHide("UseKinect", false, true)]
    private float Tseed;

    // Update is called once per frame
    void Update()
    {
        if (SaveAsFile)
        {
            SaveAsFile = false;
            Texture2D HightMap = new Texture2D(World.Instance.WorldSize.x, World.Instance.WorldSize.y);

            for (int y = 0; y < World.Instance.WorldSize.y; y++)
            {
                for (int x = 0; x < World.Instance.WorldSize.x; x++)
                {
                    float value = 1f - (World.Instance.Points[x, y].LandHeight/1.7f);
                    Color color = new Color(value, value, value);
                    HightMap.SetPixel(x, y, color);
                }
            }
            HightMap.Apply();

            byte[] bytes = HightMap.EncodeToPNG();

            File.WriteAllBytes(Application.dataPath + "/Screenshot.png" , bytes);

            Debug.Log(Application.dataPath);

        }
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

        Vector2Int Texture2DSize = new Vector2Int(
                mDepthSourceManager._Texture.width,
                mDepthSourceManager._Texture.height
            );

        Vector2 pixelsPerPixel = new Vector2((Texture2DSize.x) / World.Instance.WorldSize.x, (Texture2DSize.y) / World.Instance.WorldSize.y);


        for (int y = 0; y < World.Instance.WorldSize.y; y++)
        {
            for (int x = 0; x < World.Instance.WorldSize.x; x++)
            {

                /* newHeight[x, y] = Mathf.Clamp(
                        (InfraredSourceManager._Texture.GetPixel(x, y).r - MinAndMaxKinectValues.x) / (MinAndMaxKinectValues.y - MinAndMaxKinectValues.x),
                        0f, 1f
                    );*/

                if (AntiFlikker) {
                    newHeight[x, y] = Mathf.Clamp(
                        World.Instance.Points[x, y].LandHeight - ((World.Instance.Points[x, y].LandHeight - ( (mDepthSourceManager._Texture.GetPixel(Mathf.RoundToInt(x * KinectZoom.x + KinectPan.x), Mathf.RoundToInt(y * KinectZoom.y + KinectPan.y)).r * SchaleAndPan.x + SchaleAndPan.y) +
                        correction*(levelMap.GetPixel(Mathf.RoundToInt(x), Mathf.RoundToInt(y)).r * 2f - 1f) ))/20f),
                        0f, 1f
                    );
                }
                else
                { 
                newHeight[x, y] = Mathf.Clamp(
                        (mDepthSourceManager._Texture.GetPixel(Mathf.RoundToInt(x * KinectZoom.x + KinectPan.x), Mathf.RoundToInt(y * KinectZoom.y + KinectPan.y)).r * SchaleAndPan.x + SchaleAndPan.y)+
                        correction*(levelMap.GetPixel(Mathf.RoundToInt(x ), Mathf.RoundToInt(y )).r * 2f -1f ),
                        0f, 1f
                    );
                }
                /*
                newHeight[x, y] = 
                    Mathf.Clamp( 
                        ( 
                        (float)KinectData[
                            (
                            Mathf.RoundToInt(x * pixelsPerPixel.x * KinectZoom.x + KinectPan.x) + 
                            Mathf.RoundToInt(y * InfraredSourceManager.Size.x * pixelsPerPixel.y * KinectZoom.y) //
                            )
                            ] 
                         - MinAndMaxKinectValues.x) / (MinAndMaxKinectValues.y - MinAndMaxKinectValues.x) , 0f, 1f);

                */
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
