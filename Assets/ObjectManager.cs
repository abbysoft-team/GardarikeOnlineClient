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
        //Debug.Log("registered " + type);

        if (!instance.objects.ContainsKey(type))
        {
            instance.objects.Add(type, new List<GameObject>());
        }

        var objectsOfThatType = instance.objects[type];

        instance.objects[type].Add(someObject);
    }

    public static List<GameObject> GetObjects(ObjectType type)
    {
        return instance.objects[type];
    }
}
