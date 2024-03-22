using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class DepthSourceManager : MonoBehaviour
{   
    private KinectSensor _Sensor;
    private DepthFrameReader _Reader;
    private ushort[] _Data;
    private byte[] _RawData;
    public Texture2D _Texture;
    public Vector2 SchaleAndPan;

    public ushort[] GetData()
    {
        return _Data;
    }

    void Start () 
    {
        _Sensor = KinectSensor.GetDefault();
        
        if (_Sensor != null) 
        {
            _Reader = _Sensor.DepthFrameSource.OpenReader();
            var frameDesc = _Sensor.DepthFrameSource.FrameDescription;
            _Data = new ushort[_Sensor.DepthFrameSource.FrameDescription.LengthInPixels];
            _RawData = new byte[frameDesc.LengthInPixels * 4];
            _Texture = new Texture2D(frameDesc.Width, frameDesc.Height, TextureFormat.BGRA32, false);
        }
    }
    
    void Update () 
    {
        if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {
                frame.CopyFrameDataToArray(_Data);
                int index = 0;

                foreach (var ir in _Data)
                {
                    int intensity = (int)ir;

                    intensity = Mathf.RoundToInt((float)intensity * SchaleAndPan.x + SchaleAndPan.y);
                    
                    byte intens = (byte)(255 - Mathf.Clamp(intensity, 0, 255));
                    _RawData[index++] = intens;
                    _RawData[index++] = intens;
                    _RawData[index++] = intens;
                    _RawData[index++] = 255; // Alpha
                }

                _Texture.LoadRawTextureData(_RawData);
                _Texture.Apply();

                frame.Dispose();
                frame = null;
            }
        }
    }
    
    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }
        
        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }
            
            _Sensor = null;
        }
    }
}
