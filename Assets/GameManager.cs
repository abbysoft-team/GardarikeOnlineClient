using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool devMode;

    private float lastResourceUpdateTime = 0;

    void Start()
    {
        //ConnectToServer(LOGIN, PASSWORD);
        // Give free gold
        //PlayerPrefs.SetInt("Gold", 1000);

        //PlayerPrefs.DeleteAll();
        
        PlayerPrefs.SetInt("Population", 0);

        //EventBus.instance.onBuildingComplete += AddMaxPopulation;

        var ipAddress = devMode ? "127.0.0.1" : Private.ipAddress;
        NetworkManagerFactory.GetManager().Init(ipAddress, Private.requestSockPort, Private.eventSockPort);
    }

    private void AddMaxPopulation(BuildItem item)
    {
        Utility.AddToIntProperty(GlobalConstants.POPULATION_MAX_COUNT, GlobalConstants.POPULATION_PER_HOME);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.instance.currentScene != GameScene.GAME)
        {
            return;
        }

        if (Time.time - lastResourceUpdateTime > GlobalConstants.RESOURCE_UPDATE_DELAY)
        {
            lastResourceUpdateTime = Time.time;

            // update resources
            EventBus.instance.SendResourceUpdateRequest();
        }
        
    }
}
