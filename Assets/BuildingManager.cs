using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;
    public List<BuildItem> buildings;
    public GameObject model;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        EventBus.instance.onBuildingRegistrationEvent += RegisterBuilding;
    }

    private void RegisterBuilding(BuildItem building)
    {
        var buildingObject = Instantiate(model);
        buildingObject.transform.position = building.position;
        buildingObject.transform.localScale = model.transform.lossyScale;
        buildingObject.transform.parent = transform;
        buildingObject.tag = GlobalConstants.BUILDING_TAG;
        model.SetActive(false);

        buildingObject.SetActive(true);

        buildings.Add(building);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
