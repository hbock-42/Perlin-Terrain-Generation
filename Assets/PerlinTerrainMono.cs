using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinTerrainMono : MonoBehaviour
{
	[SerializeField] private int _widthTerrain = 10;
	[SerializeField, Tooltip("Heigh is in fact the depth of the terrain")] private int _heightTerrain = 10;
	[SerializeField] private float _widthSize = 1;
	[SerializeField] private float _heightSize = 1;
	[SerializeField] private float _trueHeighMultiplier = 6f;
	[SerializeField] private float _perlinXMultiplier = 4f;
	[SerializeField] private float _perlinYMultiplier = 6.5f;
	[SerializeField] private float _speed = 3f;
	[SerializeField] private float _yTrans = 0;

	[Header("Links")]
	[SerializeField] private MeshFilter _mf;

	private Mesh _terrainMesh;

	private void Start()
	{
		_terrainMesh = PerlinTerrain.GetMesh(_widthTerrain, _heightTerrain, _widthSize, _heightSize, _trueHeighMultiplier, _perlinXMultiplier, _perlinYMultiplier);
		_mf.mesh = _terrainMesh;
	}

	private void Update()
	{
		UpdateTerrain();
	}

	private void UpdateTerrain()
	{
		_yTrans -= Time.deltaTime * _speed;

		Vector3[] vertices = _terrainMesh.vertices;

		for (int y = 0; y < _heightTerrain; y++)
		{
			for (int x = 0; x < _widthTerrain; x++)
			{
				Vector3 v = vertices[y * _widthTerrain + x];
				v.y = Mathf.PerlinNoise((float)x / (float)(_widthTerrain - 1) * _perlinXMultiplier, (float)y / (float)(_heightTerrain - 1) * _perlinYMultiplier - _yTrans) * _trueHeighMultiplier;
				vertices[y * _widthTerrain + x] = v;
			}
		}
		_terrainMesh.vertices = vertices;
		_terrainMesh.RecalculateNormals();
		_terrainMesh.RecalculateTangents();
	}
}


public static class PerlinTerrain
{

	private static int _width;
	private static int _height;

	/// <summary>
	/// Build a mesh representing a terrain
	/// </summary>
	/// <param name="pWidth">Number of vertices for the width</param>
	/// <param name="pHeight">Number of vertices for the height</param>
	/// <param name="pWidthSize">The width of a case</param>
	/// <param name="pHeightSize">The width of a case</param>
	/// <returns>A mesh representing a terrain in 3d space</returns>
	public static Mesh GetMesh(int pWidth, int pHeight, float pWidthSize, float pHeightSize, float pHeightMultiplier, float pPerlingXMultiplier, float pPerlingYMultiplier)
	{
		_width = pWidth;
		_height = pHeight;

		Vector3[] vertices = new Vector3[pWidth * pHeight];
		int[] indices = new int[6 * (pWidth - 1) * (pHeight - 1)];
		int indicesIndex = 0;

		for (int y = 0; y < pHeight; y++)
		{
			for (int x = 0; x < pWidth; x++)
			{
				int index = GetIndex(x, y);
				vertices[index] = new Vector3((float)x * pWidthSize, Mathf.PerlinNoise((float)x / (float)(pWidth - 1) * pPerlingXMultiplier, (float)y / (float)(pHeight - 1) * pPerlingYMultiplier) * pHeightMultiplier, (float)y * pHeightSize);
				if (y != pHeight - 1)
				{
					if (x != pWidth - 1)
					{
						indices[indicesIndex] = index;
						indices[indicesIndex + 1] = index + pWidth;
						indices[indicesIndex + 2] = index + 1;
						indicesIndex += 3;
					}
					if (x != 0)
					{
						indices[indicesIndex] = index;
						indices[indicesIndex + 1] = index + pWidth - 1;
						indices[indicesIndex + 2] = index + pWidth;
						indicesIndex += 3;
					}
				}
			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.SetIndices(indices, MeshTopology.Triangles, 0, true);
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();
		return mesh;
	}

	/// <summary>
	/// Helper function that return the real index of the one dimension vertices array
	/// </summary>
	private static int GetIndex(int x, int y)
	{
		return y * _width + x;
	}
}