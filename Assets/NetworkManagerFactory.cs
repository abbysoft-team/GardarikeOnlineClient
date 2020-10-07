using System;
using System.Collections.Generic;
using System.Net.Sockets;

public class NetworkManagerFactory
{
    private static NetworkManager instance = new NetworkManagerImpl();

    public static NetworkManager GetManager()
    {
        return instance;
    }
}
