using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    private void Start()
    {
        EventBus.instance.onSpawnTreesRequest += GenerateTrees;
    }

    public void GenerateTrees(int count)
    {
        // * 2 to compensate loss of trees spawned on water
        for (int i = 0; i < count * 2; i++)
        {
            var x = Random.Range(0, GlobalConstants.CHUNK_SIZE -5);
            var y = Random.Range(0, GlobalConstants.CHUNK_SIZE -5);

            if (NotOnWater(x, y)) {
                SpawnTree(x, y);
            }
        }
    }

    private bool NotOnWater(float x, float y) {
        Ray toGround = new Ray(new Vector3(x, 10000, y), new Vector3(0, -1, 0));
        RaycastHit hit = new RaycastHit();
        bool hitOccured = Physics.Raycast(toGround, out hit);

        if (hitOccured && hit.collider.name == "Waterlevel") {
            return false;
        }

        return true;
    }

    private void SpawnTree(float x, float y)
    {
        var referenceTree = GetRandomChild();
        var newTree = Instantiate(referenceTree);

        newTree.transform.position = Utility.GetGroundedPoint(new Vector3(x, 5000, y));
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
