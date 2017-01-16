using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skogsaas.Legion.RestJson
{
    public interface IDataAccess
    {
        Dictionary<string, Channel> Channels { get; set; }
    }
}
