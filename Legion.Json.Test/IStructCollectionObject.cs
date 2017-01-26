using Skogsaas.Legion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Json.Test
{
    public interface IStructCollectionObject : IObject
    {
        List<IDummyStruct> Collection { get; set; }
    }
}
