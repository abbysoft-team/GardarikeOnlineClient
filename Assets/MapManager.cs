using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using Gardarike;

public class MapManager : MonoBehaviour
{
    public TerrainGenerator terrainGenerator;

    void Start()
    {
        EventBus.instance.onLoginComplete += SendMapRequest;
    }

    void SendMapRequest(string sessionId, RepeatedField<Character> character) {
        Debug.Log("Start loading map");

        EventBus.instance.LoadMap(sessionId);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
