﻿using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class InfraredSourceManager : MonoBehaviour
{
    private KinectSensor _Sensor;
    private InfraredFrameReader _Reader;
    private ushort[] _Data;
    private byte[] _RawData;
    private ushort[] _IntencityData;
    private byte[] _IntencityData2;
    public Vector2Int Size;

    // I'm not sure this makes sense for the Kinect APIs
    // Instead, this logic should be in the VIEW
    public Texture2D _Texture;

    public Texture2D GetInfraredTexture()
    {
        return _Texture;
    }

    public ushort[] GetIntencityData()
    {
        return _IntencityData;
    }
    public byte[] GetIntencityData2()
    {
        return _IntencityData2;
    }

    void Start()
    {
        _Sensor = KinectSensor.GetDefault();
        if (_Sensor != null)
        {
            _Reader = _Sensor.InfraredFrameSource.OpenReader();
            var frameDesc = _Sensor.InfraredFrameSource.FrameDescription;
            _Data = new ushort[frameDesc.LengthInPixels];
            _RawData = new byte[frameDesc.LengthInPixels * 4];
            _Texture = new Texture2D(frameDesc.Width, frameDesc.Height, TextureFormat.BGRA32, false);
            _IntencityData = new ushort[frameDesc.LengthInPixels];
            _IntencityData2 = new byte[frameDesc.LengthInPixels];
            Size = new Vector2Int(frameDesc.Width, frameDesc.Height);
            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }
    }

    void Update()
    {
        if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {
                frame.CopyFrameDataToArray(_Data);

                int index = 0;
                int i = 0;

                foreach (var ir in _Data)
                {
                    byte intensity = (byte)(ir >> 8); 
                    _IntencityData2[i] = intensity;
                    _IntencityData[i++] = ir;
                    _RawData[index++] = intensity;
                    _RawData[index++] = intensity;
                    _RawData[index++] = intensity;
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
