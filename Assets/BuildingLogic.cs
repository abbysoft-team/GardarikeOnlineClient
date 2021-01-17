using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLogic : MonoBehaviour
{
    private const float UI_PROTOTYPE_HEIGHT = 620.0f;
    private BuildingState state;
    public Material prototypeMaterial;
    private GameObject building;
    public GameObject prototypingUI;
    private int callbackId;

    void Start()
    {
        EventBus.instance.onChooseLocationForBuilding += ChooseBuildingLocation;
        prototypingUI.SetActive(false);
        state = BuildingState.READY_FOR_BUILDING;
    }

    public void ChooseBuildingLocation(int eventId, GameObject referenceBuilding)
    {
        Debug.Log("start building");
        
        callbackId = eventId;
        state = BuildingState.LOCATION_CHOOSE;

        building = Instantiate(referenceBuilding);
        Utility.SetMaterialForAllChildren(building, prototypeMaterial);
        building.transform.position = Utility.GetPointOnTheGroundInFrontOfCamera();
        building.SetActive(true);

        ConfigurePrototypingUI();
    }

    private void ConfigurePrototypingUI()
    {
        StickUIToPrototype();

        prototypingUI.SetActive(true);
    }

    void Update()
    {
        if (state != BuildingState.LOCATION_CHOOSE) return;
        
        var touchHappen = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Stationary;

        touchHappen = Input.GetMouseButton(0) && !ScrollAndPitch.IsClickedOnSomeWorldspaceUI();
        if (!touchHappen) return;

        building.transform.position = Utility.GetPositionOnTheGround(Input.mousePosition);
        StickUIToPrototype();
    }

    private void StickUIToPrototype()
    {
        var position = building.transform.position;

        prototypingUI.transform.position = new Vector3(position.x, position.y + UI_PROTOTYPE_HEIGHT, position.z);
    }

    public void ApplyBuilding()
    {
        building.SetActive(false);
        prototypingUI.SetActive(false);
        EventBus.instance.NotifyEventFinished(callbackId, building.transform.position);
    }

    public void CancelBuilding()
    {
        Debug.Log("Building canceled");
        this.state = BuildingState.READY_FOR_BUILDING;
        building.SetActive(false);
        prototypingUI.SetActive(false);
    }

    public void EnterRotationMode()
    {

    }

    private enum BuildingState {
        READY_FOR_BUILDING,
        LOCATION_CHOOSE,
        ONGOING_BUILDING,
        CONSTRUCTED
    }
}
