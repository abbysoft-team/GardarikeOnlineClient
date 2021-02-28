using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public int woodCount;
    private int workers;

    private void Awake()
    {
        workers = 0;
        woodCount = Random.Range(GlobalConstants.MIN_WOOD_PER_TREE, GlobalConstants.MAX_WOOD_PER_TREE);
    }

    public void RegisterWorker()
    {
        workers++;
    }

    public int GetWorkerCount()
    {
        return workers;
    }

    public bool CanBeMoreWorkers()
    {
        return workers < 3;
    }
}
