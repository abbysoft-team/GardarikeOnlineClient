using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using UnityEngine;

public class Saveable : MonoBehaviour
{
    public List<string> fieldsToSave;
    public string component;
    public List<string> propertiesToSave;
    public string saveableName;

    public string Save()
    {
        // Saving some root element, need to save all children as well 
        if (saveableName != null && saveableName != "")
        {
            var builder = new StringBuilder();
            builder.Append("@");
            builder.Append(saveableName);
            builder.Append("=[");
            builder.Append(SaveComponentInfo());
            SaveChildComponents(builder);

            return builder.ToString();
        }

        return SaveComponentInfo();
    }

    private string SaveComponentInfo()
    {
        var info = "[" + component + ";" + SavePosition() + ";" + SaveRotation() + ";" + SaveFields() + ";" + SaveProperties() + "]";
        Debug.Log(info);
        return info;
    }

    private string SavePosition()
    {
        return transform.position.ToString();
    }

    private string SaveRotation()
    {
        return transform.rotation.ToString();
    }

    private string SaveFields()
    {
        var builder = new StringBuilder();
        foreach (var field in fieldsToSave)
        {
            builder.Append(field);
            builder.Append("|");
            builder.Append(SaveField(field));
            builder.Append("~");
        }

        return builder.ToString();
    }

    private string SaveField(string fieldName)
    {
        var componentObject = GetComponent(component);
        var type = componentObject.GetType();
        var fields = type.GetTypeInfo().DeclaredFields;
        var field = FindField(fields, fieldName);

        var refFloatArray = new float[2, 2];
        if (field.FieldType == refFloatArray.GetType())
        {
            return Utility.FloatArrayToString((float[,]) field.GetValue(componentObject));
        }

        return field.GetValue(componentObject).ToString();
    }

    private FieldInfo FindField(IEnumerable<FieldInfo> fields, string fieldName)
    {
        foreach (var field in fields)
        {
            if (field.Name == fieldName)
            {
                return field;
            }
        }

        throw new KeyNotFoundException("Cant find field " + fieldName);
    }

    private string SaveProperties()
    {
        var builder = new StringBuilder();
        foreach (var property in propertiesToSave)
        {
            builder.Append(property);
            builder.Append("|");
            builder.Append(SaveProperty(property));
            builder.Append("~");
        }

        return builder.ToString();
    }

    private string SaveProperty(string property)
    {
        // TODO make able to save not only int props
        return PlayerPrefs.GetInt(property).ToString();
    }

    private void SaveChildComponents(StringBuilder builder)
    {
        var saveables = GetComponentsInChildren<Saveable>();
        foreach (var saveable in saveables)
        {
            if (saveable.name == gameObject.name) continue;
            builder.Append(saveable.Save());
        }
    }
}
