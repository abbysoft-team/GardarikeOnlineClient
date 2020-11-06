using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleManager : MonoBehaviour
{
    private GameObject referenceMan;

    void Start() 
    {
        referenceMan = transform.GetChild(0).gameObject;

        EventBus.instance.onMapReady += SpawnAfterLogin;
        EventBus.instance.onPeopleCountIncreased += SpawnPeople;
    }

    private void SpawnAfterLogin()
    {
        //var peopleCount = PlayerPrefs.GetInt("Population");
        SpawnPeople(1);
    }

    private void SpawnPeople(int count)
    {
        Debug.Log("Spawning " + count + " mans");

        for (int i = 0; i < count; i++)
        {
            InstanciateMan();
        }
    }

    private void InstanciateMan()
    {
        var newMan = Instantiate(referenceMan);
        newMan.AddComponent<PeopleController>();
        newMan.transform.parent = transform;
        newMan.SetActive(true);
    }
}
