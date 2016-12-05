using Skogsaas.Legion;

namespace LegionTest
{
    public interface TestObject : IObject
    {
        int Dummy1 { get; set; }
        TestStruct Dummy2 { get; set; }
    }
}
