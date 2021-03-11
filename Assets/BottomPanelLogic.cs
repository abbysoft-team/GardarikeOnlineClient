using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomPanelLogic : MonoBehaviour
{

    public Button buildingOption;

    void Start()
    {
        EventBus.instance.onBuildingInitiated += (arg) => buildingOption.gameObject.SetActive(false);
        EventBus.instance.onBuildingProcessFinished += () =>  buildingOption.gameObject.SetActive(true);
    }

    void Update()
    {
        
    }
}
