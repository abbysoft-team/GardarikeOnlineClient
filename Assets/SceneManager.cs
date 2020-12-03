using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;

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

        EventBus.instance.ShowInputDialog(Strings.TUTORIAL_TITLE, Strings.TUTORIAL_MESSAGE, GlobalConstants.COUNTRY_NAME_PROPERTY);

        PlayerPrefs.SetInt(GlobalConstants.TUTORIAL_COMPLETE_PROPERTY, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
