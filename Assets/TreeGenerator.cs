using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{

    private void GenerateTrees(float[,] heights)
    {
        var dimension = (int)Mathf.Sqrt(heights.Length);
        var x = Random.Range(0, dimension);
        var y = Random.Range(0, dimension);
        var randomHeight = heights[x, y];
        var randomDelta = GlobalConstants.TREE_GROW_DELTA * randomHeight;

        for (int i = 0; i < dimension; i++) {
            for (int j = 0; j < dimension; j++)
            {
                var height = heights[i, j];
                if (Mathf.Abs(height - randomHeight) <= randomDelta)
                {
                    SpawnTree(i, j);
                }
            }
        }
    }

    private void GenerateTreesNew(float[,] heights)
    {
        if (true) return;

        var dimension = (int)Mathf.Sqrt(heights.Length);

        var randomTreeQuantity = Random.Range(GlobalConstants.TREE_MIN_COUNT, GlobalConstants.TREE_MAX_COUNT);

        for (int i = 0; i < randomTreeQuantity; i++)
        {
            var x = Random.Range(0, dimension);
            var y = Random.Range(0, dimension);

            var point = Utility.GetGroundedPoint(new Vector3(x, 100, y));
            if (point.y != GlobalConstants.WATER_LEVEL)
            {
                SpawnTree(x, y);
            }
        }
    }

    private void SpawnTree(float x, float y)
    {
        x *= GlobalConstants.CHUNK_SIZE;
        y *= GlobalConstants.CHUNK_SIZE;

        var referenceTree = GetRandomChild();
        var newTree = Instantiate(referenceTree);

        newTree.transform.position = Utility.GetGroundedPoint(new Vector3(x, 100, y));
        newTree.SetActive(true);

        newTree.transform.parent = transform;

        //ObjectManager.RegisterObject(newTree, typeof(Tree));
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

    private void Start()
    {
        EventBus.instance.onTerrainGenerationFinished += GenerateTreesNew;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
