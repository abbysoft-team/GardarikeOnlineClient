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
    private byte[] buffer;
    private int multipartLengthBytes;
    private long multipartLengthPackets;


    public void ConnectToServer(string login, string password)
    {
        // Listen to server packets 
        StartCommunicationThread();

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

    private void StartCommunicationThread()
    {
        //start listening for messages and copy the messages back to the client
        Task.Factory.StartNew(async () =>
        {
            while (true)
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
                var rawResponse = client.Receive(ref endpoint);

                Debug.Log("received raw response: " + rawResponse.Length);

                if (multipartLengthPackets > 0)
                {
                    Debug.Log((multipartLengthPackets - 1) + " to go");
                    Buffer.BlockCopy(rawResponse, 0, buffer, multipartLengthBytes, rawResponse.Length);
                    multipartLengthBytes += rawResponse.Length;
                    multipartLengthPackets--;
                }
                else
                {
                    Response response;
                    if (multipartLengthBytes > 0) {
                        Debug.Log("Received big packet, size: " + multipartLengthBytes);

                        byte[] bigPacket = new byte[multipartLengthBytes];
                        Buffer.BlockCopy(buffer, 0, bigPacket, 0, multipartLengthBytes);
                        response = Response.Parser.ParseFrom(bigPacket);
                    } else {
                        response = Response.Parser.ParseFrom(rawResponse);
                    }

                    Debug.Log("Received server packet " + response);

                    switch (response.DataCase)
                    {
                        case Response.DataOneofCase.MultipartResponse:
                            ProcessMultipart(response.MultipartResponse);
                            break;
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
            }
        });
    }

    private void ProcessMultipart(MultipartResponse response)
    {
        Debug.Log("Receiving " + response.Parts + " packets as part of one big one...");
        multipartLengthBytes = 0;
        multipartLengthPackets = response.Parts;
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

        buffer = new byte[1024 * 1024 * 5];

        //listenAddress = new IPEndPoint(IPAddress.Parse(ipAddress), 27015);
        //socket = new UdpClient(listenAddress);
        //socket.Client.ReceiveTimeout = 99000;
    }

    public void SendEvent(Event eventObject)
    {
    }
}
