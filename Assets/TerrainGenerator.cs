using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
	public static TerrainGenerator instance;

	private const int HEIGHT_MAP_RESOLUTION = 100;

	public int GRAIN = 8;
	public bool GENERATE_PLAINS = false;
	public Material material;
	public float hillCoeff;
	public float[,] heights;
	private Terrain terrain;

	// Start is called before the first frame update
	void Start()
	{
		//GenerateRandomTerrain();
		//EventBus.instance.TerrainGenerationFinished(heights);
		terrain = FindObjectOfType<Terrain>();
		EventBus.instance.onTerrainLoadingComplete += OnTerrainLoaded;
	}

	private void Awake()
	{
		instance = this;
	}

	private void OnTerrainLoaded(int width, int height, float[,] heights)
	{
		terrain.terrainData.size = new Vector3(width, height, width);
		terrain.terrainData.heightmapResolution = width;
		terrain.terrainData.SetHeights(0, 0, heights);

		this.heights = heights;
	}

	private void GenerateRandomTerrain()
	{
		var terrain = FindObjectOfType<Terrain>();

		var texture = new Texture2D(HEIGHT_MAP_RESOLUTION, HEIGHT_MAP_RESOLUTION);
		var colors = new Color32[HEIGHT_MAP_RESOLUTION * HEIGHT_MAP_RESOLUTION];
		colors = drawPlasma(HEIGHT_MAP_RESOLUTION, colors);

		texture.SetPixels32(colors);
		texture.Apply();

		material.SetTexture("_HeightTex", texture);

		float[,] heights = new float[HEIGHT_MAP_RESOLUTION, HEIGHT_MAP_RESOLUTION];
		for (int i = 0; i < HEIGHT_MAP_RESOLUTION; i++)
		{
			for (int k = 0; k < HEIGHT_MAP_RESOLUTION; k++)
			{
				heights[i, k] = texture.GetPixel(i, k).grayscale * hillCoeff;
			}
		}

		terrain.terrainData.size = new Vector3(HEIGHT_MAP_RESOLUTION, HEIGHT_MAP_RESOLUTION, HEIGHT_MAP_RESOLUTION);
		terrain.terrainData.heightmapResolution = HEIGHT_MAP_RESOLUTION;
		terrain.terrainData.SetHeights(0, 0, heights);

		this.heights = heights;
	}

	float displace(float num)
	{
		var widthPlusHeight = HEIGHT_MAP_RESOLUTION * 2;
		float max = num / widthPlusHeight * GRAIN;
		return UnityEngine.Random.Range(-0.5f, 0.5f) * max;
	}

	Color32[] drawPlasma(float resolution, Color32[] colors)
	{
		float c1, c2, c3, c4;

		c1 = UnityEngine.Random.value;
		c2 = UnityEngine.Random.value;
		c3 = UnityEngine.Random.value;
		c4 = UnityEngine.Random.value;

		return divide(colors, 0.0f, 0.0f, resolution, resolution, c1, c2, c3, c4);
	}

	Color32[] divide(Color32[] colors, float x, float y, float w, float h, float c1, float c2, float c3, float c4)
	{

		float newWidth = w * 0.5f;
		float newHeight = h * 0.5f;

		if (w < 1.0f && h < 1.0f)
		{
			float c = (c1 + c2 + c3 + c4) * 0.25f;
			colors[(int)x + (int)y * HEIGHT_MAP_RESOLUTION] = new Color(c, c, c);
		}
		else
		{
			float middle = (c1 + c2 + c3 + c4) * 0.25f + displace(newWidth + newHeight);
			float edge1 = (c1 + c2) * 0.5f;
			float edge2 = (c2 + c3) * 0.5f;
			float edge3 = (c3 + c4) * 0.5f;
			float edge4 = (c4 + c1) * 0.5f;

			if (!GENERATE_PLAINS)
			{
				if (middle <= 0)
				{
					middle = 0;
				}
				else if (middle > 1.0f)
				{
					middle = 1.0f;
				}
			}
			divide(colors, x, y, newWidth, newHeight, c1, edge1, middle, edge4);
			divide(colors, x + newWidth, y, newWidth, newHeight, edge1, c2, edge2, middle);
			divide(colors, x + newWidth, y + newHeight, newWidth, newHeight, middle, edge2, c3, edge3);
			divide(colors, x, y + newHeight, newWidth, newHeight, edge4, middle, edge3, c4);

			return colors;
		}

		return colors;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void UpdateTerrain()
	{
		var terrain = FindObjectOfType<Terrain>();

		terrain.terrainData.size = new Vector3(HEIGHT_MAP_RESOLUTION, HEIGHT_MAP_RESOLUTION, HEIGHT_MAP_RESOLUTION);
		terrain.terrainData.heightmapResolution = HEIGHT_MAP_RESOLUTION;
		terrain.terrainData.SetHeights(0, 0, heights);

		EventBus.instance.TerrainGenerationFinished(heights);
	}


}