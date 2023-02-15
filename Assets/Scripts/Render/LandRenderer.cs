using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandRenderer : MonoBehaviour
{
    // Start is called before the first frame update
     void Start()
     {

     }

    // Update is called once per frame
    void Update()
    {
        // connect texture to material of GameObject t$$anonymous$$s script is attached to
        GetComponent<Renderer>().material.SetTexture("_Height", newTexture(World.instance.WorldSize.x, World.instance.WorldSize.y));
    }

    Texture2D newTexture(int SizeX, int SizeY){
        // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        Texture2D texture = new Texture2D(SizeX, SizeY, TextureFormat.ARGB32, false);
        
        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                texture.SetPixel(x, y, new Color(World.instance.Tiles[x,y].LandHeight/(float)World.instance.WorldSize.z, 0, 0, 0));
            }
        }

        // Apply all SetPixel calls
        texture.Apply();
        return(texture);
    }
}
