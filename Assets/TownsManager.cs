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
            InitTown(town);
        }

        Debug.Log("All towns initialized");
    }

    private void InitTown(Gardarike.Town town)
    {
        var townObject = Instantiate(referenceTown);
        townObject.transform.position = Utility.GetGroundedPoint(new Vector3(town.X, 5000,town.Y));
        ConfigureTownComponent(townObject, town);
        townObject.transform.parent = this.transform;
        townObject.SetActive(true);
    }

    private void ConfigureTownComponent(GameObject townObject, Gardarike.Town townParameters)
    {
        var component = townObject.AddComponent<Town>();
        component.empireName.text = townParameters.OwnerName;
        component.name.text = townParameters.Name;
        component.population.text = "" + townParameters.Population;
    }
}
