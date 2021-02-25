using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
	public static TerrainGenerator instance;

	public float[,] heights;
	private Terrain terrain;

	// Start is called before the first frame update
	void Start()
	{
		terrain = FindObjectOfType<Terrain>();

		//OnTerrainLoaded(100, 100, GetHeights());
		//GenerateRandomTerrain();
		//EventBus.instance.TerrainGenerationFinished(heights);
		EventBus.instance.onTerrainLoadingComplete += OnTerrainLoaded;
	}

	private void Awake()
	{
		instance = this;
	}

	private void OnTerrainLoaded(int width, int height, float[,] heights)
	{
		var terrainData = new TerrainData();

		terrainData.heightmapResolution = GlobalConstants.CHUNK_RESOLUTION;
		terrainData.size = new Vector3(GlobalConstants.CHUNK_SIZE, GlobalConstants.CHUNK_HEIGHT, GlobalConstants.CHUNK_SIZE);
		terrainData.SetHeights(0, 0, heights);
		GetComponent<TerrainCollider>().terrainData = terrainData;

		this.heights = heights;

		terrain.terrainData = terrainData;
	}

	// Update is called once per frame
	void Update()
	{

	}

	// public void UpdateTerrain()
	// {
	// 	var terrain = FindObjectOfType<Terrain>();

	// 	Debug.Log("LOG");

	// 	terrain.terrainData.heightmapResolution = HEIGHT_MAP_RESOLUTION;
	// 	terrain.terrainData.SetHeights(0, 0, heights);

	// 	terrain.terrainData.size = new Vector3(HEIGHT_MAP_RESOLUTION, HEIGHT_MAP_RESOLUTION, HEIGHT_MAP_RESOLUTION);

	// 	EventBus.instance.TerrainGenerationFinished(heights);
	// }

}