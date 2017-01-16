using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skogsaas.Legion;

namespace LegionTest
{
    [TestClass]
    public class ChannelTest
    {
        [TestMethod]
        public void SubscribeIdPrePublish()
        {
            int n = 0;
            Channel channel = Manager.Create("TEST");

            TestObject obj = channel.CreateType<TestObject>("ID");

            channel.SubscribePublishId(obj.Id, (Channel c, IObject o) => { if (obj == o) n++; });

            Assert.AreEqual(0, n);
            channel.Publish(obj);
            Assert.AreEqual(1, n);
        }

        [TestMethod]
        public void SubscribeIdPostPublish()
        {
            int n = 0;
            Channel channel = Manager.Create("TEST");

            TestObject obj = channel.CreateType<TestObject>("ID");

            Assert.AreEqual(0, n);
            channel.Publish(obj);
            Assert.AreEqual(0, n);

            channel.SubscribePublishId(obj.Id, (Channel c, IObject o) => { if (obj == o) n++; });

            Assert.AreEqual(1, n);
        }
    }
}
