using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gardarike;

public class BuildItem : MonoBehaviour
{
    public int priceGold;
    public GameObject model;
    public UnityEngine.Vector3 position;
    public UnityEngine.Vector3 lossyScale;

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

        if (model.activeSelf && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            model.transform.position = Utility.GetPositionOnTheGround(Input.GetTouch(0).position);
        }

        checkBuildingPlaceChosen();
    }

    private void checkBuildingPlaceChosen()
    {
        var buildPlaceChosen = model.activeSelf && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;
        if (buildPlaceChosen)
        {
            position = model.transform.position;
            lossyScale = model.transform.lossyScale;
            EventBus.instance.BuildingComplete(this);
            EventBus.instance.RegisterBuilding(this);
        }

    }

    public void StartBuilding()
    {
        Debug.Log("Start building");
        Utility.AddToIntProperty("Gold", priceGold * -1);
        model.SetActive(true);
    }

    public static BuildItem FromProtoBuilding(Vector3D location) {
        var item = new BuildItem();
        item.lossyScale = new UnityEngine.Vector3(1.33f, 1.33f, 1.33f);
        item.position = ProtoConverter.ToUnityVector(location);

        return item;
    }

    public Vector3D Location() {
        return new Vector3D {X = model.transform.position.x, Y = model.transform.position.y, Z = model.transform.position.z};
    }
}
