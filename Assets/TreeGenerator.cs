using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    private void Start()
    {
        EventBus.instance.onSpawnTreesRequest += GenerateTrees;
    }

    private void GenerateTrees(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var x = Random.Range(0, GlobalConstants.CHUNK_SIZE -5);
            var y = Random.Range(0, GlobalConstants.CHUNK_SIZE -5);

            SpawnTree(x, y);
        }
    }

    private void SpawnTree(float x, float y)
    {
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
