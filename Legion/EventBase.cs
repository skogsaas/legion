using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skogsaas.Legion
{
    public class EventBase : IEvent
    {
        protected Type iface;
        public string Id { get; set; }

        public EventBase()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public EventBase(string id)
        {
            this.Id = id;
        }

        public virtual Type GetInterface()
        {
            return iface;
        }
    }
}
