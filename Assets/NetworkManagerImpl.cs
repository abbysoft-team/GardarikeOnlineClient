using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System.Threading.Tasks;
using NetMQ.Sockets;
using NetMQ;

using UnityEngine;

using Gardarike;

public class NetworkManagerImpl : NetworkManager
{
    private string serverIp;
    private int requestPort;
    private int eventPort;
    private Queue<Response> responseQueue;
    private Queue<Gardarike.Event> eventQueue;
    private Queue<byte[]> requestQueue;

    private void StartZeroMQCommunicationThread()
    {
        Task.Run(() =>
        {
            try
            {
                ClientTask();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error occured in network thread: " + e);
                EventBus.instance.CloseLoadingDialog();
                EventBus.instance.ShowError("Could not connect to server.\nTry again later");
            }
        });

        Task.Run(() =>
        {
            try
            {
                EventTask("GLOBAL");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error occured in network thread: " + e);
                EventBus.instance.CloseLoadingDialog();
                EventBus.instance.ShowError("Could not connect to server.\nTry again later");
            }
        });
    }

    private void RunEventTask(string sessionId, RepeatedField<Character> characters) {
        Task.Run(() =>
        {
            try
            {
                EventTask(sessionId);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error occured in event thread: " + e);
                EventBus.instance.CloseLoadingDialog();
                EventBus.instance.ShowError("Could not connect to server.\nTry again later");
            }
        });
    }

    private void EventTask(string channel)
    {
        Debug.Log("Start event thread on chanel " + channel);

        var address = string.Format(">tcp://{0}:{1}", serverIp, eventPort);
        using (var subscriber = new SubscriberSocket(address))
        {
            // Listen to all topics
            subscriber.Subscribe(channel);
            while (true) {
                var eventPacket = subscriber.ReceiveFrameBytes();

                EnqueueEvent(eventPacket);
            }
        }
    }

    private void ClientTask()
    {
        Debug.Log("Start network thread");

        var address = string.Format(">tcp://{0}:{1}", serverIp, requestPort);

        using (var requester = new RequestSocket(address))
        {
            while (true)
            {
                if (requestQueue.Count <= 0)
                {
                    Thread.Sleep(10);
                    continue;
                }

                SendNextRequest(requester);

                var reply = requester.ReceiveFrameBytes();

                Debug.Log("Received response from server: " + reply.Length);

                EnqueueResponse(reply);
            }
        }
    }

    private void SendAllRequests(NetMQSocket client)
    {
        while (requestQueue.Count > 0) {
            client.TrySendFrame(requestQueue.Dequeue());
        }
    }

    private void SendNextRequest(NetMQSocket client)
    {
        bool success = client.TrySendFrame(TimeSpan.FromSeconds(5), requestQueue.Dequeue());
        if (success == false)
        {
            throw new FaultException();
        }
    }

    private void EnqueueResponse(byte[] rawResponse)
    {
        try
        {
            var response = Response.Parser.ParseFrom(rawResponse);
            responseQueue.Enqueue(response);
        }
        catch
        {
            Debug.LogError("Cannot parse packet: " + BitConverter.ToString(rawResponse));
        }
    }

    private void EnqueueEvent(byte[] eventBytes) 
    {
        try 
        {
            var eventItem = Gardarike.Event.Parser.ParseFrom(eventBytes);
            eventQueue.Enqueue(eventItem);
        }
        catch
        {
            Debug.LogError("Cannot parse event: " + BitConverter.ToString(eventBytes));
        }
    }

    public void Init(string ipAddress, int requestPort, int eventPort)
    {
        PlayerPrefs.DeleteKey("sessionId");
        PlayerPrefs.DeleteKey("userId");
        
        serverIp = ipAddress;
        this.requestPort = requestPort;
        this.eventPort = eventPort;

        responseQueue = new Queue<Response>();
        requestQueue = new Queue<byte[]>();
        eventQueue = new Queue<Gardarike.Event>();

        EventBus.instance.onLoginRequest += SendLoginRequest;
        EventBus.instance.onSelectCharacterRequest += SendCharacterSelectionRequest;
        EventBus.instance.onMapLoadRequest += SendMapRequest;
        EventBus.instance.onBulidingComplete += SendBuildingEvent;
        EventBus.instance.onLoadChatHistoryRequest += SendLoadChatHistoryRequest;
        EventBus.instance.onChatMessagePublishRequest += PublishChatMessage;
        EventBus.instance.onLoginComplete += RunEventTask;
        EventBus.instance.onWorkInfoRequest += SendWorkInfoRequest;

        StartZeroMQCommunicationThread();
    }

    private void SendCharacterSelectionRequest(Character character)
    {
        Debug.Log("Send character selection event");

        var charSelectRequest = new Request
        {
            SelectCharacterRequest = new SelectCharacterRequest
            {
                SessionID = PlayerPrefs.GetString("sessionId"),
                CharacterID = character.Id
            }
        };

        requestQueue.Enqueue(charSelectRequest.ToByteArray());
    }

    private void SendLoginRequest(string username, string password)
    {
        Debug.Log("Login request added into queue");

        var loginRequest = new Request
        {
            LoginRequest = new LoginRequest {
                Username = username,
                Password = password
            }
        };

        requestQueue.Enqueue(loginRequest.ToByteArray());
    }

    private void SendMapRequest(string sessionId)
    {
        Debug.Log("Trying to request map information ");

        var mapRequest = new Request
        {
            GetMapRequest = new GetMapRequest {
                Location = new Vector3D
                {
                    X = 0,
                    Y = 0,
                    Z = 0
                },
                SessionID = sessionId
            }
        };

        requestQueue.Enqueue(mapRequest.ToByteArray());
    }

    private void SendBuildingEvent(BuildItemInfo building)
    {
        Debug.Log("Sending build event to server");

        var buildingEvent = new Request {
            PlaceBuildingRequest = new PlaceBuildingRequest {
                BuildingID = 1,
                SessionID = PlayerPrefs.GetString("sessionId"),
                Location = building.Location()
            }
        };

        requestQueue.Enqueue(buildingEvent.ToByteArray());
    }

    private void SendLoadChatHistoryRequest()
    {
        Debug.Log("Sending chat history request");

        var chatRequest = new Request {
            GetChatHistoryRequest = new GetChatHistoryRequest {
                SessionID = PlayerPrefs.GetString("sessionId"),
                Count = ChatComponent.MAX_MESSAGES_DISPLAYED
                // null lastMessId
            }
        };

        requestQueue.Enqueue(chatRequest.ToByteArray());
    }

    private void PublishChatMessage(string text) 
    {
        Debug.Log("Sending message to the server");

        var publishRequest = new Request {
            SendChatMessageRequest = new SendChatMessageRequest {
                SessionID = PlayerPrefs.GetString("sessionId"),
                Text = text
            }
        };

        requestQueue.Enqueue(publishRequest.ToByteArray());
    }

    private void SendWorkInfoRequest()
    {
        Debug.Log("Request info about job market");

        var jobRequest = new Request {
            GetWorkDistributionRequest = new GetWorkDistributionRequest {
                SessionID = PlayerPrefs.GetString("sessionId")
            }
        };

        requestQueue.Enqueue(jobRequest.ToByteArray());
    }

    public Queue<Response> GetResponseQueue()
    {
        return responseQueue;
    }

    public Queue<Gardarike.Event> GetEventQueue() 
    {
        return eventQueue;
    }
}