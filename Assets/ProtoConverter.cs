using Google.Protobuf.Collections;
using UnityEngine;

public class ProtoConverter
{
    public static float[,] ToHeightsFromProto(RepeatedField<float> points, int width, int height) {
        float[,] heights = new float[width, height];

        int column = 0;
        int row = 0;
        foreach (var point in points)
        {
            heights[column, row] = point;
           
            column++;
            if (column >= width)
            {
                column = 0;
                row++;
            }
        }

        return heights;
    }

    public static UnityEngine.Vector3 ToUnityVector(Gardarike.Vector3D vector) {
        return new UnityEngine.Vector3(vector.X, vector.Y, vector.Z);
    }
}