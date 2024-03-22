using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTexture : MonoBehaviour
{
    public Material material;
    public InfraredSourceManager ism;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ism._Texture != null)
        {
            material.SetTexture("_Height", ism._Texture);
        }
    }
}
