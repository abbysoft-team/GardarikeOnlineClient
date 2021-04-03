using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    private float waterlevel;

    private void Start()
    {
        //EventBus.instance.onSpawnTreesRequest += GenerateTrees;
        EventBus.instance.onClearMapRequest += ResetTrees;

        waterlevel = GameManager.GetFloatProperty(GlobalConstants.WATERLEVEL_PROPERTY);
    }

    private void ResetTrees()
    {
        foreach (Transform tree in transform)
        {
            Destroy(tree.gameObject);
        }
    }

    public void GenerateTrees(int count, int chunkX, int chunkY)
    {
        var startChunkX = chunkX * GlobalConstants.SUBCHUNK_SIZE;
        var startChunkY = chunkY * GlobalConstants.SUBCHUNK_SIZE;

        // * 2 to compensate loss of trees spawned on water
        for (int i = 0; i < count * 2; i++)
        {
            var x = Random.Range(startChunkX, startChunkX + GlobalConstants.CHUNK_SIZE / 3 - 5);
            var y = Random.Range(startChunkY, startChunkY + GlobalConstants.CHUNK_SIZE / 3 - 5);

            if (NotOnWater(x, y)) {
                SpawnTree(x, y);
            }
        }
    }

    private bool NotOnWater(float x, float y) {
        return !Utility.IsOnWater(new Vector3(x, GlobalConstants.CHUNK_HEIGHT, y));
    }

    private void SpawnTree(float x, float y)
    {
        var referenceTree = GetRandomChild();
        var newTree = Instantiate(referenceTree);

        newTree.transform.position = Utility.GetGroundedPoint(new Vector3(x, GlobalConstants.CHUNK_HEIGHT + 200, y));
        newTree.SetActive(true);

        newTree.transform.parent = transform;

        ObjectManager.RegisterObject(newTree, ObjectType.TREE);
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
