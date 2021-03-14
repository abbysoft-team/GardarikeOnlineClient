using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BuildingType {
    HOUSE,
    STORAGE,
    QUARRY
}

public class Building : MonoBehaviour
{
    public BuildingType type;

    public Text buildingType;
    public Text empireName;
    public Text effect;

    public GameObject headerGui;

    public void Init()
    {
        empireName.text = PlayerPrefs.GetString(GlobalConstants.COUNTRY_NAME_PROPERTY);

        if (type == BuildingType.HOUSE) {
            buildingType.text = "House";
            effect.text = "+1 food";
        }
        else if (type == BuildingType.QUARRY)
        {
            buildingType.text = "Quarry";
            effect.text = "+5 stone";
        }
        else 
        {
            buildingType.text = "Storage";
            effect.text = "+50 stone/leather/wood capacity";
        }
    }

    public void Select()
    {
        headerGui.SetActive(true);
    }

    public void Unselect()
    {
        headerGui.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
