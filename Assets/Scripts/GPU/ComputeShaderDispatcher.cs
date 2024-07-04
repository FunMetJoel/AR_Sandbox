using UnityEngine;

public class ComputeShaderDispatcher : MonoBehaviour
{
    public ComputeShader computeShader;

    private int gridWidth = 8;
    private int gridHeight = 16;
    private int kernelHandle;
    private ComputeBuffer gridBuffer;
    private float[] gridData;

    void Start()
    {
        // Initialize the grid data
        gridData = new float[gridWidth * gridHeight];
        for (int i = 0; i < gridData.Length; i++)
        {
            gridData[i] = Random.Range(0.0f, 10.0f); // Random initial values
        }

        // Create a compute buffer and set data
        gridBuffer = new ComputeBuffer(gridData.Length, sizeof(float));
        gridBuffer.SetData(gridData);

        // Find the kernel in the compute shader
        kernelHandle = computeShader.FindKernel("CSMain");

        // Set the buffer on the compute shader
        computeShader.SetBuffer(kernelHandle, "grid", gridBuffer);

        // Dispatch the compute shader with updated parameters
        int threadGroupsX = Mathf.CeilToInt((float)gridWidth / 8);
        int threadGroupsY = Mathf.CeilToInt((float)gridHeight / 8);
        computeShader.Dispatch(kernelHandle, threadGroupsX, threadGroupsY, 1);

        // Retrieve the data from the compute buffer
        gridBuffer.GetData(gridData);

        // Print the updated grid data
        for (int i = 0; i < gridHeight; i++)
        {
            string row = "";
            for (int j = 0; j < gridWidth; j++)
            {
                row += gridData[i * gridWidth + j].ToString("F1") + " ";
            }
            Debug.Log(row);
        }

        // Release the compute buffer
        gridBuffer.Release();
    }
}
