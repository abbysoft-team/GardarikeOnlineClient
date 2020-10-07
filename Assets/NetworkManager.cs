using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public interface NetworkManager
{
    void Init(string serverUrl, int port);
    void ConnectToServer(string login, string password);
    void SendEvent(Event eventObject);
}
