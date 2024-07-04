using System.Numerics;
using UnityEngine;

public class world : MonoBehaviour
{
    public Vector2Int size;

    void Awake()
    {
        // Get all children with the layer component
        layer[] layers = GetComponentsInChildren<layer>();
        foreach (layer l in layers)
        {
            l.setWorld(this);
            l.initialiseLayer();
        }
    }
}