using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomPanelLogic : MonoBehaviour
{

    public Button buildingOption;

    void Start()
    {
        EventBus.instance.onBuildingStarted += (id, someObj) => buildingOption.gameObject.SetActive(false);
        EventBus.instance.onBulidingComplete += (result) =>  buildingOption.gameObject.SetActive(true);
    }

    void Update()
    {
        
    }
}
