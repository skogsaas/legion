using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skogsaas.Legion.RestJson
{
    public class DataAccess : IDataAccess
    {
        public Dictionary<string, Channel> Channels { get; set; }

        public DataAccess(params Channel[] channels)
        {
            this.Channels = new Dictionary<string, Channel>();

            foreach(Channel c in channels)
            {
                this.Channels.Add(c.Name, c);
            }
        }
    }
}
