using Google.Protobuf.Collections;
using Gardarike;

public class ProtoConverter
{
    public static float[,] ToHeights(RepeatedField<Vector3D> vectors, int width, int height) {
        float[,] heights = new float[width, height];

        int column = 0;
        int row = 0;
        foreach (var vector in vectors)
        {
            heights[row, column] = vector.Z;
           
            column++;
            if (column >= width)
            {
                column = 0;
                row++;
            }
        }

        return heights;
    }
}