using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skogsaas.Legion
{
    public class EventBase : IEvent
    {
        public string Id { get; private set; }

        public EventBase()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public EventBase(string id)
        {
            this.Id = id;
        }
    }
}
