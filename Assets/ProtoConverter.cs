using Google.Protobuf.Collections;

public class ProtoConverter
{
    public static float[,] ToHeightsFromProto(RepeatedField<float> points, int width, int height) {
        float[,] heights = new float[width, height];

        int column = 0;
        int row = 0;
        foreach (var point in points)
        {
            heights[row, column] = point / 10.0f;
           
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