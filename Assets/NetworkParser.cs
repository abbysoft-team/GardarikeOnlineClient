using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Gardarike;

public class NetworkParser : MonoBehaviour
{
    private NetworkManager networkManager;

    void Start()
    {
        networkManager = NetworkManagerFactory.GetManager();
    }

    // Update is called once per frame
    void Update()
    {
        // Get all buffered packets
        var packets = networkManager.getEventBuffer();
        // Nothing to process this tick
        if (packets.Count == 0) return;

        Debug.Log(string.Format("Dispatchering {0} packets", packets.Count));

        DispatchPackets(packets);

        // Clear buffer
        packets.Clear();
    }

    private void DispatchPackets(List<Response> packets)
    {
        foreach (var packet in packets)
        {
            switch (packet.DataCase)
            {
                case Response.DataOneofCase.MultipartResponse:
                    ProcessMultipart(packet.MultipartResponse);
                    break;
                case Response.DataOneofCase.GetMapResponse:
                    ProcessMapReply(packet.GetMapResponse);
                    break;
                case Response.DataOneofCase.ErrorResponse:
                    ProcessServerErrorReply(packet.ErrorResponse);
                    break;
                default:
                    ProcessInvalidReply(packet);
                    break;
            }
        }
    }


    private void ProcessMultipart(MultipartResponse response)
    {
        Debug.Log("Receiving " + response.Parts + " packets as part of one big one...");
        Debug.Log("But client can't work with this type of message =( ");
    }

    private void ProcessMapReply(GetMapResponse getMapResponse)
    {
        Debug.Log("Received map reply: " + getMapResponse);
        var width = getMapResponse.Map.Width;
        var height = getMapResponse.Map.Height;
        var heights = ProtoConverter.ToHeightsFromProto(getMapResponse.Map.Points, width, height);
        EventBus.instance.TerrainLoaded(width, height, heights);
    }

    private void ProcessServerErrorReply(ErrorResponse errorResponse)
    {
        Debug.LogError("Server replied with error: " + errorResponse.Message);
    }

    private void ProcessInvalidReply(Response response)
    {
        Debug.LogError("Server responded with invalid reponse " + response);
    }
}
