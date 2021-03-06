using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using System;
using System.Threading.Tasks;
using System.Threading;

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

        //Debug.Log(string.Format("Dispatchering {0} packets", packets.Count));
        //Debug.Log(string.Format("Dispatchering {0} events", events.Count));

        DispatchPackets(packets);
        DispatchEvents(events);

        // Clear buffer
        events.Clear();
    }

    private void DispatchPackets(Queue<Response> queue)
    {
        var packets = queue.ToArray();
        queue.Clear();
        foreach (var packet in packets)
        {
            try {
                Debug.Log("Dispatching: " + packet);
                DispatchPacket(packet);
            } catch (Exception e)
            {
                Debug.LogError("Error while process packet: " + e);
                Debug.LogError("Packet: " + packet);
            }
        }
    }

    private void DispatchPacket(Response packet)
    {
        switch (packet.DataCase)
        {
            case Response.DataOneofCase.MultipartResponse:
                ProcessMultipart(packet.MultipartResponse);
                break;
            case Response.DataOneofCase.GetWorldMapResponse:
                ProcessWorldMapReply(packet.GetWorldMapResponse);
                break;
            case Response.DataOneofCase.GetLocalMapResponse:
                ProcessLocalChunksReply(packet.GetLocalMapResponse);
                break;
            case Response.DataOneofCase.ErrorResponse:
                ProcessServerErrorReply(packet.ErrorResponse);
                break;
            case Response.DataOneofCase.LoginResponse:
                ProcessLoginResponse(packet.LoginResponse);
                break;
                // case Response.DataOneofCase.PlaceBuildingResponse:
                //     Debug.Log("Building response arrived: " + packet.PlaceBuildingResponse);
                //     break;
            case Response.DataOneofCase.SelectCharacterResponse:
                    Debug.Log("Character 0 selection confirmed");
                    EventBus.instance.CharacterSelectionConfirmed(packet.SelectCharacterResponse.Towns);
                    break;
                case Response.DataOneofCase.GetChatHistoryResponse:
                    Debug.Log("Received chat history");
                    EventBus.instance.ChatHistoryLoaded(packet.GetChatHistoryResponse.Messages);
                    break;
                case Response.DataOneofCase.SendChatMessageResponse:
                    break;
                case Response.DataOneofCase.CreateAccountResponse:
                    Debug.Log("Account created");
                    EventBus.instance.NotifyRegistrationComplete();
                    break;
                case Response.DataOneofCase.CreateCharacterResponse:
                    Debug.Log("Character " + PlayerPrefs.GetString(GlobalConstants.COUNTRY_NAME_PROPERTY) + "created");
                    // select new character
                    EventBus.instance.SelectCharacterRequest(packet.CreateCharacterResponse.Id);
                    break;
                case Response.DataOneofCase.PlaceTownResponse:
                    Debug.Log("Town placed");
                    EventBus.instance.TownPlacedResponse(packet.PlaceTownResponse);
                    break;
                case Response.DataOneofCase.GetResourcesResponse:
                    Debug.Log("Resources received " + packet.GetResourcesResponse.Resources);
                    EventBus.instance.ResourceUpdateReceived(packet.GetResourcesResponse.Resources);
                    break;
                case Response.DataOneofCase.GetEmpiresRatingResponse:
                    Debug.Log("Empires rating received");
                    EventBus.instance.TopEmpiresStatisticReceived(packet.GetEmpiresRatingResponse);
                    break;
                default:
                    ProcessInvalidReply(packet);
                    break;
            }
    }

    private void DispatchEvents(Queue<Gardarike.Event> events)
    {
        foreach (var eventItem in events)
        {
            switch (eventItem.PayloadCase) 
            {
                // case Gardarike.Event.PayloadOneofCase.BuildingPlacedEvent:
                //     ProcessBuildingEvent(eventItem.BuildingPlacedEvent);
                //     break;
                case Gardarike.Event.PayloadOneofCase.ChatMessageEvent:
                    Debug.Log("New chat message");
                    EventBus.instance.NewMessageArrived(eventItem.ChatMessageEvent.Message);
                    break;
                // case Gardarike.Event.PayloadOneofCase.CharacterUpdatedEvent:
                //     EventBus.instance.CharacterUpdateArrived(eventItem.CharacterUpdatedEvent.NewState);
                //     break;
                // case Gardarike.Event.PayloadOneofCase.ResourceUpdatedEvent:
                //     Debug.Log("Resource update: " + eventItem.ResourceUpdatedEvent);
                //     EventBus.instance.UpdateResources(eventItem.ResourceUpdatedEvent);
                //     break;
            }
        }
    }

    // private void ProcessBuildingEvent(BuildingPlacedEvent buildingEvent) {
    //     // Filter current user events 
    //     if (buildingEvent.OwnerID == PlayerPrefs.GetInt("userId")) {
    //         return;
    //     }

    //     Debug.Log(string.Format("User {0} placed building at {1}", buildingEvent.OwnerID, buildingEvent.Location));
    //     EventBus.instance.RegisterBuilding(BuildItem.FromProtoBuilding(buildingEvent.Location).info);
    // }

    private void ProcessMultipart(MultipartResponse response)
    {
        Debug.Log("Receiving " + response.Parts + " packets as part of one big one...");
        Debug.Log("But client can't work with this type of message =( ");
    }

    private void ProcessWorldMapReply(GetWorldMapResponse getMapResponse)
    {
        Debug.Log("Received map reply: " + getMapResponse);

        EventBus.instance.WorldMapChunkLoaded(getMapResponse);
    }

    private void ProcessLocalChunksReply(GetLocalMapResponse localChunksResponse)
    {
        Debug.Log("Received local chunks reply: " + localChunksResponse);

        EventBus.instance.LocalMapChunksLoaded(localChunksResponse);
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
