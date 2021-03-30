using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodGenerator : MonoBehaviour
{
    void Start()
    {
        //EventBus.instance.onTerrainGenerationFinished += GenerateFoodSources;
    }

    private void GenerateFoodSources(float[,] heights)
    {
        var dimension = (int)Mathf.Sqrt(heights.Length);

        var randomTreeQuantity = Random.Range(GlobalConstants.FOOD_MIN_COUNT, GlobalConstants.FOOD_MAX_COUNT);

        for (int i = 0; i < randomTreeQuantity; i++)
        {
            var x = Random.Range(0, dimension);
            var y = Random.Range(0, dimension);

            var point = Utility.GetGroundedPoint(new Vector3(x, 100, y));
            if (point.y != GlobalConstants.WATER_LEVEL)
            {
                SpawnFoodSource(x, y);
            }
        }
    }

    private void SpawnFoodSource(float x, float y)
    {
        x *= GlobalConstants.CHUNK_SIZE;
        y *= GlobalConstants.CHUNK_SIZE;

        var referenceTree = GetRandomChild();
        var newFood = Instantiate(referenceTree);

        newFood.transform.position = Utility.GetGroundedPoint(new Vector3(x, 100, y));
        newFood.SetActive(true);

        newFood.transform.parent = transform;
    }

    private GameObject GetRandomChild()
    {
        var index = Random.Range(0, transform.childCount);
        foreach (Transform child in transform)
        {
            if (index == 0)
            {
                return child.gameObject;
            }

            index--;
        }

        throw new UnityException("something wrong with the method");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
