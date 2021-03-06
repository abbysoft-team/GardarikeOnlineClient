using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogInfo
{
    public int id;
    public string title;
    public string message;
    public string inputProperty;
    public DialogType type;

    public DialogInfo(int id, string title, string message, string inputProperty, DialogType type)
    {
        this.id = id;
        this.title = title;
        this.message = message;
        this.inputProperty = inputProperty;
        this.type = type;
    }
}
