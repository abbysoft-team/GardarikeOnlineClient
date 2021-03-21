using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Google.Protobuf.Collections;

public class UtilityTests : MonoBehaviour
{
    void Start()
    {
        var size = 4;

        var protoHeights = new RepeatedField<float>();
        for (int height = 0; height < 16; height++)
        {
            protoHeights.Add(height);
        }

        var resultHeights = ProtoConverter.ToHeightsFromProto(protoHeights);

        Debug.Log("[");
        for (int i = 0; i < size; i++)
        {
            Debug.Log("\n");
            for (int j = 0; j < size; j++)
            {
                Debug.Log(resultHeights[i, j] + ", ");
            }
        }
    }
}
