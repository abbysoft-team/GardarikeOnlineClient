using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopUILogic : MonoBehaviour
{
    public Text currentTown;
    public GameObject globalViewButton;

    void Start()
    {
        EventBus.instance.onGoToTownView += UpdateTownUI;
        EventBus.instance.onGoToGlobalView += UpdateGlobalUI;
    }

    private void UpdateGlobalUI()
    {
        currentTown.gameObject.SetActive(false);
        globalViewButton.SetActive(false);
    }

    private void UpdateTownUI(Town town)
    {
        currentTown.gameObject.SetActive(true);
        globalViewButton.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
