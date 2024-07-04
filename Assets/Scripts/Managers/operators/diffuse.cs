using UnityEngine;

public class diffuse : MonoBehaviour
{
    [SerializeField]
    private floatLayer layer;

    [SerializeField]
    private ComputeShader diffuseShader;

    [SerializeField]
    private float diffusionRate = 0.1f;

    ComputeBuffer inputBuffer;
    ComputeBuffer outputBuffer;

    void Start()
    {
        int bufferlength = 2;
        inputBuffer = new ComputeBuffer(bufferlength, sizeof(float));
        outputBuffer = new ComputeBuffer(bufferlength, sizeof(float));
        float[] testFloats = new float[bufferlength];
        for (int i = 0; i < bufferlength; i++)
        {
            testFloats[i] = 0.5f;
        }


        // Set the input buffer to the current layer data
        inputBuffer.SetData(testFloats);

        int kernel = diffuseShader.FindKernel("Diffusion");
        diffuseShader.SetFloat("diffusionRate", diffusionRate);
        diffuseShader.SetBuffer(kernel, "inputBuffer", inputBuffer);
        diffuseShader.SetBuffer(kernel, "outputBuffer", outputBuffer);
        diffuseShader.Dispatch(kernel, 1, 1, 1);

        // Get the data from the output buffer
        outputBuffer.GetData(layer.LayerData);

        // Release the buffers
        inputBuffer.Release();
        outputBuffer.Release();
    }
}