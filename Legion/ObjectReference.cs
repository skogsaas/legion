using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skogsaas.Legion
{
    public class ObjectReference
    {
        private Channel channel;

        public string Id { get; private set; }
        public IObject Object { get; private set; }

        public ObjectReference(Channel c, string id)
        {
            this.channel = c;
            this.Id = id;

            this.channel.SubscribePublishId(this.Id, onObjectPublished);
            this.channel.SubscribeUnpublishId(this.Id, onObjectUnpublished);
        }

        private void onObjectPublished(Channel c, IObject obj)
        {
            this.Object = obj;
        }

        private void onObjectUnpublished(Channel c, IObject obj)
        {
            this.Object = null;
        }
    }
}
