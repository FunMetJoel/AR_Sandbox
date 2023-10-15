using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "World", menuName = "Scriptable Objects/Globals/World")]
public class World : SingletonScriptableObject<World>
{
    //adsf
    public Vector3Int WorldSize = new Vector3Int(40, 30, 1);
    public Point[,] Points;
    [Tooltip("Hoe groot de wereld is in km. x is onder en y is boven")]
    public Vector2 MinMaxScaleKM = new Vector2(-10, 10);

    [Tooltip("Van 0 - 24 en geeft tijd aan in uren")]
    [Range(0.0f, 24.0f)]
    public float DayTime;

    [Tooltip("Op welke plekken er een water bron is en hoeveel water ze afgeven")]
    public Vector3Int[] WaterSources;

    [Tooltip("De lijn van de zon")]
    public Vector3 SunLine;

    public float distanceToSunLine(Vector2 location)
    {
        float dist = Mathf.Abs(SunLine.x * location.x - location.y + SunLine.y)/Mathf.Sqrt(SunLine.x * SunLine.x + 1f);
        return dist;
    }

    public void GenerateWorld()
    {
        Points = new Point[WorldSize.x, WorldSize.y];

        for (int y = 0; y < WorldSize.y; y++)
        {
            for (int x = 0; x < WorldSize.x; x++)
            {
                Points[x, y] = new Point();
                Points[x, y].x = x;
                Points[x, y].y = y;
                Points[x, y].LandHeight = (float)WorldSize.z * Mathf.PerlinNoise(0.01f * x, 0.01f * y);
                Points[x, y].WaterHeight = 0.05f;
                Points[x, y].Temperature[0] = 0;
                Points[x, y].Temperature[1] = 0;
            }
        }
    }

    public bool InBounds(int x, int y)
    {        
        if(x < 0) return false;
        if(y < 0) return false;
        if(x >= WorldSize.x) return false;
        if(y >= WorldSize.y) return false;

        return true;
    }

    class Data { 

    }
    
}

[System.Serializable]
public class Point
{
    public int x;
    public int y;

    public float LandHeight = 0;
    public float WaterHeight = 0;
    public float IceHeight = 0;
    public float GroundHumidity = 0;
    public float PlantDensity = 0;

    // in de lucht
    public float[] Temperature = new float[2];
    public float[] AirHumidity = new float[2];
    public Vector2[] Wind = new Vector2[2];


    public Vector4 BiomeData = new Vector4(0, 0, 0, 0);
    
    public float AbsoluteWaterHeight()
    {
        return LandHeight + WaterHeight + 0.8f * IceHeight;
    }

    public float TotalHeight()
    {
        return LandHeight + WaterHeight + IceHeight;
    }
}
