using UnityEngine;

public class testCompute : MonoBehaviour
{
    public ComputeShader computeShader;

    void Start()
    {
        // Define the data
        float value1 = 3.0f;
        float value2 = 4.0f;

        // Create a buffer for the input data
        ComputeBuffer inputBuffer = new ComputeBuffer(1, sizeof(float) * 2);
        ComputeBuffer outputBuffer = new ComputeBuffer(1, sizeof(float));

        // Create an array to hold the input data
        FloatData[] inputData = new FloatData[1];
        inputData[0].value1 = value1;
        inputData[0].value2 = value2;

        // Set the data for the input buffer
        inputBuffer.SetData(inputData);

        // Find the kernel index
        int kernelHandle = computeShader.FindKernel("CSMain");

        // Set the buffers for the compute shader
        computeShader.SetBuffer(kernelHandle, "inputBuffer", inputBuffer);
        computeShader.SetBuffer(kernelHandle, "outputBuffer", outputBuffer);

        // Dispatch the compute shader
        computeShader.Dispatch(kernelHandle, 1, 1, 1);

        // Retrieve the data from the output buffer
        float[] result = new float[1];
        outputBuffer.GetData(result);

        // Log the result
        Debug.Log("Sum: " + result[0]);

        // Release the buffers
        inputBuffer.Release();
        outputBuffer.Release();
    }

    struct FloatData
    {
        public float value1;
        public float value2;
    }
}