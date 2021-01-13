using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLogic : MonoBehaviour
{
    private const float UI_PROTOTYPE_HEIGHT = 40.0f;
    private BuildingState state;
    public Material prototypeMaterial;
    private GameObject building;
    public GameObject prototypingUI;

    void Start()
    {
        EventBus.instance.onChooseLocationForBuilding += ChooseBuildingLocation;
        prototypingUI.SetActive(false);
    }

    public void ChooseBuildingLocation(int eventId, GameObject referenceBuilding)
    {
        Debug.Log("start building");
        
        state = BuildingState.LOCATION_CHOOSE;

        building = Instantiate(referenceBuilding);
        Utility.SetMaterialForAllChildren(building, prototypeMaterial);
        building.transform.position = Utility.GetPointOnTheGroundInFrontOfCamera();
        building.SetActive(true);

        ConfigurePrototypingUI(building);
    }

    private void ConfigurePrototypingUI(GameObject prototype)
    {
        prototypingUI.transform.position = 
        new Vector3(prototype.transform.position.x, prototype.transform.position.y + UI_PROTOTYPE_HEIGHT, prototype.transform.position.z);
    }

    void Update()
    {
        //var touchHappen = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Stationary;
        var touchHappen = Input.GetMouseButton(0);
        if (!touchHappen) return;

        if (state == BuildingState.LOCATION_CHOOSE)
        {
            building.transform.position = Utility.GetPositionOnTheGround(Input.GetTouch(0).position);
        }


        //EventBus.instance.NotifyEventFinished(eventId, building.transform.position);
    }

    public void ApplyBuilding()
    {

    }

    public void CancelBuilding()
    {

    }

    public void EnterRotationMode()
    {

    }

    private enum BuildingState {
        LOCATION_CHOOSE,
        ONGOING_BUILDING,
        CONSTRUCTED
    }
}
