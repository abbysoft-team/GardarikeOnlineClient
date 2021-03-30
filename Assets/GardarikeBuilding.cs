using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Any buildable object (ex: town, storage, mill, etc)
*/
public class GardarikeBuilding : MonoBehaviour
{
    public GameObject mesh;
    public GameObject headerGui;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Leave gui unrotated
    public void Rotate(Quaternion rotation)
    {
        mesh.transform.rotation = rotation;
    }
}
