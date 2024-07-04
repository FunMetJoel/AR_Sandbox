using UnityEngine;

public class vector2Layer : layer
{
    public Vector2[] LayerData;

    public Vector2 defaultValue;

    public override void initialiseLayer()
    {
        int arrayLength = world.size.x * world.size.y;
        LayerData = new Vector2[arrayLength];
        for (int i = 0; i < arrayLength; i++)
        {
            LayerData[i] = defaultValue;
        }
    }

    public Vector2 getValue(int x, int y)
    {
        return LayerData[x * world.size.y + y];
    }
}