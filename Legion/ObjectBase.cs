using System;

namespace Skogsaas.Legion
{
    public class ObjectBase : StructBase, IObject
    {
        private bool published;

        public string Id { get; set; }
        public string Owner { get; set; }

        public ObjectBase()
        {
            this.published = false;
            this.Id = Guid.NewGuid().ToString();
        }

        public ObjectBase(string id)
        {
            this.published = false;
            this.Id = id;
        }

        public bool IsPublished()
        {
            return this.published;
        }

        internal void SetPublished(bool state)
        {
            this.published = state;
        }
    }
}
