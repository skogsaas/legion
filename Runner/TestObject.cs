using System;
namespace Runner
{
	public interface TestObject : Skogsaas.Legion.IObject
	{
		int Dummy1 { get; set; }
		TestStruct Dummy2 { get; set; }
	}
}
