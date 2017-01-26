using Skogsaas.Legion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegionTest
{
    public interface CollectionObject : IObject
    {
        List<TestStruct> Structs { get; set; }
    }
}
