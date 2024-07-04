using System.Numerics;
using UnityEngine;


public class testFlow : MonoBehaviour
{
    public ComputeShader computeShader;

    void Start()
    {

        // Define the grid size
        int gridWidth = 256;
        int gridHeight = 256;

        // Define the data
        float value1 = 3.0f;
        float value2 = 4.0f;
        float2 vector = new float2(5.0f, 6.0f);

        // Create a buffer for the input data
        ComputeBuffer inputBuffer = new ComputeBuffer(4*4, sizeof(float) * 4);
        ComputeBuffer outputBuffer = new ComputeBuffer(4*4, sizeof(float) * 4);

        // Create an array to hold the input data
        FloatData[] inputData = new FloatData[4*4];
        for (int i = 0; i < inputData.Length; i++)
        {
            inputData[i] = new FloatData(value1, value2, vector);
        }

        // Set the data for the input buffer
        inputBuffer.SetData(inputData);

        // Find the kernel index
        int kernelHandle = computeShader.FindKernel("CSMain");

        // Set the buffers for the compute shader
        computeShader.SetBuffer(kernelHandle, "inputBuffer", inputBuffer);
        computeShader.SetBuffer(kernelHandle, "outputBuffer", outputBuffer);

        // Dispatch the compute shader
        int threadGroupsX = Mathf.CeilToInt((float)gridWidth / 8);
        int threadGroupsY = Mathf.CeilToInt((float)gridHeight / 8);
        computeShader.Dispatch(kernelHandle, threadGroupsX, threadGroupsY, 1);

        // Retrieve the data from the output buffer
        FloatData[] result = new FloatData[1];
        outputBuffer.GetData(result);

        // Log the result
        for (int i = 0; i < result.Length; i++)
        {
            Debug.Log("Result: " + result[i]);
        }

        // Release the buffers
        inputBuffer.Release();
        outputBuffer.Release();
    }

    struct FloatData
    {
        public float movableDencity;
        public float unmovableDencity;
        public float2 velocity;

        public FloatData(float movableDencity, float unmovableDencity, float2 velocity)
        {
            this.movableDencity = movableDencity;
            this.unmovableDencity = unmovableDencity;
            this.velocity = velocity;
        }

        public override string ToString()
        {
            return "(movableDencity: " + movableDencity + " unmovableDencity: " + unmovableDencity + " velocity: " + velocity + ")";
        }
    }
}