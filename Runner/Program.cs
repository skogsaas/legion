using System;
using System.ComponentModel;
using Skogsaas.Legion;

namespace Runner
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Channel channel = Manager.Create("TEST");

			int events = 0;

			TestObject i = channel.CreateType<TestObject>();
			i.PropertyChanged += (object caller, PropertyChangedEventArgs arg) => { events++; };

			i.Dummy1 = 10;
			i.Dummy2 = channel.CreateType<TestStruct>();
		}
	}
}
