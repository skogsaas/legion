using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion
{
    public class Channel
    {
        private Dictionary<string, IObject> objects;

        #region Subscriptions

        private Utilities.MultiDictionary<Type, ObjectHandler> objectPublish;
        private Utilities.MultiDictionary<Type, ObjectHandler> objectUnpublish;
        private Utilities.MultiDictionary<Type, EventHandler> eventPublish;

        #endregion

        public string Name { get; private set; }

        public delegate void ObjectHandler(Channel channel, IObject obj);
        public delegate void EventHandler(Channel channel, IEvent evt);

        internal Channel(string name)
        {
            this.objects = new Dictionary<string, IObject>();
            this.objectPublish = new Utilities.MultiDictionary<Type, ObjectHandler>();
            this.objectUnpublish = new Utilities.MultiDictionary<Type, ObjectHandler>();
            this.eventPublish = new Utilities.MultiDictionary<Type, EventHandler>();

            this.Name = name;
        }

        public void RegisterType(Type type)
        {
            Factory.RegisterType(type);
        }

        public T CreateType<T>() where T : class
        {
            Type type = typeof(T);
            Type generated = Factory.FindType(type);

            T value = (T)Activator.CreateInstance(generated);

            return value;
        }

        public IObject CreateType(string type)
        {
            Type generated = Factory.FindType(type);

            IObject value = (IObject)Activator.CreateInstance(generated);

            return value;
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

        public void Publish(IObject obj)
        {
            this.objects[obj.Id] = obj;
            triggerObjectPublished(obj);
        }

        public void Unpublish(IObject obj)
        {
            if (this.objects.ContainsKey(obj.Id))
            {
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
            this.objectPublish.Add(type, handler);

            foreach (KeyValuePair<string, IObject> pair in this.objects)
            {
                if (type.IsAssignableFrom(pair.Value.GetType()))
                {
                    handler.Invoke(this, pair.Value);
                }
            }
        }

        public void SubscribePublish(Type type, EventHandler handler)
        {
            this.eventPublish.Add(type, handler);
        }

        public void SubscribeUnpublish(Type type, ObjectHandler handler)
        {
            this.objectUnpublish.Add(type, handler);
        }

        public void UnsubscribePublish(Type type, ObjectHandler handler)
        {
            this.objectPublish.Remove(type, handler);
        }

        public void UnsubscribePublish(Type type, EventHandler handler)
        {
            this.eventPublish.Remove(type, handler);
        }

        public void UnsubscribeUnpublish(Type type, ObjectHandler handler)
        {
            this.objectUnpublish.Remove(type, handler);
        }

        #region Private triggers

        private void triggerObjectPublished(IObject obj)
        {
            foreach (KeyValuePair<Type, ObjectHandler> pair in this.objectPublish)
            {
                if (pair.Key.IsAssignableFrom(obj.GetType()))
                {
                    pair.Value.Invoke(this, obj);
                }
            }
        }

        private void triggerObjectUnpublished(IObject obj)
        {
            foreach (KeyValuePair<Type, ObjectHandler> pair in this.objectUnpublish)
            {
                if (pair.Key.IsAssignableFrom(obj.GetType()))
                {
                    pair.Value.Invoke(this, obj);
                }
            }
        }

        private void triggerEventPublished(IEvent evt)
        {
            foreach (KeyValuePair<Type, EventHandler> pair in this.eventPublish)
            {
                if (pair.Key.IsAssignableFrom(evt.GetType()))
                {
                    pair.Value.Invoke(this, evt);
                }
            }
        }

        #endregion
    }
}
