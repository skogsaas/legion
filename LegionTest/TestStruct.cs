using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegionTest
{
    public interface TestStruct : Legion.IStruct
    {
        string Dummy1 { get; set; }
    }
}
