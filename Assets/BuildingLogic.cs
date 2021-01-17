using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingLogic : MonoBehaviour
{
    private const float UI_PROTOTYPE_HEIGHT = 620.0f;
    private BuildingState state;
    public Material prototypeMaterial;
    public Material invalidMaterial;
    private GameObject building;
    public GameObject prototypingUI;
    public Button applyButton;
    public Button rotationButton;
    private int callbackId;
    private bool rotationMode;

    void Start()
    {
        EventBus.instance.onChooseLocationForBuilding += ChooseBuildingLocation;
        prototypingUI.SetActive(false);
        state = BuildingState.READY_FOR_BUILDING;
    }

    public void ChooseBuildingLocation(int eventId, GameObject referenceBuilding)
    {
        Debug.Log("start building");
        
        rotationMode = false;
        callbackId = eventId;
        state = BuildingState.LOCATION_CHOOSE;

        building = Instantiate(referenceBuilding);
        Utility.SetMaterialForAllChildren(building, prototypeMaterial);
        building.transform.position = Utility.GetPointOnTheGroundInFrontOfCamera();
        building.SetActive(true);

        ConfigurePrototypingUI();
        CheckPlacementRestrictions(building);
    }

    private void ConfigurePrototypingUI()
    {
        StickUIToPrototype();

        prototypingUI.SetActive(true);
    }

    void Update()
    {
        if (state != BuildingState.LOCATION_CHOOSE) return;
        
        var touchHappen = Input.touchCount == 1 &&
        Input.GetTouch(0).phase == TouchPhase.Stationary &&
        !ScrollAndPitch.IsClickedOnSomeWorldspaceUI();

        if (Input.touchCount == 2 && rotationMode)
        {
            var rotationDegrees = ScrollAndPitch.GetRotationDegrees();
            building.transform.RotateAround(building.transform.position, new Vector3(0, -1, 0), rotationDegrees);
        } else if (!rotationMode && touchHappen) {
            building.transform.position = Utility.GetPositionOnTheGround(Input.GetTouch(0).position);

            CheckPlacementRestrictions(building);
        }

        StickUIToPrototype();
    }

    private void CheckPlacementRestrictions(GameObject buildingPrototype)
    {
        // in further releases, must check if building allowed to be built on the water
        var invalid = Utility.IsOnWater(buildingPrototype);
        if (!invalid) {
            Utility.SetMaterialForAllChildren(buildingPrototype, prototypeMaterial);
            applyButton.enabled = true;
            return;
        }
        // Invalid place
        Utility.SetMaterialForAllChildren(buildingPrototype, invalidMaterial);
        applyButton.enabled = false;
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

    public void ToggleRotationMode()
    {
        rotationMode = !rotationMode;

        ScrollAndPitch.instance.Rotate = !rotationMode;
        if (rotationMode)
        {
            rotationButton.GetComponent<Image>().color = Color.green;
        } else 
        {
            rotationButton.GetComponent<Image>().color = Color.white;
        }
    }

    private enum BuildingState {
        READY_FOR_BUILDING,
        LOCATION_CHOOSE,
        ONGOING_BUILDING,
        CONSTRUCTED
    }
}
