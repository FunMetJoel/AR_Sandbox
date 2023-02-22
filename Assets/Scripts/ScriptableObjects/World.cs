using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "World", menuName = "Scriptable Objects/Globals/World")]
public class World : SingletonScriptableObject<World>
{
    public Vector3Int WorldSize = new Vector3Int(40, 30, 255);
    public Tile[,] Tiles;
    [Tooltip("Hoe groot de wereld is in km. x is onder en y is boven")]
    public Vector2 MinMaxScaleKM = new Vector2(-10, 10);

    [Tooltip("Van 0 - 24 en geeft tijd aan in uren")]
    [Range(0.0f, 24.0f)]
    public float DayTime;

    [Tooltip("Op welke plekken er een water bron is en hoeveel water ze afgeven")]
    public Vector3Int[] WaterSources;

    public void GenerateWorld()
    {
        Tiles = new Tile[WorldSize.x, WorldSize.y];

        for (int y = 0; y < WorldSize.y; y++)
        {
            for (int x = 0; x < WorldSize.x; x++)
            {
                Tiles[x, y] = new Tile();
                Tiles[x, y].LandHeight = Mathf.RoundToInt((float)WorldSize.z * Mathf.PerlinNoise(0.01f * x, 0.01f * y));
                Tiles[x, y].WaterHeight = 50;
            }
        }
    }
}

[System.Serializable]
public class Tile
{
    public int LandHeight = 0;
    public int WaterHeight = 0;
    public int IceHeight = 0;
    public int Humidity = 0;
    public int TreeDensity = 0;
    public Vector4 BiomeData = new Vector4(0, 0, 0, 0);

    [Tooltip("LuchtTemperatuur boven land- of water-oppervlakte in graden Celcius")]
    public float LuchtTemperatuur = 0;

    public int LuchtVochtigheid = 0;
    
    public int AbsoluteWaterHeight()
    {
        return LandHeight + WaterHeight;
    }

    public int TotalHeight()
    {
        return LandHeight + WaterHeight;
    }
}
