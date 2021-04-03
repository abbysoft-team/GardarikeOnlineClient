using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingLogic : MonoBehaviour
{
    private const float UI_PROTOTYPE_HEIGHT = 5.0f;
    private BuildingState state;
    public Material prototypeMaterial;
    public Material invalidMaterial;
    private GameObject building;
    public GameObject prototypingUI;
    public Text buildingType;
    public Button applyButton;
    public Button rotationButton;
    private bool rotationMode;
    
    private string buildingName;

    private InputManager inputManager;

    void Start()
    {
        EventBus.instance.onBuildingInitiated += StartBuildingProcess;
        prototypingUI.SetActive(false);
        state = BuildingState.READY_FOR_BUILDING;

        inputManager = InputManagerFatory.GetDefaultManager();
    }

    private void StartBuildingProcess(object building)
    {
        Debug.Log("Start building " + building);
        buildingName = (string) building;
        var referenceBuilding = ResourceManager.GetReferenceObject(buildingName);

        buildingType.text = "New " + buildingName + " construction";

        ChooseBuildingLocation(referenceBuilding);
    }

    public void ChooseBuildingLocation(GameObject referenceBuilding)
    {
        Debug.Log("start building");
        
        rotationMode = false;
        state = BuildingState.LOCATION_CHOOSE;

        building = Instantiate(referenceBuilding);
        Utility.SetMaterialForAllChildren(building, prototypeMaterial);
        building.transform.position = Utility.GetPointOnTheGroundInFrontOfCamera();
        building.transform.position = Utility.GetGroundedPointForBuildings((long) building.transform.position.x, (long) building.transform.position.z);
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
        !inputManager.IsClickedOnSomeWorldspaceUI();

        if (Input.touchCount == 2 && rotationMode)
        {
            var rotationDegrees = inputManager.GetRotationDegrees();
            building.transform.RotateAround(building.transform.position, new Vector3(0, -1, 0), rotationDegrees);
        } else if (!rotationMode && touchHappen) {
            var hit = Utility.GetHitOnTheGround(Input.GetTouch(0).position);
            if (!hit.collider.name.StartsWith("Terrain")) {
                return;
            }

            var buildingPosition = new Vector3(hit.point.x, hit.point.y + GlobalConstants.BUILDING_Y_OFFSET, hit.point.z);
            
            building.transform.position = hit.point;
            CheckPlacementRestrictions(building);
        }

        StickUIToPrototype();
    }

    private void CheckPlacementRestrictions(GameObject buildingPrototype)
    {
        // in further releases, must check if building allowed to be built on the water
        var invalid = Utility.IsOnWater(buildingPrototype.transform.position);
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
        var topPoint = Utility.GetGroundedPoint(new Vector3(position.x, position.y + GlobalConstants.CHUNK_HEIGHT, position.z));

        prototypingUI.transform.position = new Vector3(position.x,UI_PROTOTYPE_HEIGHT + topPoint.y, position.z);
    }

    public void ApplyBuilding()
    {
        building.SetActive(false);
        prototypingUI.SetActive(false);

        state = BuildingState.ONGOING_BUILDING;
        EventBus.instance.FireBuildingFinished();

        if (buildingName == "town")
        {
            TownsManager.instance.BuildTown(building.transform);
        }
        else
        {
            BuildingManager.instance.BuildBuilding(buildingName, building.transform);
        }

        Destroy(building);
    }

    public void CancelBuilding()
    {
        Debug.Log("Building canceled");
        this.state = BuildingState.READY_FOR_BUILDING;
        Destroy(building);
        prototypingUI.SetActive(false);

        EventBus.instance.BuildingCanceled();
    }

    public void ToggleRotationMode()
    {
        rotationMode = !rotationMode;

        inputManager.ToggleRotationMode(rotationMode);
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
