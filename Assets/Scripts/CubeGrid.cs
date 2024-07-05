using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CubeGrid : MonoBehaviour
{

	[Header("Prefabs")]
	public Transform CubePrefab;

	[Header("Shaders")]
	public ComputeShader CubeShader;

	[Header("Cubes")]
	public int CubesPerAxis;

	private Transform[] _cubes;
	private float[] _cubesPositions;

	private ComputeBuffer _cubesPositionBuffer;

	private void Awake() {
		_cubesPositionBuffer = new ComputeBuffer(CubesPerAxis * CubesPerAxis, sizeof(float));
	}

	private void OnDestroy() {
		_cubesPositionBuffer.Release();
	}

	private void Start() {
		CreateGrid();
	}

	void CreateGrid() {
		_cubes = new Transform[CubesPerAxis * CubesPerAxis];
		_cubesPositions = new float[CubesPerAxis * CubesPerAxis];

		for (int x = 0, i = 0; x < CubesPerAxis; x++) {
			for (int z = 0; z < CubesPerAxis; z++, i++) {
				_cubes[i] = Instantiate(CubePrefab, transform);
				_cubes[i].transform.position = new Vector3(x, 0, z);
			}
		}

		UpdatePositionsGPU();
		//StartCoroutine(UpdateCubeGrid());
	}

	// IEnumerator UpdateCubeGrid() {
	// 	while (true) {
	// 		UpdatePositionsGPU();
	// 		yield return new WaitForSeconds(1);
	// 	}
	// }

	void UpdatePositionsGPU() {
		CubeShader.SetBuffer(0, "_Positions", _cubesPositionBuffer);

		CubeShader.SetInt("_CubesPerAxis", CubesPerAxis);

		int workgroups = Mathf.CeilToInt(CubesPerAxis / 8.0f);

		CubeShader.Dispatch(0, workgroups, workgroups, 1);

		_cubesPositionBuffer.GetData(_cubesPositions);
	}

}
