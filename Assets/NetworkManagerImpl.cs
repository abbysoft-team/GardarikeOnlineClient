using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Google.Protobuf;
using System.Threading.Tasks;
using NetMQ.Sockets;
using NetMQ;

using UnityEngine;

using Gardarike;

public class NetworkManagerImpl : NetworkManager
{
    private string serverIp;
    private int serverPort;
    private Queue<Response> responseQueue;
    private Queue<byte[]> requestQueue;

    private void StartZeroMQCommunicationThread()
    {
        Task.Run(() =>
        {
            try
            {
                //ServerTask();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error occured in network thread: " + e);
                EventBus.instance.ShowError("Could not connect to server.\nTry again later");
            }
        });

        Task.Run(() =>
        {
            try
            {
                ClientTask();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error occured in network thread: " + e);
                EventBus.instance.ShowError("Could not connect to server.\nTry again later");
            }
        });
    }

    private void ClientTask()
    {
        Debug.Log("Start response thread");

        var address = string.Format(">tcp://{0}:{1}", serverIp, serverPort);

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

    private void ServerTask()
    {
        Debug.Log("Start listening thread");

        var address = string.Format(">tcp://{0}:{1}", serverIp, serverPort);

        using (var requester = new DealerSocket(address))
        {
            while (true)
            {
                // Receive
                var reply = requester.ReceiveFrameBytes();

                Debug.Log("RECEIVED RAW PACKET " + reply.Length);

                EnqueueResponse(reply);
            }
        }
    }

    private void SendAllRequests(NetMQSocket client)
    {
        Debug.Log("Sending " + requestQueue.Count + " packets");
        while (requestQueue.Count > 0) {
            client.TrySendFrame(requestQueue.Dequeue());
            Debug.Log("Sent packets");
        }
    }

    private void SendNextRequest(NetMQSocket client)
    {
        Debug.Log("Sending " + requestQueue.Count + " packets");
        bool success = client.TrySendFrame(requestQueue.Dequeue());
        Debug.Log("Sent packets with success = " + success);
    }

    private void EnqueueResponse(byte[] rawResponse)
    {
        try
        {
            var response = Response.Parser.ParseFrom(rawResponse);
            Debug.Log("Received server packet " + response);
            responseQueue.Enqueue(response);
        }
        catch
        {
            Debug.LogError("Cannot parse packet: " + rawResponse);
        }
    }

    public void Init(string ipAddress, int port)
    {
        serverIp = ipAddress;
        serverPort = port;

        responseQueue = new Queue<Response>();
        requestQueue = new Queue<byte[]>();

        EventBus.instance.onLoginRequest += SendLoginRequest;
        EventBus.instance.onMapLoadRequest += SendMapRequest;
        EventBus.instance.onBuildingRegistrationEvent += SendBuildingEvent;

        StartZeroMQCommunicationThread();
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

        Debug.Log("Sending " + BitConverter.ToString(loginRequest.ToByteArray()));
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

    private void SendBuildingEvent(BuildItem building) {
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

    public void SendEvent(Event eventObject)
    {
    }

    public Queue<Response> GetEventQueue()
    {
        return responseQueue;
    }
}
