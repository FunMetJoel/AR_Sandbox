using UnityEngine;

public class floatLayerRenderer : MonoBehaviour
{
    private floatLayer layer;

    public void Awake()
    {
        layer = GetComponent<floatLayer>();
    }

    public void Update()
    {
        Texture2D texture = new Texture2D(layer.world.size.x, layer.world.size.y);
        texture.wrapMode = TextureWrapMode.Clamp;
        for (int x = 0; x < layer.world.size.x; x++)
        {
            for (int y = 0; y < layer.world.size.y; y++)
            {
                float value = layer.getValue(x, y);
                texture.SetPixel(x, y, new Color(value, value, value));
            }
        }
        texture.Apply();
        GetComponent<Renderer>().material.SetTexture("_BaseMap", texture);
    }
}