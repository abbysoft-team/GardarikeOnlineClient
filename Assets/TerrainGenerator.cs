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
	private Terrain referenceTerrain;

	private Dictionary<string, Terrain> activeChunks = new Dictionary<string, Terrain>(10);

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

	private void OnTerrainLoaded(float[,] heights, int chunkX, int chunkY)
	{
		var terrainData = new TerrainData();

		terrainData.heightmapResolution = GlobalConstants.CHUNK_RESOLUTION;
		terrainData.size = new Vector3(GlobalConstants.CHUNK_SIZE, GlobalConstants.CHUNK_HEIGHT, GlobalConstants.CHUNK_SIZE);
		terrainData.SetHeights(0, 0, heights);

		var newTerrainObject = Instantiate(referenceTerrain);
		newTerrainObject.transform.parent = transform;
		newTerrainObject.GetComponent<TerrainCollider>().terrainData = terrainData;
		newTerrainObject.terrainData = terrainData;
		newTerrainObject.gameObject.SetActive(true);
		newTerrainObject.transform.position = new Vector3(chunkX * GlobalConstants.CHUNK_SIZE, 0, chunkY * GlobalConstants.CHUNK_SIZE);

		activeChunks.Add("" + chunkX + ";" + chunkY, newTerrainObject);

		if (activeChunks.Count == 1)
		{
			ScrollAndPitch.instance.InitCameraPosition();
		} else if (activeChunks.Count >= 9)
		{
			EventBus.instance.CloseLoadingDialog();
		}
	}

	public void LoadMap()
	{
		EventBus.instance.OpenLoadingDialog();
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				EventBus.instance.LoadMap(PlayerPrefs.GetString("sessionId"), i, j);
			}
		}
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