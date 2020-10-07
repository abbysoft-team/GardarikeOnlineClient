using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface Event
{
    EventType GetType();
    byte[] GetPayload();

    
}
public enum EventType
{
    BUILDING_PLACED
}
