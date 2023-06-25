using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour
{
    [SerializeField] private GameObject[] managers;

    // Start is called before the first frame update
    void Start()
    {
        World.Instance.GenerateWorld();
        foreach (GameObject manager in managers)
        {
            manager.SetActive(true);
        }
    }
}
