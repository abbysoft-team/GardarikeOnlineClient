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
        //PlayerPrefs.SetInt("Population", 0);

        //EventBus.instance.onBuildingComplete += AddMaxPopulation;

        NetworkManagerFactory.GetManager().Init("89.108.99.2", 27015);
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
