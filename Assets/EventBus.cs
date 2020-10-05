using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBus : MonoBehaviour
{
    public const string NEW_BUILDING_EVENT = "NEW_BUILDING";
    public static EventBus instance;

    //public event Action<BuildItem> onBuildingComplete;
    public event Action<float[,]> onTerrainGenerationFinished;
    public event Action<int, int, float[,]> onTerrainLoadingComplete;

    private void Awake()
    {
        instance = this;
    }

    //public void buildingComplete(BuildItem building)
    //{
    //    onBuildingComplete?.Invoke(building);
   // }

    public void TerrainGenerationFinished(float[,] heights)
    {
        onTerrainGenerationFinished?.Invoke(heights);
    }

    public void TerrainLoaded(int width, int height, float[,] heights)
    {
        onTerrainLoadingComplete?.Invoke(width, height, heights);
    }
}