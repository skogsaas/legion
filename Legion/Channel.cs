using System;
using System.Collections;
using System.Collections.Generic;

namespace Skogsaas.Legion
{
    public class Channel : IEnumerable<IObject>
    {
        private Dictionary<string, IObject> objects;

        private SubscriptionContainer subscriptions;

        public string Name { get; private set; }

        internal Factory Factory { get; private set; }

        public delegate void TypeRegisteredHandler(Type type, Type generated);
        public event TypeRegisteredHandler OnTypeRegistered;

        public delegate void ObjectHandler(Channel channel, IObject obj);
        public delegate void EventHandler(Channel channel, IEvent evt);

        internal Channel(string name)
        {
            this.Name = name;

            this.objects = new Dictionary<string, IObject>();
            this.subscriptions = new SubscriptionContainer();

            this.Factory = new Factory();

            // Forward events from factory.
            this.Factory.OnTypeRegistered += (Type type, Type generated) => { this.OnTypeRegistered?.Invoke(type, generated); };
        }

        public void RegisterType(Type type)
        {
            this.Factory.RegisterType(type);
        }

        public Type FindType(Type type)
        {
            return this.Factory.FindType(type);
        }

        public Type FindType(string fullname)
        {
            return this.Factory.FindType(fullname);
        }

        public T CreateType<T>(string id = null) where T : class
        {
            Type type = typeof(T);
            this.Factory.RegisterType(type);

            Type generated = this.Factory.FindType(type);

            return (T)activateIdType(generated, id);
        }

        public object CreateType(string type, string id = null)
        {
            Type generated = this.Factory.FindType(type);

            if(generated != null)
            {
                return (IId)activateIdType(generated, id);
            }

            return null;
        }

        private object activateIdType(Type type, string id = null)
        {
            if (id != null && id.Length > 0)
            {
                return Activator.CreateInstance(type, new object[] { id });
            }
            else
            {
                return Activator.CreateInstance(type);
            }
        }

        public IObject Find(string id)
        {
            if (this.objects.ContainsKey(id))
            {
                return this.objects[id];
            }

            return null;
        }

        public List<T> FindOfType<T>()
        {
            Type type = typeof(T);

            List<T> objects = new List<T>();

            foreach (KeyValuePair<string, IObject> pair in this.objects)
            {
                if (type.IsAssignableFrom(pair.Value.GetType()))
                {
                    objects.Add((T)pair.Value);
                }
            }

            return objects;
        }

        public List<IObject> FindOfType(Type type)
        {
            List<IObject> objects = new List<IObject>();

            foreach (KeyValuePair<string, IObject> pair in this.objects)
            {
                if (type.IsAssignableFrom(pair.Value.GetType()))
                {
                    objects.Add(pair.Value);
                }
            }

            return objects;
        }

        public void Publish(IObject obj)
        {
            (obj as ObjectBase).SetPublished(true);

            this.objects[obj.Id] = obj;
            triggerObjectPublished(obj);
        }

        public void Unpublish(IObject obj)
        {
            if (this.objects.ContainsKey(obj.Id))
            {
                (obj as ObjectBase).SetPublished(true);

                this.objects.Remove(obj.Id);
                triggerObjectUnpublished(obj);
            }
        }

        public void Publish(IEvent evt)
        {
            triggerEventPublished(evt);
        }

        public void SubscribePublish(Type type, ObjectHandler handler)
        {
            this.subscriptions.SubscribeObjectPublish(type, handler);

            List<IObject> objs = FindOfType(type);

            foreach (IObject obj in objs)
            {
                handler.Invoke(this, obj);
            }
        }

        public void SubscribePublish(Type type, EventHandler handler)
        {
            this.subscriptions.SubscribeEventPublish(type, handler);
        }

        public void SubscribeUnpublish(Type type, ObjectHandler handler)
        {
            this.subscriptions.SubscribeObjectUnpublish(type, handler);
        }

        public void SubscribePublishId(string id, ObjectHandler handler)
        {
            this.subscriptions.SubscribeObjectIdPublish(id, handler);

            if(this.objects.ContainsKey(id))
            {
                handler.Invoke(this, this.objects[id]);
            }
        }

        public void SubscribeUnpublishId(string id, ObjectHandler handler)
        {
            this.subscriptions.SubscribeObjectIdUnpublish(id, handler);
        }

        public void UnsubscribePublish(Type type, ObjectHandler handler)
        {
            this.subscriptions.UnsubscribeObjectPublish(type, handler);
        }

        public void UnsubscribePublish(Type type, EventHandler handler)
        {
            this.subscriptions.UnsubscribeEventPublish(type, handler);
        }

        public void UnsubscribeUnpublish(Type type, ObjectHandler handler)
        {
            this.subscriptions.UnsubscribeObjectUnpublish(type, handler);
        }

        public void UnsubscribePublishId(string id, ObjectHandler handler)
        {
            this.subscriptions.UnsubscribeObjectIdPublish(id, handler);
        }

        public void UnsubscribeUnpublishId(string id, ObjectHandler handler)
        {
            this.subscriptions.UnsubscribeObjectIdUnpublish(id, handler);
        }

        #region Private triggers

        private void triggerObjectPublished(IObject obj)
        {
            IList<ObjectHandler> idHandlers = this.subscriptions.GetObjectIdPublish(obj.Id);

            foreach (ObjectHandler handler in idHandlers)
            {
                handler.Invoke(this, obj);
            }

            IList<ObjectHandler> typeHandlers = this.subscriptions.GetObjectTypePublish(obj.GetType());

            foreach(ObjectHandler handler in typeHandlers)
            {
                handler.Invoke(this, obj);
            }
        }

        private void triggerObjectUnpublished(IObject obj)
        {
            IList<ObjectHandler> idHandlers = this.subscriptions.GetObjectIdUnpublish(obj.Id);

            foreach (ObjectHandler handler in idHandlers)
            {
                handler.Invoke(this, obj);
            }

            IList<ObjectHandler> typeHandlers = this.subscriptions.GetObjectTypeUnpublish(obj.GetType());

            foreach (ObjectHandler handler in typeHandlers)
            {
                handler.Invoke(this, obj);
            }
        }

        private void triggerEventPublished(IEvent evt)
        {
            IList<EventHandler> typeHandlers = this.subscriptions.GetEventTypePublish(evt.GetType());

            foreach (EventHandler handler in typeHandlers)
            {
                handler.Invoke(this, evt);
            }
        }

        #endregion

        #region Enumerator

        public IEnumerator<IObject> GetEnumerator()
        {
            return this.objects.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
