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
        if (PlayerPrefs.GetInt("tutorialComplete") == 1) {
            return;
        }

        EventBus.instance.ShowInputDialog(Strings.TUTORIAL_TITLE, Strings.TUTORIAL_MESSAGE, Strings.ENTER_THE_NAME);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
