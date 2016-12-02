using System;
using System.ComponentModel;
using Legion;

namespace Runner
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Manager manager = new Legion.Manager();
			Channel channel = manager.Create("TEST");

			int events = 0;

			TestObject i = channel.CreateType<TestObject>();
			i.PropertyChanged += (object caller, PropertyChangedEventArgs arg) => { events++; };

			i.Dummy1 = 10;
			i.Dummy2 = channel.CreateType<TestStruct>();
		}
	}
}
