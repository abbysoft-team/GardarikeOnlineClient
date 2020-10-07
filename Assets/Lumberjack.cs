using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Lumberjack : Role
{
    private Tree currentTree;

    public void Init(PeopleController man)
    {
        currentTree = FindTree(man.transform.position);
        man.GoTo(currentTree.transform.position);
    }

    private Tree FindTree(Vector3 manPosition)
    {
        var trees = ObjectManager.GetObjects(ObjectType.TREE);
        
        var nearestTree = trees[0];
        var minDistance = Vector3.Distance(nearestTree.transform.position, manPosition);
        foreach (var tree in trees)
        {
            var distance = Vector3.Distance(tree.transform.position, manPosition);
            if (distance < minDistance)
            {
                nearestTree = tree;
                minDistance = distance;
            }
        }

        return nearestTree.GetComponent<Tree>();
    }

    public void Update(PeopleController man)
    {
    }
}