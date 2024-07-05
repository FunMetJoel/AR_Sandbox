public class CubeGridProfiler 
{
    private int _CPUCalls;
    private float _CPUTime;

    private int _GPUCalls;
    private float _GPUTime;

    public float AverageGPUTimeInMs {
        get {
            return _GPUTime / (float)_GPUCalls;
        }
    }

	public float AverageCPUTimeInMs {
		get {
            return _CPUTime / (float)_CPUCalls;

		}
	}

    public void Reset() {
        _CPUCalls = 0;
		_CPUTime = 0;
		_GPUCalls = 0;
		_GPUTime = 0; 
	}

	public void AddCPUCall(float time) {
        _CPUCalls++;
        _CPUTime += time;
    }


	public void AddGPUCall(float time) {
		_GPUCalls++;
        _GPUTime += time;
	}
}
