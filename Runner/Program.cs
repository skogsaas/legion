using System;
using System.ComponentModel;
using Skogsaas.Legion;
using Skogsaas.Legion.Connectivity.Serializer.Json;
using Skogsaas.Legion.Connectivity.Transport.Udp;
using System.Net;
using Skogsaas.Legion.RestJson;

namespace Runner
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Channel channel = Manager.Create("TEST");
            Server restjson = new Server(51337, new Channel[] {channel});
            restjson.Start();
            
            /*
            JsonSerializer serializer = new JsonSerializer(channel);
            UdpTransport transport = new UdpTransport(IPAddress.Parse("239.1.1.100"), 52222, true);
            
            serializer.LinkTo(transport);
            transport.LinkTo(serializer);

            serializer.Subscribe(typeof(IObject));
            */

			TestObject i1 = channel.CreateType<TestObject>();
            TestObject i2 = channel.CreateType<TestObject>();
            TestObject i3 = channel.CreateType<TestObject>();

            channel.Publish(i1);
            channel.Publish(i2);
            channel.Publish(i3);

            Console.ReadLine();
		}
	}
}
