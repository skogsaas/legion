using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;

namespace LegionTest
{
    [TestClass]
    public class FactoryTest
    {
        public static Legion.Manager manager;
        public static Legion.Channel channel;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            FactoryTest.manager = new Legion.Manager();
            FactoryTest.channel = FactoryTest.manager.Create("TEST");
        }

        [TestMethod]
        public void TestCreate()
        {
            int oEvents = 0;
            int sEvents = 0;

            TestObject i = FactoryTest.channel.CreateType<TestObject>();
            i.PropertyChanged += (object caller, PropertyChangedEventArgs args) => { oEvents++; };

            Assert.AreEqual(0, oEvents);
            i.Dummy1 = 10;
            Assert.AreEqual(1, oEvents);

            Assert.IsNull(i.Dummy2);
            i.Dummy2 = FactoryTest.channel.CreateType<TestStruct>();
            Assert.IsNotNull(i.Dummy2);

            Assert.AreEqual(2, oEvents);

            i.Dummy2.PropertyChanged += (object caller, PropertyChangedEventArgs args) => { sEvents++; };
            Assert.AreEqual(0, sEvents);
            i.Dummy2.Dummy1 = "Hello World!";
            Assert.AreEqual(1, sEvents);
            Assert.AreEqual(3, oEvents);
        }
    }
}