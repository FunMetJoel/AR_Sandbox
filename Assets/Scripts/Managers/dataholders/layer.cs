using UnityEngine;

public abstract class layer : MonoBehaviour
{
    public world world;

    public void setWorld(world world)
    {
        this.world = world;
    }

    public abstract void initialiseLayer();
}