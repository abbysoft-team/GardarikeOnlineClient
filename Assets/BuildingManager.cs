using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;
    public List<Building> buildings;
    private Building selectedBuilding;

    public GameObject model;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        EventBus.instance.onBuildingRegistrationEvent += RegisterBuilding;
        EventBus.instance.onClickWasMade += ProcessClick;

        buildings = new List<Building>();
    }

    private void ProcessClick(GameObject collider)
    {
        if (collider.tag != GlobalConstants.BUILDING_TAG) {
            if (selectedBuilding == null) return;
            selectedBuilding.Unselect();
            selectedBuilding = null;
        }

        var buildingComponent = collider.GetComponent<Building>();
        if (buildingComponent == null) return;
        SelectBuilding(buildingComponent);
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

        var buildingInfo = buildingObject.GetComponent<Building>();
        buildings.Add(buildingInfo);
        buildingInfo.Init();
    }

    public void BuildBuilding(string building, Transform info)
    {
        var reference = ResourceManager.GetReferenceObject(building);
        var copy = Instantiate(reference);

        copy.transform.parent = transform;
        copy.transform.position = info.position;
        copy.gameObject.SetActive(true);
       
        var buildingInfo = reference.GetComponent<Building>();
        buildings.Add(buildingInfo);
        buildingInfo.Init();
        buildingInfo.Rotate(info.rotation);

        SelectBuilding(buildingInfo);

        EventBus.instance.BuildingComplete(buildingInfo);
    }

    private void SelectBuilding(Building building)
    {
        if (selectedBuilding != null) {
            selectedBuilding.Unselect();
        }

        selectedBuilding = building;

        selectedBuilding.Select();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
