using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;

using Gardarike;

public class TownsManager : MonoBehaviour
{
    public GameObject referenceTown;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitTowns(RepeatedField<Gardarike.Town> towns)
    {
        Debug.Log("Initializing " + towns.Count + " towns...");

        foreach (var town in towns)
        {
            InitTown(town, new Quaternion());
        }

        Debug.Log("All towns initialized");
    }

    private void InitTown(Gardarike.Town town, Quaternion rotation)
    {
        var townObject = Instantiate(referenceTown);
        townObject.transform.position = Utility.GetGroundedPoint(new Vector3(town.X, 5000,town.Y));
        ConfigureTownComponent(townObject, town);
        townObject.transform.parent = this.transform;
        townObject.transform.rotation = rotation;
        townObject.SetActive(true);
    }

    private void ConfigureTownComponent(GameObject townObject, Gardarike.Town townParameters)
    {
        //var component = townObject.AddComponent<Town>();
        var component = townObject.GetComponent<Town>();

        component.empireName.text = townParameters.OwnerName;
        component.name.text = townParameters.Name;
        component.population.text = "" + townParameters.Population;

        component.headerUI.SetActive(true);
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

        var vector = new Vector2D();
        vector.X = newTown.X;
        vector.Y = newTown.Y;

        EventBus.instance.SendNewTownRequest(vector, newTown.Name);
    }
}
