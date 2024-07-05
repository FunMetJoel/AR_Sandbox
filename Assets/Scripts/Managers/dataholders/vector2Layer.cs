using UnityEngine;

public class vector2Layer : layer
{
    public Vector2[] data;

    public Vector2 defaultValue;

    public override void initialiseLayer()
    {
        int arrayLength = world.size.x * world.size.y;
        data = new Vector2[arrayLength];
        for (int i = 0; i < arrayLength; i++)
        {
            data[i] = defaultValue;
        }
    }

    public Vector2 getValue(int x, int y)
    {
        return data[x * world.size.y + y];
    }
}