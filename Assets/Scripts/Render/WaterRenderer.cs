using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterRenderer : MonoBehaviour
{
    Renderer mRenderer = null;

    // Start is called before the first frame update
    void Start()
    {
        mRenderer = GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (mRenderer.material.GetTexture("_Height") != null)
        {
            Destroy(mRenderer.material.GetTexture("_Height"));
        }
        // connect texture to material of GameObject t$$anonymous$$s script is attached to
        mRenderer.material.SetTexture("_Height", newTexture(World.Instance.WorldSize.x, World.Instance.WorldSize.y));

        if (mRenderer.material.GetVector("_MinMaxSchale") != new Vector4(World.Instance.MinMaxScaleKM.x, World.Instance.MinMaxScaleKM.y, 0, 0))
        {
            mRenderer.material.SetVector("_MinMaxSchale", World.Instance.MinMaxScaleKM);
        }
    }

    Texture2D newTexture(int SizeX, int SizeY){
        // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        Texture2D texture = new Texture2D(SizeX, SizeY, TextureFormat.ARGB32, false);
        
        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                if(World.Instance.Points[x,y].WaterHeight > 0){
                    texture.SetPixel(x, y, new Color(World.Instance.Points[x, y].WaterHeight, World.Instance.Points[x, y].IceHeight, 0, 1));
                }else{
                    texture.SetPixel(x, y, new Color(World.Instance.Points[x,y].WaterHeight, World.Instance.Points[x, y].IceHeight, 0, 0));
                }
                //texture.SetPixel(x, y, new Color(World.Instance.Points[x,y].WaterHeight/(float)World.Instance.WorldSize.z, 0, 0, 0));
            }
        }

        // Apply all SetPixel calls
        texture.filterMode = FilterMode.Trilinear;
        texture.Apply();
        return (texture);
    }
}
