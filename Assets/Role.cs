using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface Role
{
    void Init(PeopleController man);
    void ActionAccomplished(PeopleController man);

}
