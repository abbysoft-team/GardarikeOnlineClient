using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;

public class CacheMissException : System.Exception
{
    public CacheMissException() { }
    public CacheMissException(int x, int y) : base("cacheMiss(" + x + "; " + y +")") { }
    public CacheMissException(int x, int y, System.Exception inner) : base("cacheMiss(" + x + "; " + y +")", inner) { }
    protected CacheMissException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

public class MapCache : MonoBehaviour
{
    void Start()
    {
    }

    public static void StoreWorldChunk(Gardarike.GetWorldMapResponse map)
    {
        if (!ChunkIsMissing(map.Map.X, map.Map.Y, true)) return;

        byte[] buffer = new byte[map.CalculateSize()];
        using (var output = new CodedOutputStream(new MemoryStream(buffer))) {
            map.WriteTo(output);
        }

        var key = GetKey(map.Map.X, map.Map.Y, true);

        PlayerPrefs.SetString(key, BitConverter.ToString(buffer));

        Debug.Log("Map chunk " + key + " saved. Size " + buffer.Length);
    }

    public static Gardarike.GetWorldMapResponse GetGlobalChunk(int x, int y)
    {
        // if (ChunkIsMissing(x, y, true)) 
        // {
        //     Debug.LogError("Chunk cache miss for chunk " + x + ";" + y);
        //     //EventBus.instance.LoadMap(PlayerPrefs.GetString("sessionId"), x, y);
        //     throw new CacheMissException(x, y);
        // }
        Debug.Log("Loading " + x + ";" + y + " from cache");
        return Deserialize(x, y);
    }

    public static bool ChunkIsMissing(int x, int y, bool isWorldMap)
    {
        return !PlayerPrefs.HasKey(GetKey(x, y, true));
    }

    private static string GetKey(int x, int y, bool isWorldMap)
    {
        var type = isWorldMap ? "G" : "L";

        return type + x + ";" + y;
    }

    private static Gardarike.GetWorldMapResponse Deserialize(int x, int y)
    {
        var encodedString = PlayerPrefs.GetString(GetKey(x, y, true));
        var bytes = GetBytes(encodedString);
        return Gardarike.GetWorldMapResponse.Parser.ParseFrom(bytes);
    }

    private static byte[] GetBytes(string encoded)
    {
        String[] arr = encoded.Split('-');
        byte[] array = new byte[arr.Length];
        for(int i=0; i<arr.Length; i++) array[i]=Convert.ToByte(arr[i],16);

        return array;
    }

    public static void DropCache()
    {
        for (int i = 0; i < 1000; i++)
        {
            for (int j = 0; j < 1000; j++)
            {
                if (ChunkIsMissing(i, j, true)) continue;

                PlayerPrefs.DeleteKey(GetKey(i, j, true));
            }
        }
    }


}
