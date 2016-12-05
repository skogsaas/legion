using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegionTest
{
    public interface ParentObject : TestObject
    {
        string Alias { get; set; }
    }
}
