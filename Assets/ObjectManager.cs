using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;

    private Dictionary<ObjectType, List<GameObject>> objects;

    private void Awake()
    {
        objects = new Dictionary<ObjectType, List<GameObject>>();
        instance = this;
    }

    public static void RegisterObject(GameObject someObject, ObjectType type)
    {
        var objectsOfThatType = instance.objects[type];
        if (objectsOfThatType == null)
        {
            instance.objects.Add(type, new List<GameObject>());
        }

        instance.objects[type].Add(someObject);
    }

    public static List<GameObject> GetObjects(ObjectType type)
    {
        return null;
        //return instance.objects[type];
    }
}
