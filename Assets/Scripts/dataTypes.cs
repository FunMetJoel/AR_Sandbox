using UnityEngine;
using UnityEditor;


public struct float2
{
    public float x;
    public float y;

    public float2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }
}