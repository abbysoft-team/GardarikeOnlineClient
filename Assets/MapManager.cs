using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using Gardarike;

public class MapManager : MonoBehaviour
{
    public TerrainGenerator terrainGenerator;

    public static MapManager instance;

    void Start()
    {
        instance = this;
        EventBus.instance.onMapObjectsLoadingComplete += PlaceMapObjects;
    }

    private void PlaceMapObjects(RepeatedField<Building> buildings, int treesCount) {
        // foreach (var building in buildings) {
        //     EventBus.instance.RegisterBuilding(ToBuildingItem(building));
        // }

        // place trees
        EventBus.instance.SpawnTrees(200);
        EventBus.instance.MapIsReady();
    }

    private BuildItemInfo ToBuildingItem(Building building) {
        var item = new BuildItemInfo();
        //item.position = ProtoConverter.ToUnityVector(building.Location);
       // item.position = Utility.GetGroundedPoint(item.position);

        return item;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
