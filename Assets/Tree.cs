using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public int woodCount;

    private void Awake()
    {
        woodCount = Random.Range(GlobalConstants.MIN_WOOD_PER_TREE, GlobalConstants.MAX_WOOD_PER_TREE);
    }
}
