﻿using System;

namespace Skogsaas.Legion
{
    public class ObjectBase : StructBase, IObject
    {
        public string Id { get; private set; }

        public ObjectBase()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public ObjectBase(string id)
        {
            this.Id = id;
        }
    }
}
