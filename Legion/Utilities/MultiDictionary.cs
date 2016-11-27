using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Utilities
{
    internal class MultiDictionary<Key, Value> : IEnumerable<KeyValuePair<Key, Value>>
    {
        protected Dictionary<Key, List<Value>> internalValues;

        public List<Value> this[Key key]
        {
            get
            {
                List<Value> lstValues = null;
                if (!this.internalValues.TryGetValue(key, out lstValues))
                {
                    lstValues = new List<Value>();
                    this.internalValues[key] = lstValues;
                }
                return lstValues;
            }
        }

        public MultiDictionary()
        {
            this.internalValues = new Dictionary<Key, List<Value>>();
        }

        public MultiDictionary(MultiDictionary<Key, Value> rhs)
        {
            this.internalValues = new Dictionary<Key, List<Value>>(rhs.internalValues);
        }

        public void Add(Key key, Value value)
        {
            if (!this.internalValues.ContainsKey(key))
            {
                List<Value> values = new List<Value>();
                values.Add(value);
                this.internalValues.Add(key, values);
            }
            else
            {
                this.internalValues[key].Add(value);
            }
        }

        public void Remove(Key key, Value value)
        {
            if (this.internalValues.ContainsKey(key))
            {
                this.internalValues[key].Remove(value);
            }
        }

        public void Remove(Key key)
        {
            this.internalValues.Remove(key);
        }

        public bool ContainsKey(Key key)
        {
            return this.internalValues.ContainsKey(key);
        }

        public bool Contains(Key key, Value value)
        {
            if (this.internalValues.ContainsKey(key))
            {
                if (this.internalValues[key].Contains(value))
                {
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            this.internalValues.Clear();
        }

        public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator()
        {
            foreach(KeyValuePair<Key, List<Value>> pair in this.internalValues)
            {
                foreach(Value value in pair.Value)
                {
                    yield return new KeyValuePair<Key, Value>(pair.Key, value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
