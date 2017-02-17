using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Skogsaas.Legion
{
    internal class SubscriptionContainer
    {
        private Dictionary<Type, List<Channel.ObjectHandler>> objPub;
        private Dictionary<Type, List<Channel.ObjectHandler>> objUnPub;
        private Dictionary<string, List<Channel.ObjectHandler>> objPubId;
        private Dictionary<string, List<Channel.ObjectHandler>> objUnPubId;
        private Dictionary<Type, List<Channel.EventHandler>> evtPub;

        public SubscriptionContainer()
        {
            this.objPub = new Dictionary<Type, List<Channel.ObjectHandler>>();
            this.objUnPub = new Dictionary<Type, List<Channel.ObjectHandler>>();
            this.objPubId = new Dictionary<string, List<Channel.ObjectHandler>>();
            this.objUnPubId = new Dictionary<string, List<Channel.ObjectHandler>>();
            this.evtPub = new Dictionary<Type, List<Channel.EventHandler>>();
        }

        public IList<Channel.ObjectHandler> GetObjectTypePublish(Type type)
        {
            List<Channel.ObjectHandler> handlers = new List<Channel.ObjectHandler>();

            foreach(var pair in this.objPub)
            {
                if(pair.Key.IsAssignableFrom(type))
                {
                    handlers.AddRange(pair.Value);
                }
            }

            return handlers;
        }

        public IList<Channel.ObjectHandler> GetObjectTypeUnpublish(Type type)
        {
            List<Channel.ObjectHandler> handlers = new List<Channel.ObjectHandler>();

            foreach (var pair in this.objUnPub)
            {
                if (pair.Key.IsAssignableFrom(type))
                {
                    handlers.AddRange(pair.Value);
                }
            }

            return handlers;
        }

        public IList<Channel.ObjectHandler> GetObjectIdPublish(string id)
        {
            List<Channel.ObjectHandler> handlers = new List<Channel.ObjectHandler>();

            if(this.objPubId.ContainsKey(id))
            {
                handlers.AddRange(this.objPubId[id]);
            }

            return handlers;
        }

        public IList<Channel.ObjectHandler> GetObjectIdUnpublish(string id)
        {
            List<Channel.ObjectHandler> handlers = new List<Channel.ObjectHandler>();

            if (this.objUnPubId.ContainsKey(id))
            {
                handlers.AddRange(this.objUnPubId[id]);
            }

            return handlers;
        }

        public IList<Channel.EventHandler> GetEventTypePublish(Type type)
        {
            List<Channel.EventHandler> handlers = new List<Channel.EventHandler>();

            foreach (var pair in this.evtPub)
            {
                if (pair.Key.IsAssignableFrom(type))
                {
                    handlers.AddRange(pair.Value);
                }
            }

            return handlers;
        }

        #region Subscribe

        public void SubscribeObjectPublish(Type type, Channel.ObjectHandler handler)
        {
            if(!this.objPub.ContainsKey(type))
            {
                this.objPub[type] = new List<Channel.ObjectHandler>();
            }

            this.objPub[type].Add(handler);
        }

        public void SubscribeObjectUnpublish(Type type, Channel.ObjectHandler handler)
        {
            if (!this.objUnPub.ContainsKey(type))
            {
                this.objUnPub[type] = new List<Channel.ObjectHandler>();
            }

            this.objUnPub[type].Add(handler);
        }

        public void SubscribeObjectIdPublish(string id, Channel.ObjectHandler handler)
        {
            if (!this.objPubId.ContainsKey(id))
            {
                this.objPubId[id] = new List<Channel.ObjectHandler>();
            }

            this.objPubId[id].Add(handler);
        }

        public void SubscribeObjectIdUnpublish(string id, Channel.ObjectHandler handler)
        {
            if (!this.objUnPubId.ContainsKey(id))
            {
                this.objUnPubId[id] = new List<Channel.ObjectHandler>();
            }

            this.objUnPubId[id].Add(handler);
        }

        public void SubscribeEventPublish(Type type, Channel.EventHandler handler)
        {
            if (!this.evtPub.ContainsKey(type))
            {
                this.evtPub[type] = new List<Channel.EventHandler>();
            }

            this.evtPub[type].Add(handler);
        }

        #endregion

        #region Unsubscribe

        public void UnsubscribeObjectPublish(Type type, Channel.ObjectHandler handler)
        {
            if (!this.objPub.ContainsKey(type))
            {
                return;
            }

            this.objPub[type].Remove(handler);
        }

        public void UnsubscribeObjectUnpublish(Type type, Channel.ObjectHandler handler)
        {
            if (!this.objUnPub.ContainsKey(type))
            {
                return;
            }

            this.objUnPub[type].Remove(handler);
        }

        public void UnsubscribeObjectIdPublish(string id, Channel.ObjectHandler handler)
        {
            if (!this.objPubId.ContainsKey(id))
            {
                return;
            }

            this.objPubId[id].Remove(handler);
        }

        public void UnsubscribeObjectIdUnpublish(string id, Channel.ObjectHandler handler)
        {
            if (!this.objUnPubId.ContainsKey(id))
            {
                return;
            }

            this.objUnPubId[id].Remove(handler);
        }

        public void UnsubscribeEventPublish(Type type, Channel.EventHandler handler)
        {
            if (!this.evtPub.ContainsKey(type))
            {
                return;
            }

            this.evtPub[type].Remove(handler);
        }

        #endregion
    }
}
