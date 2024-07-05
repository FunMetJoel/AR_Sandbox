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
    int kernel;

    void Awake()
    {
        inputBuffer = new ComputeBuffer(layer.world.size.x * layer.world.size.y, sizeof(float));
        outputBuffer = new ComputeBuffer(layer.world.size.x * layer.world.size.y, sizeof(float));

        kernel = diffuseShader.FindKernel("CSMain");
    }

    void Update()
    {
        // Set the data in the input buffer
        inputBuffer.SetData(layer.data);

        diffuseShader.SetFloat("_DiffusionRate", diffusionRate * Time.deltaTime);
        diffuseShader.SetInt("_Width", layer.world.size.x);
        diffuseShader.SetInt("_Height", layer.world.size.y);
        diffuseShader.SetBuffer(kernel, "_InputBuffer", inputBuffer);
        diffuseShader.SetBuffer(kernel, "_OutputBuffer", outputBuffer);
        diffuseShader.Dispatch(kernel, Mathf.CeilToInt(layer.world.size.x / 8.0f), Mathf.CeilToInt(layer.world.size.y / 8.0f), 1);

        // Get the data from the output buffer
        outputBuffer.GetData(layer.data);
    }

    private void OnDestroy() {
		outputBuffer.Release();
	}
}