using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using System;

using Gardarike;

public class SceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake() 
    {
        EventBus.instance.onMapObjectsLoadingComplete += MapLoadingComplete;
    }

    private void MapLoadingComplete(RepeatedField<Building> buildings, int treeCount) {
        ShowTutorialIfNeed();
    }

    private void ShowTutorialIfNeed() {
        if (PlayerPrefs.GetInt(GlobalConstants.TUTORIAL_COMPLETE_PROPERTY) == 2) {
            return;
        }

        Debug.Log("Showing tutorial message");
        // TODO extract messages to Strings.cs
        EventBus.instance.ShowInputDialog(Strings.TUTORIAL_TITLE, Strings.TUTORIAL_MESSAGE, GlobalConstants.COUNTRY_NAME_PROPERTY);
        EventBus.instance.ShowInfo("Info", "New empire " + PlayerPrefs.GetString(GlobalConstants.COUNTRY_NAME_PROPERTY) + " now exist on the world map!");
        EventBus.instance.ShowInputDialog("Capital", "Some of the first men of " + PlayerPrefs.GetString(GlobalConstants.COUNTRY_NAME_PROPERTY) + " found first town. Choose the name for your capital.", GlobalConstants.CAPITAL_NAME_PROPERTY);
        
        //EventBus.instance.SendChatMessage("New empire " + PlayerPrefs.GetString(GlobalConstants.COUNTRY_NAME_PROPERTY) + " now exists");
        //EventBus.instance.SendChatMessage(PlayerPrefs.GetString(GlobalConstants.CAPITAL_NAME_PROPERTY) + " is now the capital of " + GlobalConstants.COUNTRY_NAME_PROPERTY);

        PlayerPrefs.SetInt(GlobalConstants.TUTORIAL_COMPLETE_PROPERTY, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
