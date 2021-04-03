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

	private Vector2Int centralCell;

	private HashSet<Vector2Int> chunksToLoad = new HashSet<Vector2Int>();
	private HashSet<Vector2Int> loadedChunks = new HashSet<Vector2Int>();

	private float[,] bigChunkData = new float[GlobalConstants.CHUNK_RESOLUTION, GlobalConstants.CHUNK_RESOLUTION];
	private TerrainData data;

	private bool terrainLoaded = false;

	private InputManager inputManager;


	// Start is called before the first frame update
	void Start()
	{
		inputManager = InputManagerFatory.GetDefaultManager();
		referenceTerrain = FindObjectOfType<Terrain>();
		data = new TerrainData();
		data.heightmapResolution = GlobalConstants.CHUNK_RESOLUTION;
		data.size = new Vector3(GlobalConstants.CHUNK_SIZE, GlobalConstants.CHUNK_HEIGHT, GlobalConstants.CHUNK_SIZE);
		referenceTerrain.terrainData = data;

		//OnTerrainLoaded(100, 100, GetHeights());
		//GenerateRandomTerrain();
		//EventBus.instance.TerrainGenerationFinished(heights);
		EventBus.instance.onTerrainLoadingComplete += OnWorldChunkLoaded;
	}

	private void Awake()
	{
		instance = this;
	}

	// TODO Implement chunk pool to reduce instantiation count
	private void OnWorldChunkLoaded(float[,] heights, int chunkX, int chunkY)
	{
		var chunk = new Vector2Int(chunkX, chunkY);
		chunksToLoad.Remove(chunk);
		loadedChunks.Add(chunk);
		if (chunksToLoad.Count == 0) FinishTerrainGeneration();
	}

	private void FinishTerrainGeneration()
	{
		var serverChunks = GetServerChunks(centralCell.x, centralCell.y);

		FillBigChunkHeights(serverChunks);
		// chunk 0;0 will be at the origin
		var x = (centralCell.x - 1) * GlobalConstants.CHUNK_SIZE / 3.0f;
		var y = (centralCell.y - 1) * GlobalConstants.CHUNK_SIZE / 3.0f;

		ConfigureTerrainComponent(x, y, bigChunkData);

		var cameraPos = InputManagerFatory.GetDefaultManager().InitCameraPosition();
		var chunkPos = Utility.ToChunkPos(cameraPos);
		cameraCell = chunkPos;

		terrainLoaded = true;

		EventBus.instance.TerrainGenerationFinished(serverChunks);

		EventBus.instance.CloseLoadingDialog();
	}

	private List<Gardarike.GetWorldMapResponse> GetServerChunks(int x, int y)
	{
		var chunks = new List<Gardarike.GetWorldMapResponse>();

		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				chunks.Add(MapCache.GetGlobalChunk(x + i, y + j));
			}
		}

		return chunks;
	}

	private void FillBigChunkHeights(List<Gardarike.GetWorldMapResponse> serverChunks)
	{
		int chunkSize = (int) GlobalConstants.SERVER_CHUNK_SIZE;

		foreach (var chunk in serverChunks)
		{


			var offsetX = (chunk.Map.X - centralCell.x + 1) * chunkSize;
			var offsetY = (chunk.Map.Y - centralCell.y + 1) * chunkSize;

			var chunkData = ProtoConverter.ToHeightsFromProto(chunk.Map.Data);

			data.SetHeights(offsetX, offsetY, chunkData);
		}
	}

	private void ConfigureTerrainComponent(float x, float y, float[,] heights)
	{
		//var newTerrainObject = Instantiate(referenceTerrain);
		//referenceTerrain.transform.parent = transform;
		referenceTerrain.gameObject.SetActive(true);
		referenceTerrain.transform.position = new Vector3(x, 0, y);
		referenceTerrain.GetComponent<TerrainCollider>().terrainData = data;

		// cameraCell = new Vector2Int(0, 0);
		// ScrollAndPitch.instance.InitCameraPosition();
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

	public void LoadMap(int x, int y)
	{
		//EventBus.instance.OpenLoadingDialog();

		FillChunksToLoadAndLoaded(x, y);
		centralCell = new Vector2Int(x, y);

		if (chunksToLoad.Count == 0) {
			FinishTerrainGeneration();
			return;
		}

		foreach (var chunks in chunksToLoad)
		{
			EventBus.instance.LoadMap(chunks.x, chunks.y);	
		}

		cameraCell = new Vector2Int(x, y);
	}

	private void FillChunksToLoadAndLoaded(int x, int y)
	{
		chunksToLoad.Clear();
		loadedChunks.Clear();

		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				var miss = MapCache.ChunkIsMissing(j + x, i + y, true);
				if (miss)
				{
					chunksToLoad.Add(new Vector2Int(j + x, i + y));
				} else
				{
					loadedChunks.Add(new Vector2Int(j + x, i + y));
				}
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

		//OnTerrainLoaded(heights, 0, 0);
	}

	// Update is called once per frame
	void Update()
	{

	}
    public void CameraMoved(Vector3 position)
    {  
		if (!terrainLoaded) return;

		var x = position.x;
		var y = position.z;

		var chunkPos = Utility.ToChunkPos(position);

		int diffX = cameraCell.x - chunkPos.x;
		int diffY = cameraCell.y - chunkPos.y;

		if (diffX == 0 && diffY == 0) return;

		// reload all map, camera chunk is changed
		LoadMap(chunkPos.x, chunkPos.y);

		cameraCell = new Vector2Int(chunkPos.x, chunkPos.y);
    }

	private bool ChunkIsActive(int x, int y)
	{
		return activeChunks.ContainsKey(GetChunkKey(x, y));
	}

	private string GetChunkKey(int x, int y)
	{
		return x + ";" + y;
	}

	public void ClearActiveChunks()
	{
		activeChunks.Clear();
	}

	public float GetHeight(float x, float y)
	{
		var chunkPos = Utility.ToServerCoordinates(x, y);
		chunkPos.X += GlobalConstants.SERVER_CHUNK_SIZE;
		chunkPos.Y += GlobalConstants.SERVER_CHUNK_SIZE;

		return data.GetHeight((int) chunkPos.X, (int) chunkPos.Y);
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