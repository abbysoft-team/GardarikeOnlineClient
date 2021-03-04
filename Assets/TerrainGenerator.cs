using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
	LEFT,
	RIGTH,
	UP,
	DOWN,
	DOWN_LEFT,
	DOWN_RIGHT,
	UP_LEFT,
	UP_RIGHT
}

public class TerrainGenerator : MonoBehaviour
{
	public static TerrainGenerator instance;

	public float[,] heights;
	private Terrain referenceTerrain;

	private Dictionary<Direction, Terrain> activeChunks = new Dictionary<Direction, Terrain>(3);

	// Start is called before the first frame update
	void Start()
	{
		referenceTerrain = FindObjectOfType<Terrain>();

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
		referenceTerrain.GetComponent<TerrainCollider>().terrainData = terrainData;

		this.heights = heights;

		referenceTerrain.terrainData = terrainData;

		// Set camera at the saved point
		var x = PlayerPrefs.GetFloat("cameraX");
		var z = PlayerPrefs.GetFloat("cameraZ");
		var y = (GlobalConstants.MAX_CAMERA_Y - GlobalConstants.MIN_CAMERA_Y) / 2 + Utility.GetGroundedPoint(new Vector3(x, GlobalConstants.CHUNK_HEIGHT + 200f, z)).y;

		Camera.main.transform.position = new Vector3(x, y, z);
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