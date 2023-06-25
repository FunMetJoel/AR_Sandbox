using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceRenderer : MonoBehaviour
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
        // connect texture to material of GameObject t$$anonymous$$s script is attached to
        mRenderer.material.SetTexture("_Height", newTexture(World.Instance.WorldSize.x, World.Instance.WorldSize.y));
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
                texture.SetPixel(x, y, new Color((World.Instance.Points[x, y].IceHeight / World.Instance.WorldSize.z), 0, 0, 0));
            }
        }

        // Apply all SetPixel calls
        texture.Apply();
        return (texture);
    }
}
