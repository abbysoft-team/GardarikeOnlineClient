using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using Gardarike;

public class MapManager : MonoBehaviour
{
    public TerrainGenerator terrainGenerator;

    void Start()
    {
        EventBus.instance.onLoginComplete += SendMapRequest;
        EventBus.instance.onMapObjectsLoadingComplete += PlaceMapObjects;
    }

    private void SendMapRequest(string sessionId, RepeatedField<Character> character) {
        Debug.Log("Start loading map");

        EventBus.instance.LoadMap(sessionId);
    }

    private void PlaceMapObjects(RepeatedField<Building> buildings) {
        foreach (var building in buildings) {
            Debug.Log("Register building: " + building);
            EventBus.instance.RegisterBuilding(ToBuildingItem(building));
        }
    }

    private BuildItem ToBuildingItem(Building building) {
        var item = new BuildItem();
        item.position = ProtoConverter.ToUnityVector(building.Location);

        return item;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
