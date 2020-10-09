using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        //ConnectToServer(LOGIN, PASSWORD);
        // Give free gold
        //PlayerPrefs.SetInt("Gold", 1000);
        PlayerPrefs.SetInt("Population", 0);

        var array = new float[2, 2];
        array[0, 0] = 0;
        array[0, 1] = 1;
        array[1, 0] = 10;
        array[1, 1] = 11;

        Debug.Log(Utility.FloatArrayToString(array));

        EventBus.instance.onBuildingComplete += AddMaxPopulation;

        NetworkManagerFactory.GetManager().Init("25.83.171.25", 27013);
        NetworkManagerFactory.GetManager().ConnectToServer("some", "pass");
    }

    private void AddMaxPopulation(BuildItem item)
    {
        Utility.AddToIntProperty(GlobalConstants.POPULATION_MAX_COUNT, GlobalConstants.POPULATION_PER_HOME);
    }

    // Update is called once per frame
    void Update()
    {
        Utility.AddToIntProperty(GlobalConstants.GAME_MILLIS, (int) (Time.deltaTime * 1000));
    }
}
