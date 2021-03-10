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

	private Vector2Int cameraCell;

	private List<Vector2Int> chunksToLoad = new List<Vector2Int>(GlobalConstants.MAX_ACTIVE_CHUNKS);

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

	// TODO Implement chunk pool to reduce instantiation count
	private void OnTerrainLoaded(float[,] heights, int chunkX, int chunkY)
	{
		if (activeChunks.Count >= GlobalConstants.MAX_ACTIVE_CHUNKS)
		{
			UnloadFarthestChunk(chunkX, chunkY);
		}

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

		activeChunks.Add(GetChunkKey(chunkX, chunkY), newTerrainObject);

		if (activeChunks.Count == 1)
		{
			cameraCell = new Vector2Int(0, 0);
			ScrollAndPitch.instance.InitCameraPosition();
		} else if (activeChunks.Count >= 9)
		{
			EventBus.instance.CloseLoadingDialog();
		}
	}

	private void UnloadFarthestChunk(int chunkX, int chunkY)
	{
		Debug.LogFormat("Unload chunk {0};{1}", chunkX, chunkY);
		var farthestX = 0;
		var farthestY = 0;
		var maxDistance = 0.0;
		foreach (var chunk in activeChunks)
		{
			var x = int.Parse(chunk.Key.Split(';')[0]);
			var y = int.Parse(chunk.Key.Split(';')[1]);

			var dx = Math.Abs(x - chunkX);
			var dy = Math.Abs(y - chunkY);

			var distance = Math.Sqrt(dx * dx + dy * dy);

			if (distance > maxDistance)
			{
				maxDistance = distance;
				farthestX = x;
				farthestY = y;
			}
		}

		var farthestChunk = activeChunks[GetChunkKey(farthestX, farthestY)];
		activeChunks.Remove(GetChunkKey(farthestX, farthestY));

		farthestChunk.gameObject.SetActive(false);
	}

	public void LoadMap()
	{
		SetRandomTerrain();

		//EventBus.instance.OpenLoadingDialog();
		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				//MapCache.LoadGlobalChunk(i, j);
			}
		}
	}

	private void SetRandomTerrain()
	{
		float[,] heights = new float[GlobalConstants.CHUNK_RESOLUTION, GlobalConstants.CHUNK_RESOLUTION];

		for (int i = 0; i < GlobalConstants.CHUNK_RESOLUTION; i++)
		{
			for (int j = 0; j < GlobalConstants.CHUNK_RESOLUTION; j++)
			{
				heights[i, j] = 0.6f;
			}
		}

		OnTerrainLoaded(heights, 0, 0);
	}

	// Update is called once per frame
	void Update()
	{

	}
    public void CameraMoved(Vector3 position)
    {   
		if (activeChunks.Count < 1) return;

		var x = position.x;
		var y = position.z;

		if (x < 0) x-= GlobalConstants.CHUNK_SIZE;
		if (y < 0) y-= GlobalConstants.CHUNK_SIZE;

		int chunkX = (int) (x / GlobalConstants.CHUNK_SIZE);
		int chunkY = (int) (y / GlobalConstants.CHUNK_SIZE);

		int diffX = cameraCell.x - chunkX;
		int diffY = cameraCell.y - chunkY;

		if (diffX == 0 && diffY == 0) return;

		chunksToLoad.Clear();

		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				if (ChunkIsActive(chunkX + i, chunkY + j)) continue;

				MapCache.LoadGlobalChunk(chunkX + i, chunkY + j);
			}
		}

		cameraCell = new Vector2Int(chunkX, chunkY);
    }

	private bool ChunkIsActive(int x, int y)
	{
		return activeChunks.ContainsKey(GetChunkKey(x, y));
	}

	private string GetChunkKey(int x, int y)
	{
		return x + ";" + y;
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