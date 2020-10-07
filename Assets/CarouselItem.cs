using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarouselItem : MonoBehaviour
{
    public string enableCondition;

    void Start()
    {
        var enabled = ParseEnableCondition(enableCondition);
    }

    private bool ParseEnableCondition(string enableCondition)
    {
        if (enableCondition == null || enableCondition.Trim() == "") return true;

        if (enableCondition.StartsWith("property"))
        {
            return ParsePropertyCondition(enableCondition.Substring(9));
        }

        return false;
    }

    private bool ParsePropertyCondition(string condition)
    {
        var propertyName = condition.Split(' ')[0];

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
