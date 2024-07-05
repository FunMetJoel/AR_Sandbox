using UnityEngine;

public class floatLayer : layer
{
    public float[] data;

    public float defaultValue;

    public override void initialiseLayer()
    {
        int arrayLength = world.size.x * world.size.y;
        data = new float[arrayLength];
        for (int i = 0; i < arrayLength; i++)
        {
            data[i] = defaultValue;
        }
    }

    public float getValue(int x, int y)
    {
        return data[x * world.size.y + y];
    }
}