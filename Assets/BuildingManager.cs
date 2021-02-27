using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;
    public List<Building> buildings;

    public GameObject model;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        EventBus.instance.onBuildingRegistrationEvent += RegisterBuilding;

        buildings = new List<Building>();
    }

    private void RegisterBuilding(BuildItemInfo building)
    {
        var buildingObject = Instantiate(model);
        buildingObject.transform.position = building.position;
        buildingObject.transform.localScale = model.transform.lossyScale;
        buildingObject.transform.parent = transform;
        buildingObject.tag = GlobalConstants.BUILDING_TAG;
        model.SetActive(false);

        buildingObject.SetActive(true);

        //buildings.Add(building);
    }

    public void BuildBuilding(GameObject building)
    {
        building.transform.parent = transform;
        building.SetActive(true);
       
        var buildingInfo = building.GetComponent<Building>();
        buildings.Add(buildingInfo);

        
        EventBus.instance.BuildingComplete(buildingInfo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
