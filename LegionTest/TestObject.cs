using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegionTest
{
    public interface TestObject : Legion.IObject
    {
        int Dummy1 { get; set; }
        TestStruct Dummy2 { get; set; }
    }
}
