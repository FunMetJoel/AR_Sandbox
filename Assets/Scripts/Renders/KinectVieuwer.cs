using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KinectVieuwer : MonoBehaviour
{
    public GetKinectDepth mGetKinectDepth;
    public MultiSourceManager mMultiSource;

    public RawImage mRawImage;
    public RawImage mRawDepth;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mRawImage.texture = mMultiSource.GetColorTexture();

        mRawDepth.texture = mGetKinectDepth.mDepthTexture;
    }
}
