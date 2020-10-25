using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gardarike;
public interface NetworkManager
{
    void Init(string serverUrl, int requestPort, int eventPort);
    Queue<Response> GetResponseQueue();
    Queue<Gardarike.Event> GetEventQueue();
}
