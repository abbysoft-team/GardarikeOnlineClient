using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;

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
        // Get all buffered packets and events
        var packets = networkManager.GetResponseQueue();
        var events = networkManager.GetEventQueue();
        // Nothing to process this tick
        if (events.Count == 0 && packets.Count == 0) return;

        Debug.Log(string.Format("Dispatchering {0} packets", packets.Count));
        Debug.Log(string.Format("Dispatchering {0} events", events.Count));

        DispatchPackets(packets);
        DispatchEvents(events);

        // Clear buffer
        packets.Clear();
        events.Clear();
    }

    private void DispatchPackets(Queue<Response> packets)
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
                case Response.DataOneofCase.LoginResponse:
                    ProcessLoginResponse(packet.LoginResponse);
                    break;
                case Response.DataOneofCase.PlaceBuildingResponse:
                    Debug.Log("Building response arrived: " + packet.PlaceBuildingResponse);
                    break;
                case Response.DataOneofCase.SelectCharacterResponse:
                    Debug.Log("Character 0 selection confirmed");
                    EventBus.instance.CharacterSelectionConfirmed();
                    break;
                case Response.DataOneofCase.GetChatHistoryResponse:
                    Debug.Log("Received chat history");
                    EventBus.instance.ChatHistoryLoaded(packet.GetChatHistoryResponse.Messages);
                    break;
                case Response.DataOneofCase.SendChatMessageResponse:
                    break;
                default:
                    ProcessInvalidReply(packet);
                    break;
            }
        }
    }

    private void DispatchEvents(Queue<Gardarike.Event> events)
    {
        foreach (var eventItem in events)
        {
            switch (eventItem.PayloadCase) 
            {
                case Gardarike.Event.PayloadOneofCase.BuildingPlacedEvent:
                    ProcessBuildingEvent(eventItem.BuildingPlacedEvent);
                    break;
                case Gardarike.Event.PayloadOneofCase.ChatMessageEvent:
                    Debug.Log("New chat message");
                    EventBus.instance.NewMessageArrived(eventItem.ChatMessageEvent.Message);
                    break;
                case Gardarike.Event.PayloadOneofCase.CharacterUpdatedEvent:
                    EventBus.instance.CharacterUpdateArrived(eventItem.CharacterUpdatedEvent.NewState);
                    break;
            }
        }
    }

    private void ProcessBuildingEvent(BuildingPlacedEvent buildingEvent) {
        // Filter current user events 
        if (buildingEvent.OwnerID == PlayerPrefs.GetInt("userId")) {
            return;
        }

        Debug.Log(string.Format("User {0} placed building at {1}", buildingEvent.OwnerID, buildingEvent.Location));
        EventBus.instance.RegisterBuilding(BuildItem.FromProtoBuilding(buildingEvent.Location).info);
    }

    private void ProcessMultipart(MultipartResponse response)
    {
        Debug.Log("Receiving " + response.Parts + " packets as part of one big one...");
        Debug.Log("But client can't work with this type of message =( ");
    }

    private void ProcessMapReply(GetMapResponse getMapResponse)
    {
        Debug.Log("Received map reply: " + getMapResponse);
        Debug.Log("Buildings on the map: " + getMapResponse.Map.Buildings.Count);
        Debug.Log("Trees on map: " + getMapResponse.Map.TreesCount);
        var width = getMapResponse.Map.Width;
        var height = getMapResponse.Map.Height;
        var heights = ProtoConverter.ToHeightsFromProto(getMapResponse.Map.Points, width, height);
        EventBus.instance.TerrainLoaded(width, height, heights);
        EventBus.instance.MapObjectsLoaded(getMapResponse.Map.Buildings, getMapResponse.Map.TreesCount);
    }

    private void ProcessLoginResponse(LoginResponse loginResponse) {
        Debug.Log("Received login response: " + loginResponse);

        EventBus.instance.LoginComplete(loginResponse.SessionID, loginResponse.Characters);
    }

    private void ProcessServerErrorReply(ErrorResponse errorResponse)
    {
        Debug.LogError("Server replied with error: " + errorResponse.Message);
        EventBus.instance.ShowError(errorResponse.Message);
    }

    private void ProcessInvalidReply(Response response)
    {
        Debug.LogError("Server responded with invalid reponse " + response);
        EventBus.instance.ShowError("Server sent invalid response");
    }
}
