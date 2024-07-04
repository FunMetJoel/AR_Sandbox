using UnityEngine;

public class floatLayer : layer
{
    public float[] LayerData;

    public float defaultValue;

    public override void initialiseLayer()
    {
        int arrayLength = world.size.x * world.size.y;
        LayerData = new float[arrayLength];
        for (int i = 0; i < arrayLength; i++)
        {
            LayerData[i] = defaultValue;
        }
    }

    public float getValue(int x, int y)
    {
        return LayerData[x * world.size.y + y];
    }
}