using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLogic : MonoBehaviour
{
    private BuildingState state;
    public Material prototypeMaterial;
    private GameObject building;

    void Start()
    {
        EventBus.instance.onChooseLocationForBuilding += ChooseBuildingLocation;
    }
    public void ChooseBuildingLocation(int eventId, GameObject referenceBuilding)
    {
        Debug.Log("start building");
        
        state = BuildingState.LOCATION_CHOOSE;

        building = Instantiate(referenceBuilding);
        Utility.SetMaterialForAllChildren(building, prototypeMaterial);
        building.transform.position = Utility.GetPointOnTheGroundInFrontOfCamera();
        building.SetActive(true);
    }

    void Update()
    {
        var touchHappen = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Stationary;
        if (!touchHappen) return;

        if (state == BuildingState.LOCATION_CHOOSE)
        {
            building.transform.position = Utility.GetPositionOnTheGround(Input.GetTouch(0).position);
        }


        //EventBus.instance.NotifyEventFinished(eventId, building.transform.position);
    }

    private enum BuildingState {
        LOCATION_CHOOSE,
        ONGOING_BUILDING,
        CONSTRUCTED
    }
}
