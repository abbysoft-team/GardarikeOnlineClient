using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using UnityEngine;

public class LevelInfoManager : MonoBehaviour
{
    public static LevelInfoManager instance;

    public List<Saveable> saveables;
    public List<GameObject> referenceObjects;

    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void AddSaveable(Saveable saveable)
    {
        instance.saveables.Add(saveable);
    }

    public void Save()
    {
        var savingName = "testLevel";

        var startTime = Time.time;

        var saveContent = new List<string>();

        foreach (var saveable in instance.saveables)
        {
            saveContent.Add(saveable.Save());
        }

        PlayerPrefs.SetString(GlobalConstants.SAVE_PREFIX + savingName, Utility.SquashStringList(saveContent));

        Debug.Log("Saved game " + savingName + " in " + (Time.time - startTime) * 1000 + " millis");
        Debug.Log("Content: " + Utility.SquashStringList(saveContent));
    }

    public void Load()
    {
        Debug.Log("Start loading map info");

        //NetworkManagerFactory.GetManager().ConnectToServer("some", "pass");

        //var propertyName = GlobalConstants.SAVE_PREFIX + "testLevel";
        //var savingContent = PlayerPrefs.GetString(propertyName);

        //var rootSaveables = savingContent.Split('@');
        //RestoreAllRootSaveables(rootSaveables);

        // Custom actions
        //TerrainGenerator.instance.UpdateTerrain();

        Debug.Log("Loaded succesfully");
    }

    private void RestoreAllRootSaveables(string[] rootSaveables)
    {
        foreach (string saveable in rootSaveables)
        {
            if (saveable == "") continue;
            RestoreRootSaveable(saveable);
        }
    }

    private void RestoreRootSaveable(string saveableContent)
    {
        var headerAndContent = saveableContent.Split('=');
        var rootComponentName = headerAndContent[0];
        var savedInfo = headerAndContent[1].Substring(2);

        var saveable = FindSaveableForComponent(rootComponentName);
        RestoreSaveable(null, saveable.gameObject, savedInfo);
    }

    private Saveable FindSaveableForComponent(string name)
    {
        foreach (var saveable in instance.saveables)
        {
            if (saveable.saveableName == name)
            {
                return saveable;
            }
        }

        return null;
    }

    private GameObject FindReferenceObjectForComponent(string name)
    {
        foreach (var reference in instance.referenceObjects)
        {
            if (reference.name == GlobalConstants.REFERENCE_TAG + name)
            {
                return reference;
            }
        }

        return null;
    }

    private void RestoreSaveable(GameObject parent, GameObject savedObject, string saveableContent)
    {
        var rootAndChildrenInfo = saveableContent.Split('[');
        var rootInfo = rootAndChildrenInfo[0];

        if (savedObject != null)
        {
            SetSaveableInfo(savedObject, rootInfo);
        } 
        else
        {
            CreateObjectBasedOnSavedInfo(parent, rootInfo);
        }

        for (int i = 1; i < rootAndChildrenInfo.Length; i++)
        {
            RestoreSaveable(savedObject, null, rootAndChildrenInfo[i]);
        }
    }

    private void SetSaveableInfo(GameObject restoredObject, string info)
    {
        var typeAndValues = info.Split(';');
        var type = typeAndValues[0];

        var position = Utility.ParseVector3(typeAndValues[1]);
        var rotation = Utility.ParseQuaternion(typeAndValues[2]);
        var fields = typeAndValues[3];
        var properties = typeAndValues[4];

        restoredObject.transform.position = position;
        restoredObject.transform.rotation = rotation;
        
        if (fields != "")
        {
            RestoreFields(restoredObject, type, fields);
        }
        if (properties != "]" && properties != "")
        {
            RestoreProperties(properties);
        }
    }

    private void RestoreFields(GameObject saveable, string type, string fieldsContent)
    {
        var component = saveable.GetComponent(type);
        var declaredFields = component.GetType().GetTypeInfo().DeclaredFields;

        var fields = fieldsContent.Substring(0, fieldsContent.Length - 2).Split('~');
        foreach (var field in fields)
        {
            var keyAndValue = field.Split('|');
            var key = keyAndValue[0];
            var valueString = keyAndValue[1];

            if (valueString == "") continue;

            var declaredField = FindField(declaredFields, key);
            var fieldType = declaredField.FieldType;

            var refFloatArray = new float[2, 2];
            object value;
            if (fieldType == refFloatArray.GetType())
            {
                value = Utility.ParseFloatArray(valueString);
            }
            else
            {
                value = System.Convert.ChangeType(valueString, fieldType);
            }

            declaredField.SetValue(component, value);
        }
    }

    private FieldInfo FindField(IEnumerable<FieldInfo> fields, string key)
    {
        foreach (var field in fields)
        {
            if (field.Name == key) return field;
        }

        return null;
    }

    private void RestoreProperties(string properties)
    {
        var propertiesSplit = properties.Substring(0, properties.Length - 2).Split('~');
        foreach (var property in propertiesSplit)
        {
            var valueAndKey = property.Split('|');
            var key = valueAndKey[0];
            var value = valueAndKey[1];
            PlayerPrefs.SetInt(key, int.Parse(value));
        }
    }

    private void CreateObjectBasedOnSavedInfo(GameObject parent, string info)
    {
        var typeName = info.Split(';')[0];

        var reference = FindReferenceObjectForComponent(typeName);
        if (reference == null) reference = new GameObject();

        GameObject instance = Instantiate(reference);
        instance.transform.parent = parent.transform;
        instance.SetActive(true);  

        SetSaveableInfo(instance, info);
    }

    public void ReloadMap()
    {
        // TerrainGenerator.instance.ClearActiveChunks();
        
        // for (int i = -1; i < 1; i++)
		// {
		// 	for (int j = -1; j < 1; j++)
		// 	{
        //         EventBus.instance.LoadMap(i, j);
		// 	}
		// }
    }
}
