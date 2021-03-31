using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;

using Gardarike;

public class TownsManager : MonoBehaviour
{
    public GameObject referenceTown;

    private Town selectedTown;

    private HashSet<Town> towns = new HashSet<Town>();

    public static TownsManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        EventBus.instance.onClickWasMade += CheckTownClicked;
        EventBus.instance.onClearMapRequest += ClearTowns;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ClearTowns()
    {
        foreach (Transform town in transform)
        {
            town.gameObject.SetActive(false);
        }
    }

    public void RestoreTowns()
    {
        foreach (Transform town in transform)
        {
            town.gameObject.SetActive(true);
        }
    }

    private void CheckTownClicked(GameObject collider)
    {
        if (collider == null) return;

        if (collider.name != "TownContext" && selectedTown != null) {
            selectedTown.ResetSelection();
        }
        
        if (collider.tag != "TouchDetector") return;

        selectedTown = collider.GetComponentInParent<Town>();
        selectedTown.WasClicked();
    }

    public void RegisterServerTowns(RepeatedField<Gardarike.Town> towns, int chunkX, int chunkY)
    {
        Debug.Log("Initializing " + towns.Count + " towns...");

        foreach (var town in towns)
        {
            RegisterServerTown(town, chunkX, chunkY);
        }

        Debug.Log("All towns initialized");
    }

    public GameObject RegisterServerTown(Gardarike.Town town, int chunkX, int chunkY)
    {
        Debug.Log(town);
        var gameCoords = Utility.FromServerCoords(town.X, town.Y, chunkX, chunkY);
        town.X = (long) gameCoords.x;
        town.Y = (long) gameCoords.z;
        return InitTown(town, new Quaternion());
    }

    private GameObject InitTown(Gardarike.Town town, Quaternion rotation)
    {
        var townObject = Instantiate(referenceTown);
        townObject.transform.position = Utility.GetGroundedPointForBuildings(town.X, town.Y);
        var townComponent = ConfigureTownComponent(townObject, town, rotation);
        townObject.transform.parent = this.transform;
        townObject.SetActive(true);

        towns.Add(townComponent);

        return townObject;
    }

    private Town ConfigureTownComponent(GameObject townObject, Gardarike.Town townParameters, Quaternion rotation)
    {
        //var component = townObject.AddComponent<Town>();
        var component = townObject.GetComponent<Town>();
        component.Init(townParameters);
        component.Rotate(rotation);

        return component;
    }

    public void InitiateTownBuilding()
    {
        var actionId = EventBus.instance.ChooseLocationForBuilding(referenceTown);
        EventBus.instance.onEventFinished += (id, location) => {
            if (id == actionId) BuildTown((Transform) location);
        };
    }

    public void BuildTown(Transform transform)
    {
        var newTown = new Gardarike.Town();
        newTown.OwnerName = PlayerPrefs.GetString(GlobalConstants.COUNTRY_NAME_PROPERTY);
        newTown.Name = "New town " + Random.Range(0, 999);
        newTown.Population = 0;
        newTown.X = (long) transform.position.x;
        newTown.Y = (long) transform.position.z;

        InitTown(newTown, transform.localRotation);

        EventBus.instance.SendNewTownRequest(Utility.ToServerCoordinates(transform.position), newTown.Name, transform.rotation.y);
    }
}
