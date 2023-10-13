using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureRenderer : MonoBehaviour
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
        if (mRenderer.material.GetTexture("_Temp") != null)
        {
            Destroy(mRenderer.material.GetTexture("_Temp"));
        }
        // connect texture to material of GameObject t$$anonymous$$s script is attached to
        mRenderer.material.SetTexture("_Temp", newTexture(World.Instance.WorldSize.x, World.Instance.WorldSize.y));
    }

    Texture2D newTexture(int SizeX, int SizeY)
    {
        // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        Texture2D texture = new Texture2D(SizeX, SizeY, TextureFormat.ARGB32, false);
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                texture.SetPixel(x, y, new Color((World.Instance.Points[x, y].Temperature[0] + 60f) / 120f, (World.Instance.Points[x, y].Temperature[1] + 60f) / 120f, (Mathf.Max(0, World.Instance.SunLine.z*255f + (1f - World.Instance.SunLine.z)*(255f - 1f*World.Instance.distanceToSunLine(new Vector2(x, y)))) / 255f), 0));
                //Debug.Log((World.Instance.Points[x, y].Temperature[0] + 60f) / 120f);
            }
        }

        // Apply all SetPixel calls
        texture.Apply();
        return (texture);
    }
}
