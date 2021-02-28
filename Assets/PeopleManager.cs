using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Gardarike;

public class PeopleManager : MonoBehaviour
{
    private GameObject referenceMan;

    void Start() 
    {
        referenceMan = transform.GetChild(0).gameObject;

       // EventBus.instance.onMapReady += SpawnAfterLogin;
        //EventBus.instance.onPeopleCountIncreased += SpawnPeople;
    }

    private void SpawnAfterLogin()
    {
        var peopleCount = PlayerPrefs.GetInt("Population");
        SpawnPeople(peopleCount);
    }

    private void SpawnNewPeople(Character character)
    {
        var newPeople = (int) character.CurrentPopulation - PlayerPrefs.GetInt("Population");
        if (newPeople > 0)
        {
            SpawnPeople(newPeople);
        }
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
