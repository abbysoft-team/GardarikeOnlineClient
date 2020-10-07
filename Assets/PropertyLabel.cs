using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyLabel : MonoBehaviour
{
    public string property;
    public PropertyType type;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var text = GetComponent<Text>();
        if (type == PropertyType.INTEGER)
        {
            text.text = PlayerPrefs.GetInt(property).ToString();
        }
        if (type == PropertyType.FLOAT)
        {
            text.text = PlayerPrefs.GetFloat(property).ToString();
        }
        if (type == PropertyType.STRING)
        {
            text.text = PlayerPrefs.GetString(property);
        }
    }

    public enum PropertyType
    {
        INTEGER,
        FLOAT,
        STRING
    }
}
