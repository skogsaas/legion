using System;
using System.ComponentModel;
using System.Reflection;

namespace Legion
{
    public class ObjectBase : StructBase, IObject
    {
        private string id;
        private bool published;

        public string Id
        {
            get { return this.id; }
            set {
                if (!this.published)
                {
                    this.id = value;
                }
                else
                {
                    throw new InvalidOperationException("Cannot set the Id of an object after it's published.");
                }
            }
        }

        public ObjectBase()
        {
            this.id = Factory.GenerateId(); // Get a default id until it's set manually
        }

        public ObjectBase(string id)
        {
            this.id = id;
        }
    }
}
