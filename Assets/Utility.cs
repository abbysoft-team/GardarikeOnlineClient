using UnityEngine;
using System.Collections.Generic;
using System.Text;

class Utility
{
    public static void AddToIntProperty(string property, int diff)
    {
        var oldValue = PlayerPrefs.GetInt(property);
        PlayerPrefs.SetInt(property, oldValue + diff);
    }

    public static Vector3 GetGroundedPoint(Vector3 point)
    {
        Ray toGround = new Ray(point, new Vector3(0, -1, 0));
        RaycastHit hit = new RaycastHit();
        bool hitOccured = Physics.Raycast(toGround, out hit);

        return hitOccured ? hit.point : point;
    }

    public static string SquashStringList(List<string> strings)
    {
        var builder = new StringBuilder();
        foreach (var stringObject in strings)
        {
            builder.Append(stringObject);
        }

        return builder.ToString().Substring(0, builder.Length - 1);
    }

    public static Vector3 ParseVector3(string vector)
    {
        var values = vector.Substring(1, vector.Length - 2).Split(',');
        var x = float.Parse(values[0].Replace('.', ','));
        var y = float.Parse(values[1].Replace('.', ','));
        var z = float.Parse(values[2].Replace('.', ','));

        return new Vector3(x, y, z);
    }

    public static Quaternion ParseQuaternion(string quaternion)
    {
        var values = quaternion.Substring(1, quaternion.Length - 2).Split(',');
        var x = float.Parse(values[0].Replace('.', ','));
        var y = float.Parse(values[1].Replace('.', ','));
        var z = float.Parse(values[2].Replace('.', ','));
        var w = float.Parse(values[3].Replace('.', ','));

        return new Quaternion(x, y, z, w);
    }

    public static void CloneSaveableComponent(GameObject from, GameObject to)
    {
        //var saveableComponent = from.GetComponent<Saveable>();
        //to.AddComponent(saveableComponent.GetType());
        //var newSaveableComponent = to.GetComponent<Saveable>();
        //newSaveableComponent.fieldsToSave = saveableComponent.fieldsToSave;
        //newSaveableComponent.propertiesToSave = saveableComponent.propertiesToSave;
        //newSaveableComponent.saveableName = saveableComponent.saveableName;
        //newSaveableComponent.component = saveableComponent.component;
    }

    public static string FloatArrayToString(float[,] array)
    {
        var builder = new StringBuilder();
        foreach (var value in array)
        {
            builder.Append(value);
            builder.Append(" ");
        }

        return builder.ToString().Substring(0, builder.Length - 1);
    }

    public static float[,] ParseFloatArray(string floatString)
    {
        var values = floatString.Split(' ');
        var dimension = (int)Mathf.Sqrt(values.Length);
        var array = new float[dimension, dimension];

        for (int i = 0; i < values.Length; i++)
        {
            var j = i / dimension;


            array[j, i % dimension] = (float)System.Convert.ChangeType(values[i], (0.5f).GetType());

        }

        return array;
    }

    public static Vector3 To3rdDimension(Vector2 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }

    public static Vector3 GetPositionOnTheGround(Vector2 pointOnTheScreen)
    {
        var ray = Camera.main.ScreenPointToRay(pointOnTheScreen);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        Debug.LogError("Raycasting position failed");

        return ray.origin;
    }

    public static GameObject GetColliderFromTouch(Vector2 pointOnTheScreen)
    {
        var ray = Camera.main.ScreenPointToRay(pointOnTheScreen);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.collider.name);
            return hit.rigidbody.gameObject;
        }

        Debug.LogError("Raycasting position failed");

        return null;
    }

    public static void SetMaterialForAllChildren(GameObject parent, Material material)
    {
        var childrenMeshes = parent.GetComponentsInChildren<MeshRenderer>(true);
        foreach (var child in childrenMeshes)
        {
            child.material = material;   
        }
    }

    public static Vector3 GetPointOnTheGroundInFrontOfCamera()
    {
        return GetPositionOnTheGround(new Vector2(Screen.width / 2, Screen.height / 2));
    }
}