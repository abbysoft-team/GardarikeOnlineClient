using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gardarike;
using Google.Protobuf.Collections;

public class EventBus : MonoBehaviour
{
    public const string NEW_BUILDING_EVENT = "NEW_BUILDING";
    public static EventBus instance;

    public event Action<BuildItem> onBuildingComplete;
    public event Action<float[,]> onTerrainGenerationFinished;
    public event Action<int, int, float[,]> onTerrainLoadingComplete;
    public event Action<string, RepeatedField<Character>> onLoginComplete;
    public event Action<string> onErrorShowRequest;
    public event Action<string, string> onLoginRequest;
    public event Action<string> onMapLoadRequest;

    private void Awake()
    {
        instance = this;
    }

    public void buildingComplete(BuildItem building)
    {
        onBuildingComplete?.Invoke(building);
    }

    public void TerrainGenerationFinished(float[,] heights)
    {
        onTerrainGenerationFinished?.Invoke(heights);
    }

    public void TerrainLoaded(int width, int height, float[,] heights)
    {
        onTerrainLoadingComplete?.Invoke(width, height, heights);
    }

    public void LoginComplete(string sessionId, RepeatedField<Character> characters) {
        onLoginComplete?.Invoke(sessionId, characters);
    }

    public void ShowError(string error)
    {
        onErrorShowRequest?.Invoke(error);
    }

    public void LoginRequest(string username, string password)
    {
        onLoginRequest?.Invoke(username, password);
    }

    public void LoadMap(string sessionId) {
        onMapLoadRequest?.Invoke(sessionId);
    }
}
