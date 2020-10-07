using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //var buildings = FindAllBuildings();
    }

    private List<GameObject> FindAllBuildings()
    {
        List<GameObject> children = new List<GameObject>(transform.parent.childCount);
        foreach (Transform transform in transform.parent)
        {
            if (transform.tag == GlobalConstants.BUILDING_TAG)
            {
                children.Add(transform.gameObject);
            }
        }

        return children;
    }
}
