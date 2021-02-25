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

    // Start is called before the first frame update
    void Start()
    {
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
            Destroy(town.gameObject);
        }
    }

    private void CheckTownClicked(GameObject collider)
    {
        if (collider.name != "TownContext" && selectedTown != null) selectedTown.ResetSelection();
        
        if (collider.tag != "TouchDetector") return;

        selectedTown = collider.GetComponentInParent<Town>();
        selectedTown.WasClicked();
    }

    public void InitTowns(RepeatedField<Gardarike.Town> towns)
    {
        Debug.Log("Initializing " + towns.Count + " towns...");

        foreach (var town in towns)
        {
            Debug.Log(town);
            InitTown(town, new Quaternion());
        }

        Debug.Log("All towns initialized");
    }

    private void InitTown(Gardarike.Town town, Quaternion rotation)
    {
        var townObject = Instantiate(referenceTown);
        townObject.transform.position = Utility.FromServerCoords(town.X, town.Y);
        var townComponent = ConfigureTownComponent(townObject, town);
        townObject.transform.parent = this.transform;
        townObject.transform.rotation = rotation;
        townObject.SetActive(true);

        towns.Add(townComponent);
    }

    private Town ConfigureTownComponent(GameObject townObject, Gardarike.Town townParameters)
    {
        //var component = townObject.AddComponent<Town>();
        var component = townObject.GetComponent<Town>();
        component.Init(townParameters);

        return component;
    }

    public void InitiateTownBuilding()
    {
        var actionId = EventBus.instance.ChooseLocationForBuilding(referenceTown);
        EventBus.instance.onEventFinished += (id, location) => {
            if (id == actionId) BuildTown((Transform) location);
        };
    }

    private void BuildTown(Transform transform)
    {
        var newTown = new Gardarike.Town();
        newTown.OwnerName = PlayerPrefs.GetString(GlobalConstants.COUNTRY_NAME_PROPERTY);
        newTown.Name = "New town " + Random.Range(0, 999);
        newTown.Population = 0;
        newTown.X = (long) transform.position.x;
        newTown.Y = (long) transform.position.z;

        InitTown(newTown, transform.localRotation);

        EventBus.instance.SendNewTownRequest(Utility.ToServerCoordinates(transform.position), newTown.Name);
    }
}
