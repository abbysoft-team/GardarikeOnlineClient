using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using Google.Protobuf;
using System.Threading.Tasks;
using UnityEngine;
using Gardarike;

public class NetworkManagerImpl : NetworkManager
{
    private const string SERVER_URL = "https://server.abbysoft.org:27015";
    private UdpClient client;
    private UdpClient socket;

    public void ConnectToServer(string login, string password)
    {
        // Listen to server packets 
        StartListeningThread();
        // Request Map
        GetMapRequest mapRequest = new GetMapRequest
        {
            Location = new Vector3D
            { 
                X = 1,
                Y = 2,
                Z = 3
            }
        };

        var bytes = mapRequest.ToByteArray();

        client.Send(bytes, bytes.Length);
    }

    private void StartListeningThread()
    {
        //start listening for messages and copy the messages back to the client
        Task.Factory.StartNew(async () => {
            while (true)
            {
                var result = await socket.ReceiveAsync();
                var response = Response.Parser.ParseFrom(result.Buffer);

                switch (response.DataCase)
                {
                    case Response.DataOneofCase.GetMapResponse:
                        ProcessMapReply(response.GetMapResponse);
                        break;
                    case Response.DataOneofCase.ErrorResponse:
                        ProcessServerErrorReply(response.ErrorResponse);
                        break;
                    default:
                        ProcessInvalidReply(response);
                        break;
                }
            }
        });
    }

    private void ProcessMapReply(GetMapResponse getMapResponse)
    {
        var width = getMapResponse.Map.Width;
        var height = getMapResponse.Map.Height;
        var heights = ProtoConverter.ToHeights(getMapResponse.Map.Points, width, height);
        EventBus.instance.TerrainLoaded(width, height, heights);
    }

    private void ProcessServerErrorReply(ErrorResponse errorResponse)
    {
        Debug.LogError("Server replied with error: " + errorResponse.Message);
    }

    private void ProcessInvalidReply(Response response)
    {
        Debug.LogError("Server responded with invalid reponse");
    }

    public void Init(string ipAddress, int port)
    {
        client = new UdpClient(ipAddress, port);

        IPEndPoint listenAddress = new IPEndPoint(IPAddress.Any, port);
        socket = new UdpClient(listenAddress);
    }

    public void SendEvent(Event eventObject)
    {
    }
}
