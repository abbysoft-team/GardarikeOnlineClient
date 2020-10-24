﻿using System;
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
                ServerTask();
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

        using (var requester = new DealerSocket(address))
        {
            while (true)
            {
                if (requestQueue.Count <= 0)
                {
                    Thread.Sleep(10);
                    continue;
                }

                SendAllRequests(requester);
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

    private void SendAllRequests(DealerSocket client)
    {
        Debug.Log("Sending " + requestQueue.Count + " packets");
        while (requestQueue.Count > 0) {
            client.SendFrame(requestQueue.Dequeue());
        }
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
        EventBus.instance.onBuildingComplete += SendBuildingEvent;

        StartZeroMQCommunicationThread();
    }

    private void SendLoginRequest(string username, string password)
    {
        Debug.Log("Login request added into queue");
        var loginRequest = new LoginRequest
        {
            Username = username,
            Password = password
        };

        requestQueue.Enqueue(loginRequest.ToByteArray());
    }

    private byte[] SendMapRequest()
    {
        Debug.Log("Trying to request map information ");

        var mapRequest = new GetMapRequest
        {
            Location = new Vector3D
            {
                X = 0,
                Y = 0,
                Z = 0
            }
        };

        return mapRequest.ToByteArray();
    }

    private void SendBuildingEvent(BuildItem bulding) {
        Debug.Log("Sending build event to server");

        //var buildingEvent = new Buildi
    }

    public void SendEvent(Event eventObject)
    {
    }

    public Queue<Response> GetEventQueue()
    {
        return responseQueue;
    }
}
