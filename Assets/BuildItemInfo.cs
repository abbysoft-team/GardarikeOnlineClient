using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Gardarike;

public class BuildItemInfo
{
    public UnityEngine.Vector3 lossyScale;
    public UnityEngine.Vector3 position;


    public Vector3D Location() {
        return new Vector3D {X = position.x, Y = position.y, Z = position.z};
    }
}
