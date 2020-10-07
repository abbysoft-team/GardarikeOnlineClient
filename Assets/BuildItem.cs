using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildItem : MonoBehaviour
{
    public int priceGold;
    public GameObject model;

    void Start()
    {
        DisableIfNotEnoughMaterials();
    }

    private void DisableIfNotEnoughMaterials()
    {
        if (enabled && PlayerPrefs.GetInt("Gold") < priceGold)
        {
            enabled = false;
            var button = GetComponent<Button>().interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        DisableIfNotEnoughMaterials();

        if (model.activeSelf)
        {
            model.transform.position = Utility.GetPositionOnTheGround(Input.mousePosition);
        }

        checkBuildingPlaceChosen();
    }

    private void checkBuildingPlaceChosen()
    {
        var buildPlaceChosen = model.activeSelf && Input.GetMouseButton(0);
        if (buildPlaceChosen)
        {
            EventBus.instance.buildingComplete(this);
        }
    }

    public void StartBuilding()
    {
        Utility.AddToIntProperty("Gold", priceGold * -1);
        model.SetActive(true);
    }
}
