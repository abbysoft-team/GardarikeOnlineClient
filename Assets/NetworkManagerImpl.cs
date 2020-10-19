using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
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
    private List<Response> responseBuffer;

    public void ConnectToServer(string login, string password)
    {
        // Listen to server packets 
        StartZeroMQCommunicationThread();
    }

    private void StartZeroMQCommunicationThread()
    {
        Task.Factory.StartNew(async () =>
        {
            try
            {
                CommunicationTask();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error occured in network thread: " + e);
            }
        });
    }

    private void CommunicationTask()
    {
        Debug.Log("Start listening thread");

        var mapRequest = new GetMapRequest
        {
            Location = new Vector3D
            {
                X = 0,
                Y = 0,
                Z = 0
            }
        };

        var address = string.Format(">tcp://{0}:{1}", serverIp, serverPort);

        using (var requester = new RequestSocket(address))
        {
            Debug.Log("Trying to request map information ");

            // Send Map request
            requester.SendFrame(mapRequest.ToByteArray());

            Debug.Log("Sent map request to server");

            while (true)
            {
                // Receive
                var reply = requester.ReceiveFrameBytes();

                Debug.Log("RECEIVED RAW PACKET " + reply.Length);

                ParseResponse(reply);
            }
        }
    }

    private void ParseResponse(byte[] rawResponse)
    {
        try
        {
            var response = Response.Parser.ParseFrom(rawResponse);
            Debug.Log("Received server packet " + response);
            responseBuffer.Add(response);
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

        responseBuffer = new List<Response>();
    }

    public void SendEvent(Event eventObject)
    {
    }

    public List<Response> getEventBuffer()
    {
        return responseBuffer;
    }
}
