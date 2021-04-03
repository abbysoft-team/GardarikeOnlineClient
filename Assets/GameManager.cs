using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool devMode;

    public GameObject waterlevel;

    private float lastResourceUpdateTime = 0;

    /**
        Modules can set this properties and use them

        This is looses intercomunication between parts of program

        Don't want to user PlayerPrefs because it's persistent
    */
    private static Dictionary<string, object> properties = new Dictionary<string, object>();

    void Start()
    {
        //ConnectToServer(LOGIN, PASSWORD);
        // Give free gold
        //PlayerPrefs.SetInt("Gold", 1000);

        //PlayerPrefs.DeleteAll();
        
        // Reset all counters
        PlayerPrefs.SetInt(GlobalConstants.PEOPLE_COUNT, 0);
        PlayerPrefs.SetInt(GlobalConstants.STONE, 0);
        PlayerPrefs.SetInt(GlobalConstants.FOOD, 0);
        PlayerPrefs.SetInt(GlobalConstants.LEATHER, 0);
        PlayerPrefs.SetInt(GlobalConstants.WOOD, 0);
        PlayerPrefs.SetInt(GlobalConstants.FOOD, 0);

        //EventBus.instance.onBuildingComplete += AddMaxPopulation;

        var ipAddress = devMode ? "127.0.0.1" : Private.ipAddress;
        NetworkManagerFactory.GetManager().Init(ipAddress, Private.requestSockPort, Private.eventSockPort);

        InitProperties();
    }

    private void InitProperties()
    {
        properties.Add(GlobalConstants.WATERLEVEL_PROPERTY, waterlevel.transform.position.y);
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

    /**
        Get some properties, registered by other modules
    */
    public static float GetFloatProperty(string key)
    {
        if (!properties.ContainsKey(key)) throw new NoSuchPropertyException();

        return (float) properties[key];
    }

    /**
        Register some property accessable for any module
    */
    public static void AddProperty(string key, object value)
    {
        properties[key] = value;
    }

    public class NoSuchPropertyException : System.Exception {

    }
}
