using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skogsaas.Legion
{
    public interface IEvent : IId
    {
        Type GetInterface();
    }
}
