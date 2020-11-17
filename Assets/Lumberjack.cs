using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Lumberjack : Role
{
    private Tree currentTree;
    private State state;

    public void Init(PeopleController man)
    {
        currentTree = FindTree(man.transform.position);
        if (currentTree == null)
        {
            //Debug.Log("No trees near me");
            man.SetRandomAction();
            return;
        }

        currentTree.RegisterWorker();
        man.GoTo(currentTree.transform.position);
        state = State.GO_TO_JOB;
    }

    private Tree FindTree(Vector3 manPosition)
    {
        var trees = ObjectManager.GetObjects(ObjectType.TREE);
        
        var nearestTree = trees[0];
        var minDistance = Vector3.Distance(nearestTree.transform.position, manPosition);
        foreach (var tree in trees)
        {
            var treeComponent = tree.GetComponent<Tree>();
            if (!treeComponent.CanBeMoreWorkers()) continue;

            var distance = Vector3.Distance(tree.transform.position, manPosition);
            if (distance < minDistance)
            {
                nearestTree = tree;
                minDistance = distance;
            }
        }

        return nearestTree.GetComponent<Tree>();
    }

    public void ActionAccomplished(PeopleController man)
    {
        if (state == State.GO_TO_JOB)
        {
            //Debug.Log("LUMBER: start cutting a tree");
            // working
            man.Idle(20);
            man.LoopSound("cutTree");
            state = State.WORKING;
            return;
        }
        else if (state == State.WORKING)
        {
            //Debug.Log("LUMBER: done with the tree, obtain 10 wood");
            // increase wood
            Utility.AddToIntProperty("Wood", 10);
            man.PlayOneShot("breakTree");
            ObjectManager.UnregisterObject(currentTree.gameObject, ObjectType.TREE);
            currentTree.gameObject.SetActive(false);
            
            Init(man);
        }
    }

    private enum State
    {
        GO_TO_JOB,
        WORKING,
        GO_HOME
    }
}